using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Configs;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    internal class ArsModelBinder : IModelBinder
    {
        private readonly IExcelResolve _excelResolve;
        private readonly IExcelStorage _excelStorage;
        private readonly IOptions<IArsBasicConfiguration> _basicConfig;
        private readonly IOptions<IArsUploadExcelConfiguration> _uploadConfig;
        public ArsModelBinder(IExcelResolve excelResolve, IExcelStorage excelStorage,
            IOptions<IArsBasicConfiguration> basicConfig,
            IOptions<IArsUploadExcelConfiguration> uploadConfig)
        {
            _excelResolve = excelResolve;
            _excelStorage = excelStorage;
            _basicConfig = basicConfig;
            _uploadConfig = uploadConfig;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelName.Equals(nameof(IExcelData<IExcelModel>.ExcelModels))) 
            {
                Valid.ThrowException(_basicConfig.Value.ApplicationUrl.IsNullOrEmpty(), "ApplicationUrl未配置");

                IFormFile file;
                if (bindingContext.HttpContext.Request.Form.Files.Count > 0)
                    file = bindingContext.HttpContext.Request.Form.Files[0];
                else
                    throw new ArsExcelException("请选择上传的Excel文件");
                bindingContext.HttpContext.Request.Form.TryGetValue(
                    nameof(IExcelData<IExcelModel>.ExcelColumnFromRow), 
                    out StringValues ExcelColumnFromRow);
                _excelResolve.ExcelColumnFromRow = int.Parse(ExcelColumnFromRow);

                using Stream stream = file.OpenReadStream();
                var result = _excelResolve.GetType()
                    .GetMethod(nameof(IExcelResolve.ToList))!
                    .InvokeGeneric(
                        _excelResolve,
                        new object[] { stream },
                        bindingContext.ModelType.GetGenericArguments()[0])
                    .As<ExcelResolveResult>();

                if (!_excelResolve.Successed) //校验失败
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    if (_excelResolve.ValidCellFailed)
                    {
                        var savescheme = new ExcelSaveScheme
                        {
                            SavePath = Path.Combine(_basicConfig.Value.Root, _uploadConfig.Value.UploadRoot),
                            Column = result!.Column,
                            List = result.List,
                            ItemType = result.ItemType,
                            SlidingExpireTime = _uploadConfig.Value.SlidingExpireTime
                        };
                        //保存新的excel
                        var save = await _excelStorage.ExcelSave(savescheme);
                        if (save)
                        {
                            throw new ArsExcelException(
                                string.Concat(
                                    _basicConfig.Value.ApplicationUrl, 
                                    $"/{_uploadConfig.Value.RequestPath}",
                                    $"/{savescheme.ExportFileName}.xls"),
                                "数据行校验失败");
                        }
                        else 
                        {
                            throw new ArsExcelException("保存错误提示的excel失败");
                        }
                    }
                    else 
                    {
                        throw new ArsExcelException(_excelResolve.ErrorMsg!);
                    }
                }
                else //校验成功
                {
                    bindingContext.Result = ModelBindingResult.Success(result!.List);
                }
            }

            return;
        }
    }
}
