using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ASPMVC_AgroML.Models;

namespace ASPMVC_AgroML.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Forecasting()
        {
            var forecast = new Forecast()
            {
                BurRz = 0,
                ChernBakt = 0,
                KarlGol = 0,
                KornGnil = 0,
                None = 0,
                PilnGol = 0
            };
            Chart theChart = new Chart(width: 300, height: 200);

            theChart.ToWebImage().Save("~/Content/chart", "PNG");
            return View(forecast);
        }

        [HttpPost]
        public ActionResult Forecasting(Meteo meteo)
        {
            var forecast = Tools.MeteoToForecast(meteo);

            var x = new List<string>()
            {
                "Нет риска болезни",
                "Бурая ржавчина",
                "Пыльная головня",
                "Карликовая головня",
                "Черный бактериоз пшеницы",
                "Офиоболезная корневая гниль"
            };

            var y = new List<double>()
            {
                forecast.None,
                forecast.BurRz,
                forecast.PilnGol,
                forecast.KarlGol,
                forecast.ChernBakt,
                forecast.KornGnil

            };

            Chart theChart = new Chart(width: 300, height: 200)
                .AddSeries(
                    chartType: "bar",
                    legend: "Income",
                    xValue: x,
                    yValues: y);


            
            theChart.ToWebImage().Save("~/Content/chart", "PNG");


            return View(forecast);
        }


        

        public ActionResult Watson()
        {
            return View();
        }

        public ActionResult Bluemix()
        {
            return View();
        }

        public ActionResult DataScience()
        {
            return View();
        }
    }
}