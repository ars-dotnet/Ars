using FastReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class FastReportTest
    {
        [Fact]
        public void Test1()
        {
            Report report = new Report();
            report.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/ArsTest.frx"));

            DataSet dataSet = new DataSet();
            DataTable dtpat = new DataTable();
            dtpat.Columns.Add("CustomerID", typeof(string));
            dtpat.Columns.Add("CompanyName", typeof(string));
            dtpat.Columns.Add("Country", typeof(string));
            dtpat.Columns.Add("Address", typeof(string));
            dtpat.Columns.Add("City", typeof(string));
            dtpat.Columns.Add("PostalCode", typeof(decimal));
            dtpat.Columns.Add("Barcode", typeof(string));

            dtpat.Rows.Add("11", "22", "33", "44", "55", 189.555m, "123456");
            //dtpat.Rows.Add("12", "22", "32", "42", "52", 122.555m, "1232");
            //dtpat.Rows.Add("13", "23", "33", "43", "53", 123.555m, "1233");
            //dtpat.Rows.Add("14", "24", "34", "44", "54", 124.555m, "1234");
            //dtpat.Rows.Add("15", "25", "35", "45", "55", 125.555m, "1235");
            //dtpat.Rows.Add("16", "26", "36", "46", "56", 126.555m, "1236");
            //dtpat.Rows.Add("17", "27", "37", "47", "57", 127.555m, "1237");
            dtpat.TableName = "Ars";
            dataSet.Tables.Add(dtpat);

            report.RegisterData(dataSet, "MyArs");
            //report.PrintSettings.Printer = "HP Smart Universal Printing";
            report.PrintSettings.ShowDialog = true;
            report.Print();
        }
    }
}
