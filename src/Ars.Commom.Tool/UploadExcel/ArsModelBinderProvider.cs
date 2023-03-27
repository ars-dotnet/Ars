using Ars.Common.Tool.Extension;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    internal class ArsModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var metadata = context.Metadata;
            if (null != metadata.ContainerType && 
                typeof(IExcelData<>).IsAssignableGenericFrom(metadata.ContainerType) && 
                metadata.Name == nameof(IExcelData<IExcelModel>.ExcelModels)) 
            {
                return context.Services.GetService<ArsModelBinder>();
            }

            return null;
        }
    }
}
