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
    public class GalleryController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Gallery
        public ActionResult Index(string sort,string search,string filter,int? page)
        {
            ViewBag.sort = sort;
            ViewBag.Head = string.IsNullOrEmpty(sort) ? "head_desc" : "";
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = filter;
            }
            ViewBag.filter = search;
            var galleries = from g in db.Galleries select g;
            if (!string.IsNullOrEmpty(search))
            {
                galleries = galleries.Where(g => g.Head.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    galleries = galleries.OrderByDescending(g => g.Head);
                    break;
                default:
                    galleries = galleries.OrderBy(g => g.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 2;
            return View(galleries.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                db.Galleries.Add(gallery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gallery);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery==null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(Gallery gallery)
        {
            try
            {
                db.Entry(gallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Gallery)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The gallery was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Gallery)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Icon != clientValues.Icon)
                        ModelState.AddModelError("Icon", "Current Value:" + databaseValues.Icon);
                    if (databaseValues.Image != clientValues.Image)
                        ModelState.AddModelError("Image", "Current Value:" + databaseValues.Image);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit this record click the save button again,otherwise click the back to list hyperlink.");
                    gallery.RowVersion = databaseValues.RowVersion;
                }
                
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again ,and if the problem persists contact your system administrator.");
            }
            return View(gallery);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (gallery==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        +" Click the back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to delete this record click the delete button again,otherwise click the back to list hyperlink.";
                }
            }
            return View(gallery);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(Gallery gallery)
        {
            try
            {
                db.Entry(gallery).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {

                return RedirectToAction("Delete", new { concurrencyError = true, id = gallery.GalleryID });
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(gallery);
        }
    }
}