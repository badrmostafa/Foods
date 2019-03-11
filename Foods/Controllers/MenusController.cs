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
    public class MenusController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Menus
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
            var menus = from m in db.Menus select m;
            if (!string.IsNullOrEmpty(search))
            {
                menus = menus.Where(m => m.Head.ToUpper().Contains(search.ToUpper()) ||
                  m.Head1.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    menus = menus.OrderByDescending(m => m.Head);
                    break;
                default:
                    menus = menus.OrderBy(m => m.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 3;
            return View(menus.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(Menu menu)
        {
            if (ModelState.IsValid)
            {
                db.Menus.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Menu menu = db.Menus.Find(id);
            if (menu==null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(Menu menu)
        {
            try
            {
                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Menu)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The menu was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Menu)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Head1 != clientValues.Head1)
                        ModelState.AddModelError("Head1", "Current Value:" + databaseValues.Head1);
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Description", "Current Value:" + databaseValues.Description);
                    if (databaseValues.Price != clientValues.Price)
                        ModelState.AddModelError("Price", "Current Value:" + databaseValues.Price);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit this record click the save button again,otherwise click the back to list hyperlink.");
                    menu.RowVersion = databaseValues.RowVersion;
                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            return View(menu);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (menu==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value "
                        +" Click the back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to delete this record click the delete button again,otherwise click the back to list hyperlink.";
                }
            }
            return View(menu);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(Menu menu)
        {
            try
            {
                db.Entry(menu).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {

                return RedirectToAction("Delete", new { concurrencyError = true, id = menu.MenuID });
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(menu);
        }
    }
}