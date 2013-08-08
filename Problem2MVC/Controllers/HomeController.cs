using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Problem2MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            if (TempData["Mensagem"] != null)
            {
                ViewBag.Message = TempData["Mensagem"].ToString();
            }

            return View();
        }
    }
}
