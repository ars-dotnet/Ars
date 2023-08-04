using Ars.Commom.Tool.Extension;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MyFastReport.Models;
using System.Data;
using System.Diagnostics;

namespace MyFastReport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static DataSet _DefaultData = null;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{reportIndex:int?}")]
        public IActionResult Index(int? reportIndex = 0)
        {
            var model = new HomeModel()
            {
                WebReport = new WebReport(),
                ReportsList = new[]
                {
                    "Simple List",
                    "Labels",
                    "Master-Detail",
                    "Badges",
                    "Interactive Report, 2-in-1",
                    "Hyperlinks, Bookmarks",
                    "Outline",
                    "Complex (Hyperlinks, Outline, TOC)",
                    "Drill-Down Groups",
                    "Polygon",
                    "Barcode",
                    "ArsTest",
                    "ArsTestPrint"
                },
            };

            var reportToLoad = model.ReportsList[0];
            if (reportIndex >= 0 && reportIndex < model.ReportsList.Length)
                reportToLoad = model.ReportsList[reportIndex.Value];
            model.WebReport.Report.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Reports/{reportToLoad}.frx"));

            if (reportToLoad.StartsWith("ArsTest"))
            {
                IEnumerable<ArsModel> arsModels = new List<ArsModel>
                {
                    new ArsModel
                    {
                        CustomerID = "1",
                        CompanyName = "2",
                        Country = "3",
                        Address = "4",
                        City = "5",
                        PostalCode = 127.567m,
                        Barcode = "7"
                    },
                    new ArsModel
                    {
                        CustomerID = "11",
                        CompanyName = "21",
                        Country = "31",
                        Address = "41",
                        City = "51",
                        PostalCode = 1271.567m,
                        Barcode = "71"
                    },
                    new ArsModel
                    {
                        CustomerID = "111",
                        CompanyName = "211",
                        Country = "311",
                        Address = "411",
                        City = "511",
                        PostalCode = 12711.567m,
                        Barcode = "711"
                    },
                    new ArsModel
                    {
                        CustomerID = "1111",
                        CompanyName = "2111",
                        Country = "3111",
                        Address = "4111",
                        City = "5111",
                        PostalCode = 127111.567m,
                        Barcode = "7111"
                    },
                    new ArsModel
                    {
                        CustomerID = "1112",
                        CompanyName = "2112",
                        Country = "3112",
                        Address = "4112",
                        City = "5112",
                        PostalCode = 127112.567m,
                        Barcode = "7112"
                    },
                    new ArsModel
                    {
                        CustomerID = "11123",
                        CompanyName = "21123",
                        Country = "31123",
                        Address = "41123",
                        City = "51123",
                        PostalCode = 1271123.567m,
                        Barcode = "71123"
                    },
                    new ArsModel
                    {
                        CustomerID = "11124",
                        CompanyName = "21124",
                        Country = "31124",
                        Address = "41124",
                        City = "51124",
                        PostalCode = 1271124.567m,
                        Barcode = "71124"
                    },
                    new ArsModel
                    {
                        CustomerID = "111245",
                        CompanyName = "211245",
                        Country = "311245",
                        Address = "411245",
                        City = "511245",
                        PostalCode = 12711245.567m,
                        Barcode = "711245"
                    },
                    new ArsModel
                    {
                        CustomerID = "111246",
                        CompanyName = "211246",
                        Country = "311246",
                        Address = "411246",
                        City = "511246",
                        PostalCode = 12711246.567m,
                        Barcode = "711246"
                    }
                };

                model.WebReport.Report.RegisterData(arsModels.ToDataSet("Ars"), "MyArs");

                if (reportToLoad.EndsWith("Print"))
                {
                    
                }
            }
            else
            {
                if (null == _DefaultData) 
                {
                    _DefaultData = new DataSet();
                    _DefaultData.ReadXml(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports/nwind.xml"));
                }
                model.WebReport.Report.RegisterData(_DefaultData, "NorthWind");
            }


            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}