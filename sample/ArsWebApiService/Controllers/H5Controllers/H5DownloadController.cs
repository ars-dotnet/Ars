using Ars.Commom.Tool.Extension;
using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Repository;
using Ars.Common.Tool;
using ArsWebApiService.Controllers.BaseControllers;
using ArsWebApiService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ArsWebApiService.Controllers.H5Controllers
{
    [Authorize("default")]
    public class H5DownloadController : ArsWebApiBaseController
    {
        [Autowired]
        public IRepository<AppVersion> _repo { get; set; }

        /// <summary>
        /// 下载app
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<FileStreamResult> DownloadApp(string version)
        {
            var data = await _repo.GetAll().FirstOrDefaultAsync(r => r.Version.Equals(version));
            Valid.ThrowException(null == data, "未获取到当前版本app");

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"wwwroot/AppDownload/{version}.apk");
            Valid.ThrowException(!System.IO.File.Exists(path), "文件不存在");
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet-stream", $"{version}.apk");
        }

        /// <summary>
        /// 获取最新版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ArsOutput<AppVersion>> GetAppLastVersion() 
        {
            var data = await _repo.GetAll().OrderByDescending(r => r.Id).FirstOrDefaultAsync();

            return new ArsOutput<AppVersion>(data);
        }

        /// <summary>
        /// 上传app
        /// </summary>
        /// <param name="file">app文件</param>
        /// <param name="version">版本号，格式如v1.0.0, v1.1.0</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ArsOutput> UploadApp([Required] IFormFile file, [Required] string version)
        {
            ArsOutput arsOutput = new ArsOutput();

            if (!file.FileName.EndsWith("apk"))
            {
                arsOutput.Code = 1;
                arsOutput.Message = "仅支持上传apk文件";
                goto over;
            }
            if (!version.StartsWith("v"))
            {
                arsOutput.Code = 1;
                arsOutput.Message = "版本号必须以v开头";
                goto over;
            }

            bool hasone = await _repo.GetAll().AnyAsync(r => r.Version.Equals(version));
            if (hasone)
            {
                arsOutput.Code = 1;
                arsOutput.Message = "该版本已存在";
                goto over;
            }

            string? lastversion = (await _repo.GetAll().OrderByDescending(r => r.Id).FirstOrDefaultAsync())?.Version;
            if (null != lastversion && string.Compare(version, lastversion) < 0)
            {
                arsOutput.Code = 1;
                arsOutput.Message = $"传入的版本号必须大于最新版本{lastversion}";
                goto over;
            }

            //保存文件
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/AppDownload");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            string filepath = version + ".apk";
            string allpath = string.Concat(root,"/", filepath);
            using (FileStream fileStream = new FileStream(allpath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            //保存记录
            await _repo.InsertAsync(new AppVersion
            {
                Version = version,
                Path = string.Concat("/apps/download/", filepath)
            });

            over:
            return arsOutput;
        }
    }
}
