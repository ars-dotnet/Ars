using Ars.Commom.Tool.Extension;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Tools
{
    public static class ExcelTool
    {
        public static MemoryStream GetStream(ExcelExportScheme input) 
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            int index = 0;
            ICell cell;

            //渲染title
            if (input.Title.IsNotNullOrEmpty())
            {
                IRow rhead = sheet.CreateRow(index);
                rhead.Height = 100 * 5;

                cell = rhead.CreateCell(index);
                cell.CellStyle = GetNPOITitleStyle(workbook);
                cell.SetCellValue(input.Title);
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(index, index, 0, input.Column.Count - 1));

                index++;
            }

            int col = 0;
            //渲染header
            if (input.Header.HasValue())
            {
                IRow rhead = sheet.CreateRow(index);
                rhead.Height = 100 * 5;

                foreach (var header in input.Header)
                {
                    cell = rhead.CreateCell(col);
                    col += header.Value.Item2 - header.Value.Item1 + 1;
                    cell.CellStyle = GetNPOIHeaderStyle(workbook);
                    cell.SetCellValue(header.Key);
                    if (header.Value.Item1 == header.Value.Item2)
                        continue;

                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(index, index, header.Value.Item1, header.Value.Item2));
                }

                index++;
            }

            //渲染行名称
            col = 0;
            IRow columnR = sheet.CreateRow(index);
            List<int> columnMaxWidthList = new List<int>(input.Column.Count);//每列的最大列宽
            var columnNameStyle = GetNPOIColumnStyle(workbook);
            foreach (var column in input.Column)
            {
                cell = columnR.CreateCell(col);
                cell.CellStyle = columnNameStyle;
                cell.SetCellValue(column.Value);
                //以列头最为初始值
                columnMaxWidthList.Add(System.Text.Encoding.Default.GetBytes(column.Value).Length + 1);

                col++;
            }

            //第一个参数表示要冻结的列数；
            //第二个参数表示要冻结的行数；
            //第三个参数表示右边区域可见的首列序号，从0开始计算；
            //第四个参数表示下边区域可见的首行序号，从1开始计算；
            sheet.CreateFreezePane(0, index + 1, 0, index + 1);
            sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(index, index, 0, input.Column.Count - 1)); //首行筛选

            //渲染数据
            IRow rbody;
            foreach (var item in input.List)
            {
                col = 0;
                index++;
                rbody = sheet.CreateRow(index);

                string? value;
                foreach (var c in input.Column)
                {
                    cell = rbody.CreateCell(col);
                    value = ConvertTool.ToString(input.ItemType.GetProperty(c.Key)!.GetValue(item));
                    cell.SetCellValue(value);

                    int length = value.IsNullOrEmpty() 
                        ? 0 
                        : System.Text.Encoding.Default.GetBytes(value).Length + 1;
                    if (length > columnMaxWidthList[col]) { columnMaxWidthList[col] = length; }

                    col++;
                }
            }

            //设置行宽
            col = -1;
            while (++col < columnMaxWidthList.Count)
            {
                columnR.Sheet.SetColumnWidth(col, columnMaxWidthList[col] * 256);
            }
            
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;

            return ms;
        }

        public static async Task<bool> SaveExcel(ExcelSaveScheme input) 
        {
            if (Directory.Exists(input.SavePath))
            {
                var fiels = Directory.GetFiles(input.SavePath).Where(r => File.GetLastWriteTime(r) + input.SlidingExpireTime < DateTime.Now);
                foreach (var item in fiels)
                {
                    File.Delete(item);
                }
            }
            else 
            {
                Directory.CreateDirectory(input.SavePath);
            }

            input.ExportFileName = 
                input.ExportFileName.IsNullOrEmpty() 
                ? DateTime.Now.ToString("yyyyMMddHHmmss") 
                : input.ExportFileName;
            using var ms = GetStream(input);
            using var fileStream = new FileStream(string.Concat(input.SavePath,"/",input.ExportFileName,".xls"),FileMode.Create);
            await ms.CopyToAsync(fileStream);

            return true;
        }

        public static FileStreamResult ExportExcel(ExcelExportScheme input) 
        {
            var ms = GetStream(input);

            input.ExportFileName =
                input.ExportFileName.IsNullOrEmpty()
                ? DateTime.Now.ToString("yyyyMMddHHmmss")
                : input.ExportFileName;
            return new FileStreamResult(ms, "application/octet-stream") { FileDownloadName = string.Concat(input.ExportFileName, ".xls") };
        }

        /// <summary>
        /// 定义title样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static ICellStyle GetNPOITitleStyle(XSSFWorkbook workbook) 
        {
            ICellStyle titleCellStyle = workbook.CreateCellStyle();
            IFont fontStyle = workbook.CreateFont();
            fontStyle.FontHeightInPoints = 22;
            fontStyle.IsBold = true;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;//上下居中
            titleCellStyle.Alignment = HorizontalAlignment.Center;//左右居中
            titleCellStyle.SetFont(fontStyle);
            return titleCellStyle;
        }

        /// <summary>
        /// 定义header样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static ICellStyle GetNPOIHeaderStyle(XSSFWorkbook workbook) 
        {
            ICellStyle headCellStyle = workbook.CreateCellStyle();
            headCellStyle.VerticalAlignment = VerticalAlignment.Center;//上下居中
            headCellStyle.Alignment = HorizontalAlignment.Center;//左右居中

            return headCellStyle;
        }

        /// <summary>
        /// 定义行名称样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static ICellStyle GetNPOIColumnStyle(XSSFWorkbook workbook) 
        {
            ICellStyle columnNameCellStyle = workbook.CreateCellStyle();
            IFont fontStyle = workbook.CreateFont();
            fontStyle.FontHeightInPoints = 10;//字体大小(字号方式)
            fontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;//白色字体
            fontStyle.IsBold = true;
            columnNameCellStyle.SetFont(fontStyle);
            columnNameCellStyle.FillPattern = FillPattern.SolidForeground;//必须开启这个才有背景色 且背景色不叫背景色 叫前景色 
            columnNameCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Indigo.Index;//靛蓝背景
            columnNameCellStyle.VerticalAlignment = VerticalAlignment.Center;//上下居中
            columnNameCellStyle.Alignment = HorizontalAlignment.Center;//左右居中
            columnNameCellStyle.WrapText = true;
            return columnNameCellStyle;
        }
    }
}
