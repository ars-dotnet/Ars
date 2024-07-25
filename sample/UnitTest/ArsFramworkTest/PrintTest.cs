using NUnit.Framework;
using System.Data;
using System.IO;
using System;
using FastReport;

namespace Ars.Framwork.PrintTest
{
    public class PrintTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// 自动打印
        /// </summary>
        [Test]
        public void TestPrintAuto()
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
            dtpat.Rows.Add("12", "22", "32", "42", "52", 122.555m, "1232");

            dtpat.TableName = "Ars";
            dataSet.Tables.Add(dtpat);

            report.RegisterData(dataSet, "MyArs");

            report.PrintSettings.ShowDialog = false;

            report.PrintSettings.Printer = "HP LaserJet Pro MFP M126nw";

            report.Print();

            report.Dispose();
        }

        /// <summary>
        /// 手动打印
        /// </summary>
        [Test]
        public void TestPrintManual()
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
            dtpat.Rows.Add("12", "22", "32", "42", "52", 122.555m, "1232");

            dtpat.TableName = "Ars";
            dataSet.Tables.Add(dtpat);

            report.RegisterData(dataSet, "MyArs");

            report.Prepare();

            report.PrintSettings.ShowDialog = true;

            report.Print();

            report.Dispose();
        }
    }
}