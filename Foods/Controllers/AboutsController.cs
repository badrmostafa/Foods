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
    public class AboutsController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Abouts
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
            var abouts = from a in db.Abouts select a;
            if (!string.IsNullOrEmpty(search))
            {
                abouts = abouts.Where(a => a.Head.ToUpper().Contains(search.ToUpper()) ||
                  a.Head1.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    abouts = abouts.OrderByDescending(a => a.Head);
                    break;
                default:
                    abouts = abouts.OrderBy(a => a.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 2;
            return View(abouts.ToPagedList(PageNumber, PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(About about)
        {
            if (ModelState.IsValid)
            {
                db.Abouts.Add(about);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new  HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            About about = db.Abouts.Find(id);
            if (about==null)
            {
                return HttpNotFound();
            }
            return View(about);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            About about = db.Abouts.Find(id);
            if (about == null)
            {
                return HttpNotFound();
            }
            return View(about);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(About about)
        {
            try
            {
                db.Entry(about).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (About)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The about was deleted by another user.");
                }
                else
                {
                    var databaseValues = (About)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Head1 != clientValues.Head1)
                        ModelState.AddModelError("Head1", "Current Value:" + databaseValues.Head1);
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Description", "Current Value:" + databaseValues.Description);
                    if (databaseValues.Image != clientValues.Image)
                        ModelState.AddModelError("Image", "Current Value:" + databaseValues.Image);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit click the save button again,otherwise click the back to list hyperlink.");
                    about.RowVersion = databaseValues.RowVersion;
                }
                
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            return View(about);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            About about = db.Abouts.Find(id);
            if (about == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (about==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        +" Click The back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to delete this record click the delete button again ,otherwise click the back to list hyperlink.";
                }
            }
            return View(about);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(About about)
        {
            try
            {
                db.Entry(about).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = about.AboutID });
                
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(about);
        }
    }
}