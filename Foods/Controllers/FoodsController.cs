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
    public class FoodsController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Foods
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
            var foods = from f in db.Foods select f;
            if (!string.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.Head.ToUpper().Contains(search.ToUpper()) ||
                  f.Description.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    foods = foods.OrderByDescending(f => f.Head);
                    break;
                default:
                    foods = foods.OrderBy(f => f.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 3;
            return View(foods.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(Food food)
        {
            if (ModelState.IsValid)
            {
                db.Foods.Add(food);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(food);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Food food = db.Foods.Find(id);
            if (food==null)
            {
                return HttpNotFound();
            }
            return View(food);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(Food food)
        {
            try
            {
                db.Entry(food).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Food)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The food was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Food)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Description", "Current Value:" + databaseValues.Description);
                    if (databaseValues.Image != clientValues.Image)
                        ModelState.AddModelError("Image", "Current Value:" + databaseValues.Image);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit this record click the save button again,otherwise click the back to list hyperlink.");
                    food.RowVersion = databaseValues.RowVersion;
                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            return View(food);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (food==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        +" click the back to list hyperlink";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to delete click the delete button again,otherwise click the back to list hyperlink";
                }
            }
            return View(food);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(Food food)
        {
            try
            {
                db.Entry(food).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = food.FoodID });
                
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(food);
        }

    }
}