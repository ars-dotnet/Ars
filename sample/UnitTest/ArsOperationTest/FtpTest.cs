using FluentFTP;
using FluentFTP.Rules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Loggers;
using System.Diagnostics;

namespace ArsOperationTest
{
    /// <summary>
    /// 测试ftp上传与下载
    /// </summary>
    public class FtpTest
    {
        private readonly IServiceProvider _serviceProvider;
        public FtpTest()
        {
            IHost host = Host.CreateDefaultBuilder()
             .ConfigureAppConfiguration(builder =>
             {

             })
             .ConfigureServices((builder, service) =>
             {
                 service.AddLogging();

                 service.AddScoped(_ => new MyService());
                 service.AddSingleton(_ => new MyService());
                 service.AddTransient(_ => new MyService());
             }).
             ConfigureLogging((hostingContext, logging) =>
             {
                 logging.AddLog4Net("Configs/log4net.Config");
                 logging.AddArsLog4Net("Configs/arslog4net.Config");
             })
             .Build();

            _serviceProvider = host.Services;
        }

        [Fact]
        public async Task UploadDirectory()
        {
            using (var conn = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                var token = CancellationToken.None;
                await conn.Connect(token);

                Assert.True(conn.IsConnected);

                // upload a folder and all its files
                var data1 = await conn.UploadDirectory(
                    @"C:\core\Publish\FtpClientFile\", 
                    @"/public_html01/imgs", FtpFolderSyncMode.Update, token: token);

                // upload a folder and all its files, and delete extra files on the server
                //会删除服务器额外的文件
                data1 = await conn.UploadDirectory(
                    @"C:\core\Publish\FtpClientFile", 
                    @"/public_html02/imgs", FtpFolderSyncMode.Mirror, token: token);
            }
        }

        [Fact]
        public async Task UploadDirectoryWithRulesAsync()
        {
            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                var token = new CancellationToken();

                await ftp.Connect(token);

                // upload only PDF files under 1 GB from a folder, by using the rule engine
                var rules = new List<FtpRule>{
                   new FtpFileExtensionRule(true, new List<string>{ "png" }),  // only allow png files
				   new FtpSizeRule(FtpOperator.LessThan, 1000000000)           // only allow files <1 GB
				};
                await ftp.UploadDirectory(@"C:\core\Publish\FtpClientFile\", @"/public_html01/imgs",
                    FtpFolderSyncMode.Update, FtpRemoteExists.Skip, FtpVerify.None, rules, token: token);


                // upload all files from a folder, but skip the sub-directories named `.git`, `.svn`, `node_modules` etc
                var rules2 = new List<FtpRule>{
                   new FtpFolderNameRule(false, FtpFolderNameRule.CommonBlacklistedFolders),
                };
                await ftp.UploadDirectory(@"C:\core\Publish\FtpClientFile\", @"/public_html02/imgs",
                    FtpFolderSyncMode.Update, FtpRemoteExists.Skip, FtpVerify.None, rules2, token: token);

            }
        }

        [Fact]
        public async Task UploadFileAsync()
        {
            var token = new CancellationToken();
            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                await ftp.Connect(token);

                // upload a file and ensure the FTP directory is created on the server
                await ftp.UploadFile(
                    @"C:\core\Publish\FtpClientFile\ccl100.png", 
                    "/public_html01/20231220imgs/ccl100b.png", 
                    FtpRemoteExists.Overwrite, true, token: token);

                // upload a file to an existing FTP directory
                // 服务器文件夹不存在会抛错
                await ftp.UploadFile(
                    @"C:\core\Publish\FtpClientFile\ccl100.png", 
                    "/public_html01/20231220imgs/ccl100a.png", token: token);

                // upload a file and ensure the FTP directory is created on the server, verify the file after upload
                var datas = await ftp.UploadFile(
                    @"C:\core\Publish\FtpClientFile\ccl100.png",
                    "/public_html01/20231220imgs/ccl100c.png", 
                    FtpRemoteExists.Overwrite, true, FtpVerify.Retry, token: token);
            }
        }

        [Fact]
        public async Task UploadFileWithProgressAsync()
        {
            var loggerFac = _serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.FtpLog");
            HashSet<int> codes = new HashSet<int>();

            var token = new CancellationToken();
            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                await ftp.Connect(token);

                // define the progress tracking callback
                Progress<FtpProgress> progress = new Progress<FtpProgress>(p => {
                    var code = (int)p.Progress;
                    if (codes.Add(code)) {
                        logger.LogInformation(
                            $"文件上传进度:{code}%，上传速度:{p.TransferSpeedToString()}");
                    }
                });

                Stopwatch sw = new Stopwatch();
                sw.Start();

                // upload a file with progress tracking
                await ftp.UploadFile(
                    @"C:\core\Publish\FtpClientFile\ideas\cn_visio_professional_2013_x64_1138440.exe",
                    "/public_html01/20231220imgs/ideas/visio_professional_2013_x64.exe", 
                    FtpRemoteExists.Skip, true, FtpVerify.None, progress, token);

                sw.Stop();

                logger.LogInformation($"上传文件耗时:{sw.ElapsedMilliseconds}ms");
            }
        }

        [Fact]
        public async Task UploadMultiFilesWithProgressAsync()
        {
            var loggerFac = _serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.FtpLog");
            HashSet<int> codes = new HashSet<int>();

            var token = new CancellationToken();
            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                await ftp.Connect(token);

                IProgress<FtpProgress> progress = new
                    Progress<FtpProgress>(p =>
                    {
                        var code = (int)p.Progress;
                        if (codes.Add(code))
                        {
                            logger.LogInformation($"文件:{p.LocalPath},上传进度:{code}%，上传速度:{p.TransferSpeedToString()}");

                            if (100 == code) 
                            {
                                codes.Clear();
                            }
                        }
                    });

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // upload many files, skip if they already exist on server
                await ftp.UploadFiles(
                    new[] {
                        @"C:\core\Publish\FtpClientFile\ideas\cn_visio_professional_2013_x64_1138440.exe",
                        @"C:\core\Publish\FtpClientFile\ideas\dotnet-sdk-6.0.405-win-x64.exe",
                        @"C:\core\Publish\FtpClientFile\ideas\WeChatSetup.exe",
                    },
                    "/public_html01/20231220imgs/ideas/", 
                    FtpRemoteExists.Skip,createRemoteDir:true, progress: progress, token: token);

                stopwatch.Stop();

                logger.LogInformation($"上传文件耗时:{stopwatch.ElapsedMilliseconds}ms");
            }
        }

        [Fact]
        public async Task DownloadDirectoryWithRulesAsync()
        {
            var token = new CancellationToken();
            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                await ftp.Connect(token);

                // download only PDF files under 1 GB from a folder, by using the rule engine
                var rules = new List<FtpRule>{
                   new FtpFileExtensionRule(true, new List<string>{ "png" }),  // only allow PDF files
				   new FtpSizeRule(FtpOperator.LessThan, 1000000000)           // only allow files <1 GB
				};
                await ftp.DownloadDirectory(
                    @"C:\core\Publish\FtpClientFile\download\imgs",
                    @"/public_html01/20231220imgs",
                    FtpFolderSyncMode.Update, FtpLocalExists.Skip, FtpVerify.None, rules, token: token);

                // download all files from a folder, but skip the sub-directories named `.git`, `.svn`, `node_modules` etc
                FtpFolderNameRule.CommonBlacklistedFolders.Add("ideas");
                var rules2 = new List<FtpRule>{
                   new FtpFolderNameRule(false,FtpFolderNameRule.CommonBlacklistedFolders),
                };
                await ftp.DownloadDirectory(
                    @"C:\core\Publish\FtpClientFile\download\imgs",
                    @"/public_html01/20231220imgs",
                    FtpFolderSyncMode.Update, FtpLocalExists.Skip, FtpVerify.None, rules2, token: token);
            }
        }

        [Fact]
        public async Task DownloadFilesAsync()
        {
            var token = new CancellationToken();

            var loggerFac = _serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.FtpLog");
            HashSet<int> codes = new HashSet<int>();

            using (var ftp = new AsyncFtpClient("127.0.0.1", "ftp001", "aabb1212"))
            {
                await ftp.Connect(token);

                IProgress<FtpProgress> progress = new
                    Progress<FtpProgress>(p =>
                    {
                        var code = (int)p.Progress;
                        if (codes.Add(code))
                        {
                            logger.LogInformation($"文件:{p.RemotePath},下载进度:{code}%，下载速度:{p.TransferSpeedToString()}");

                            if (100 == code)
                            {
                                codes.Clear();
                            }
                        }
                    });

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // download many files, skip if they already exist on disk
                await ftp.DownloadFiles(@"C:\core\Publish\FtpClientFile\download\20231220imgs\",
                    new[] {
                        @"/public_html01/20231220imgs/ideas/cn_visio_professional_2013_x64_1138440.exe",
                        @"/public_html01/20231220imgs/ideas/dotnet-sdk-6.0.405-win-x64.exe",
                        @"/public_html01/20231220imgs/ideas/WeChatSetup.exe",
                    }, FtpLocalExists.Overwrite, progress:progress, token: token);

                stopwatch.Stop();

                logger.LogInformation($"下载文件耗时:{stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
