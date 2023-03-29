using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Configs;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public class ArsExcelActionFilter : IAsyncActionFilter
    {
        private readonly IExcelStorage _excelStorage;
        private readonly IOptions<IArsBasicConfiguration> _basicConfig;
        private readonly IOptions<IArsUploadExcelConfiguration> _uploadConfig;
        public ArsExcelActionFilter(IExcelStorage excelStorage,
            IOptions<IArsBasicConfiguration> basicConfig,
            IOptions<IArsUploadExcelConfiguration> uploadConfig)
        {
            _excelStorage = excelStorage;
            _basicConfig = basicConfig;
            _uploadConfig = uploadConfig;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.Any(r => typeof(IExcelDataValidation).IsAssignableFrom(r.Value?.GetType())))
            {
                var idatavalidation = context.ActionArguments
                   .FirstOrDefault(r => typeof(IExcelDataValidation).IsAssignableFrom(r.Value?.GetType()))
                   .Value
                   .As<IExcelDataValidation>()!;
                var itemType = idatavalidation
                    .GetType()
                    .GetInterfaces()
                    .FirstOrDefault(r => typeof(IExcelData<>).IsAssignableGenericFrom(r))!
                    .GetGenericArguments()[0];
                ExcelSaveScheme savescheme = null;
                if (!idatavalidation.Validation())
                {
                    savescheme = new ExcelSaveScheme
                    {
                        SavePath = Path.Combine(_basicConfig.Value.Root, _uploadConfig.Value.UploadRoot),
                        Column = itemType.GetExcelMappingAttributes().Select(r => new ExcelExportColumn 
                        {
                            Field = r.Property,
                            Column = r.Column,
                            IsRequired = r.IsRequired
                        }),
                        List = idatavalidation.GetType().GetProperty(nameof(IExcelData<IExcelModel>.ExcelModels))!.GetValue(idatavalidation).As<IEnumerable>()!,
                        ItemType = itemType,
                        SlidingExpireTime = _uploadConfig.Value.SlidingExpireTime
                    };
                    //保存新的excel
                    await SaveExcel(savescheme);
                }

                var result = await next();
                if (null != result.Exception || result.ExceptionHandled)
                {
                    if (null == savescheme)
                    {
                        savescheme = new ExcelSaveScheme()
                        {
                            SavePath = Path.Combine(_basicConfig.Value.Root, _uploadConfig.Value.UploadRoot),
                            Column = itemType.GetExcelMappingAttributes().Select(r => new ExcelExportColumn
                            {
                                Field = r.Property,
                                Column = r.Column,
                                IsRequired = r.IsRequired
                            }),
                            ItemType = itemType,
                            SlidingExpireTime = _uploadConfig.Value.SlidingExpireTime
                        };
                    }

                    var list = idatavalidation.GetType().GetProperty(nameof(IExcelData<IExcelModel>.ExcelModels))!.GetValue(idatavalidation).As<IEnumerable<IExcelModel>>()!;
                    if (result.Exception is ArsExcelSaveErrOnlyException ex)
                    {
                        list = list.Where(r => r.IsErr && r.FieldErrMsg.HasValue());
                        savescheme.List = list;
                        await SaveExcel(savescheme, ex.Message);
                    }
                    else if (result.Exception is ArsExcelSaveAllException ex1)
                    {
                        savescheme.List = list;
                        await SaveExcel(savescheme, ex1.Message);
                    }
                }
            }
            else 
            {
                await next();
            }
        }

        private async Task<bool> SaveExcel(ExcelSaveScheme input,string message = null) 
        {
            var save = await _excelStorage.SaveExcel(input);
            if (save)
            {
                throw new ArsExcelException(
                    string.Concat(
                        _basicConfig.Value.ApplicationUrl,
                        $"/{_uploadConfig.Value.RequestPath}",
                        $"/{input.ExportFileName}.xls"),
                        message.IsNullOrEmpty() 
                        ? "数据行校验失败" 
                        : message);
            }
            else
            {
                throw new ArsExcelException("保存校验错误的excel失败");
            }
        }
    }
}
