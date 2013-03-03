using EasyReading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyReading.Controllers
{
    public class HomeController : Controller
    {

        private BookDbContext db = new BookDbContext();

        public ActionResult Index()
        {
            return View(db.BookGroups.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
