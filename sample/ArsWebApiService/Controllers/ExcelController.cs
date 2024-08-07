﻿using Ars.Common.Core.Configs;
using Ars.Common.Core.Excels.ExportExcel;
using Ars.Common.Core.Excels.UploadExcel;
using ArsWebApiService.Controllers.BaseControllers;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// excel test controller
    /// </summary>
    public class ExcelController : ArsWebApiBaseController
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// 上传excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UplaodByGet([FromForm] GetPageInput input)
        {
            return Ok("ok");
        }

        /// <summary>
        /// 上传excel - 测试抛错
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public Task UplaodWithErr([FromForm] UploadInput input) 
        {
            var a = input.ExcelModels.FirstOrDefault()!;
            a.IsErr = true;
            a.FieldErrMsg = new Dictionary<string,string>{ { "托盘号", "托盘错误" } };
            throw new ArsExcelSaveErrOnlyException("错误");
        }
    }
}
