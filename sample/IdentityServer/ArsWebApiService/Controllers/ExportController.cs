using Ars.Common.Tool.Export;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class ExportController : Controller
    {
        private readonly IExportManager _exportManager;
        public ExportController(IExportManager exportManager)
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
    }
}
