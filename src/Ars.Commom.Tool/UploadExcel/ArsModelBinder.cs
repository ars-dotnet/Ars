using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public ArsModelBinder(IExcelResolve excelResolve, IExcelStorage excelStorage)
        {
            _excelResolve = excelResolve;
            _excelStorage = excelStorage;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.Name.Equals(nameof(IExcelData<IExcelModel>.ExcelModels))) 
            {
                IFormFile file;
                if (bindingContext.HttpContext.Request.Form.Files.Count > 0)
                    file = bindingContext.HttpContext.Request.Form.Files[0];
                else
                    throw new ArsExcelException("请选择上传的Excel文件");

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
                        //保存新的excel
                        await _excelStorage.ExcelSave(new ExcelSaveScheme
                        {
                            SavePath = "",
                            Column = result!.Column,
                            List = result.List,
                            ItemType = result.ItemType
                        });
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
