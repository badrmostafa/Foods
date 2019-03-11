using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Foods.Models.Classes;

namespace Foods.Controllers
{
    public class HomeController : Controller
    {
        private FoodContext db = new FoodContext();
        public ActionResult Index()
        {
            ViewBag.food = db.Foods.First();
            /////////////////////////////////////
            ViewBag.topfood = db.TopFoods.First();
            ViewBag.top = db.TopFoods.ToList();
            /////////////////////////////////////
            ViewBag.menu = db.Menus.First();
            ViewBag.m = db.Menus.ToList();
            ////////////////////////////////////
            ViewBag.gallery = db.Galleries.First();
            ViewBag.g = db.Galleries.ToList();
            ///////////////////////////////////
            ViewBag.client = db.Clients.ToList();
            ////////////////////////////////////
            ViewBag.reservation = db.Reservations.First();





            return View();
        }

        public ActionResult About()
        {
            ViewBag.about = db.Abouts.First();
            //////////////////////////////////
            ViewBag.client = db.Clients.ToList();

            return View();
        }
        public ActionResult Menu()
        {
            ViewBag.menu = db.Menus.First();
            ViewBag.m = db.Menus.ToList();
            return View();
        }
        public ActionResult Gallery()
        {

            ViewBag.gallery = db.Galleries.First();
            ViewBag.g = db.Galleries.ToList();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.reservation = db.Reservations.First();

            return View();
        }
    }
}