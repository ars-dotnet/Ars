using Ars.Common.Tool.Export;
using Ars.Common.Tool.UploadExcel;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class ExcelController : Controller
    {
        private readonly IExportManager _exportManager;
        public ExcelController(IExportManager exportManager)
        {
            _exportManager = exportManager;
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<FileStreamResult> Export([FromBody]ExportExcelInput input) 
        {
            return _exportManager.GetExcel(input);
        }

        /// <summary>
        /// 上传excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public Task Uplaod([FromForm] UploadInput input) 
        {
            var a = input.ExcelModels.FirstOrDefault();
            a.IsErr = true;
            a.FieldErrMsg = new Dictionary<string,string>{ { "Barcode", "托盘错误" } };
            throw new ArsExcelSaveErrOnlyException("错误");

            return Task.CompletedTask;
        }
    }
}
