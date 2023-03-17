using Ars.Commom.Tool.Extension;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Tools
{
    public static class ExcelTool
    {
        public static FileStreamResult ExportExcel(ExportExcelInput input) 
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            ICellStyle headCellStyle = GetNPOITitleStyle(workbook);
            
            //渲染表头
            int index = 0;
            IRow rhead = sheet.CreateRow(index);
            rhead.Height = 100 * 5;

            List<int> columnMaxWidthList = new List<int>(input.Column.Count);//每列的最大列宽
            int col = 0;
            ICell cell;
            foreach (var column in input.Column) 
            {
                cell = rhead.CreateCell(col);
                cell.CellStyle = headCellStyle;
                cell.SetCellValue(column.Value);
                //以列头最为初始值
                columnMaxWidthList.Add(System.Text.Encoding.Default.GetBytes(column.Value).Length + 1);

                col++;
            }

            //第一个参数表示要冻结的列数；
            //第二个参数表示要冻结的行数；
            //第三个参数表示右边区域可见的首列序号，从0开始计算；
            //第四个参数表示下边区域可见的首行序号，从1开始计算；
            sheet.CreateFreezePane(0, index+1, 0, index+1);
            sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, input.Column.Count - 1)); //首行筛选

            //渲染数据
            IRow rbody;
            foreach (var item in input.List) 
            {
                col = 0;
                index++;
                rbody = sheet.CreateRow(index);

                string value;
                foreach (var c in input.Column) 
                {
                    cell = rbody.CreateCell(col);
                    value = input.ItemType.GetProperty(c.Key)!.GetValue(item)!.ToString()!;
                    cell.SetCellValue(value);

                    int length = System.Text.Encoding.Default.GetBytes(value).Length + 1;
                    if (length > columnMaxWidthList[col]) { columnMaxWidthList[col] = length; }

                    col++;
                }
            }

            col = -1;
            while (++col < columnMaxWidthList.Count)
            {
                rhead.Sheet.SetColumnWidth(col, columnMaxWidthList[col] * 256);
            }

            input.ExportFileName = 
                input.ExportFileName.IsNullOrEmpty()
                ? DateTime.Now.ToString("yyyyMMddHHmmss") 
                : input.ExportFileName;
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;
            return new FileStreamResult(ms, "application/octet-stream") { FileDownloadName = string.Concat(input.ExportFileName, ".xls") };
        }

        public static ICellStyle GetNPOITitleStyle(XSSFWorkbook workbook) 
        {
            //定义表头样式
            ICellStyle headCellStyle = workbook.CreateCellStyle();
            IFont fontStyle = workbook.CreateFont();
            fontStyle.FontHeightInPoints = 10;//字体大小(字号方式)
            fontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;//白色字体
            fontStyle.IsBold = true;
            headCellStyle.SetFont(fontStyle);
            headCellStyle.FillPattern = FillPattern.SolidForeground;//必须开启这个才有背景色 且背景色不叫背景色 叫前景色 
            headCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Indigo.Index;//靛蓝背景
            headCellStyle.VerticalAlignment = VerticalAlignment.Center;//上下居中
            headCellStyle.Alignment = HorizontalAlignment.Center;//左右居中
            headCellStyle.WrapText = true;
            return headCellStyle;
        }
    }
}
