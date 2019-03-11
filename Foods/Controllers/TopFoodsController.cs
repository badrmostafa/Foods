using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data;
using System.Net;
using Foods.Models.Classes;
using PagedList;

namespace Foods.Controllers
{
    public class TopFoodsController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: TopFoods
        public ActionResult Index(string sort,string search,string filter,int? page)
        {
            ViewBag.sort = sort;
            ViewBag.Head = string.IsNullOrEmpty(sort) ? "head_desc" : "";
            if (search!=null)
            {
                page = 1;
            }
            else
            {
                search = filter;
            }
            ViewBag.filter = search;
            var topfoods = from t in db.TopFoods select t;
            if (!string.IsNullOrEmpty(search))
            {
                topfoods = topfoods.Where(t => t.Head.ToUpper().Contains(search.ToUpper()) ||
                  t.Head1.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    topfoods = topfoods.OrderByDescending(t => t.Head);
                    break;
                default:
                    topfoods = topfoods.OrderBy(t => t.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 3;
            return View(topfoods.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(TopFood topFood)
        {
            if (ModelState.IsValid)
            {
                db.TopFoods.Add(topFood);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(topFood);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
            TopFood topFood = db.TopFoods.Find(id);
            if (topFood==null)
            {
                return HttpNotFound();
            }
            return View(topFood);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
            TopFood topFood = db.TopFoods.Find(id);
            if (topFood == null)
            {
                return HttpNotFound();
            }
            return View(topFood);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(TopFood topFood)
        {

            try
            {
                db.Entry(topFood).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (TopFood)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The topfood was deleted by another user.");
                }
                else
                {
                    var databaseValues = (TopFood)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Image != clientValues.Image)
                        ModelState.AddModelError("Image", "Current Value:" + databaseValues.Image);
                            if (databaseValues.Head1 != clientValues.Head1)
                        ModelState.AddModelError("Head1", "Current Value:" + databaseValues.Head1);
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Description", "Current Value:" + databaseValues.Description);
                    if (databaseValues.Price != clientValues.Price)
                        ModelState.AddModelError("Price", "Current Value:" + databaseValues.Price);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit operation click the save button again,otherwise click the back to list hyperlink.");
                    topFood.RowVersion = databaseValues.RowVersion;
                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            return View(topFood);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
            TopFood topFood = db.TopFoods.Find(id);
            if (topFood == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (topFood==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        +" Click the back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to delete click the delete button again,otherwise click the back to list hyperlink.";
                }
            }
            return View(topFood);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(TopFood topFood)
        {
            try
            {
                db.Entry(topFood).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true , id = topFood.TopFoodID });
               
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(topFood);
        }
    }
}