using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Ars.Common.Tool.Extension;
using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.UploadExcel.Validation;
using Ars.Common.Tool.Tools;

namespace Ars.Common.Tool.UploadExcel
{
    internal class ExcelResolve : IExcelResolve
    {
        public bool Successed { get; private set; }

        public string? ErrorMsg { get; private set; }

        public bool ValidCellFailed { get; private set; }

        public int MaxRowCount { get; set; }

        public int ExcelColumnFromRow { get; set; }

        public ExcelResolveResult? ToList<T>(Stream stream) where T : IExcelModel, new()
        {
            ExcelResolveResult result = new ExcelResolveResult();
            IList<T>? list = null;
            try
            {
                //条数校验
                if (!CheckCount(stream, out Cells cells)) 
                {
                    goto over;
                }

                //返回DataTable
                var dataTable = GetDataTable(cells);

                //行标题校验
                if (!CheckColumn<T>(dataTable,out var mappingAttributes))
                {
                    goto over;
                }

                //校验行明细,组装list
                if (!CheckAndReturnRows(dataTable, mappingAttributes, out list))
                {
                    ValidCellFailed = true;
                    Successed = false;
                    ErrorMsg = "行数据校验失败";
                }
                else 
                {
                    Successed = true;
                }

                result.Column = mappingAttributes.ToDictionary(r => r.Property,t => t.Column);
                result.ItemType = typeof(T);
                result.List = list;
            }
            catch (Exception e) 
            {
                Successed = false;
                ErrorMsg = e.Message;
            }

            over:

            return result;
        }

        public virtual bool CheckCount(Stream stream,out Cells cells) 
        {
            Workbook workbook = new Workbook(stream);
            cells = workbook.Worksheets[0].Cells;
            int maxDataRow = cells.MaxDataRow;
            if (MaxRowCount > 0 && MaxRowCount < maxDataRow) 
            {
                Successed = false;
                ErrorMsg = $"实际数量{maxDataRow}超出最大条数{MaxRowCount}";

                return false;
            }

            return true;
        }

        public virtual DataTable GetDataTable(Cells cells)
        {
            DataTable dataTable = new DataTable();

            for (var i = 0; i < cells.MaxDataColumn + 1; i++)
            {
                dataTable.Columns.Add(new DataColumn(cells[ExcelColumnFromRow,i].StringValue,typeof(string)));
            }

            DataRow dataRow;
            for (var h = ExcelColumnFromRow + 1; h < cells.MaxDataRow + 1;h++) //行
            {
                dataRow = dataTable.NewRow();//每行数据一个row

                for (int j = 0; j < cells.MaxDataColumn + 1; j++) //列
                {
                    dataRow[j] = cells[h,j].Value;
                }

                dataTable.Rows.Add(dataRow);
            }
            
            return dataTable;
        }

        public virtual bool CheckColumn<T>(DataTable dataTable,out ExcelMappingAttribute[] mappingAttributes)
             where T : IExcelModel, new()
        {
            bool check = false;
            mappingAttributes = typeof(T).GetExcelMappingAttributes().ToArray();
            if (mappingAttributes.HasNotValue()) 
            {
                Successed = false;
                ErrorMsg = $"实体{typeof(T).Name}不存在property标记有ExcelMappingAttribute特性";

                goto over;
            }

            for (int i = 0;i < mappingAttributes.Count();i++) 
            {
                if (null == dataTable.Columns[mappingAttributes[i].Column] && mappingAttributes[i].ReadOrWrite == ReadOrWrite.Read) 
                {
                    Successed = false;
                    ErrorMsg = $"上传的excel文件不存在名称为{mappingAttributes[i].Column}的列";

                    goto over;
                }
            }

            check = true;
         over:
            return check;
        }

        public virtual bool CheckAndReturnRows<T>(DataTable dataTable, ExcelMappingAttribute[] excelMappingAttributes,out IList<T> list)
            where T : IExcelModel, new()
        {
            var iexcelValidation = typeof(T).GetExcelValidationAttributes();
            list = new List<T>();
            T t;
            IEnumerable<IExcelValidation>? iValidations;
            foreach (DataRow row in dataTable.Rows) //每一行明细
            {
                t = Activator.CreateInstance<T>();
                foreach (var attr in excelMappingAttributes.Where(r => r.ReadOrWrite == ReadOrWrite.Read)) //每一个字段
                {
                    var value = row[attr.Column];
                    if (iexcelValidation.TryGetValue(attr.Property,out iValidations)) 
                    {
                        foreach (var valid in iValidations)
                        {
                            bool? iserr = null;
                            StringBuilder? stringBuilder = null;
                            if (!valid.Validation(attr.Property, value)) 
                            {
                                iserr ??= true;
                                stringBuilder ??= new StringBuilder();
                                stringBuilder.Append(valid.ErrorMsg).Append("\r\n");
                            }
                            if (iserr ?? false) 
                            {
                                t.IsErr = true;
                                t.FieldErrMsg ??= new Dictionary<string, string>();
                                t.FieldErrMsg.Add(attr.Property, stringBuilder!.ToString());
                            }
                        }
                    }

                    if (ConvertTool.TryChangeType(value, attr.PropertyType, out object? newvalue))
                    {
                        t.GetType().GetProperty(attr.Property)!.SetValue(t, newvalue);
                    }
                    else 
                    {
                        t.IsErr = true;
                        t.FieldErrMsg ??= new Dictionary<string, string>();
                        t.FieldErrMsg.Add(attr.Property, $"数据{value}转化类型为{attr.PropertyType.Name}失败");
                    }
                }

                list.Add(t);
            }

            return !list.Any(r => r.IsErr);
        }
    }
}
