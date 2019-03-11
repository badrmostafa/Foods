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
    public class ClientsController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Clients
        public ActionResult Index(string sort,string filter,string search,int? page)
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
            var clients = db.Clients.Include(c => c.Menu);
            if (!string.IsNullOrEmpty(search))
            {
                clients = clients.Where(c => c.Head.ToUpper().Contains(search.ToUpper()) ||
                  c.Head1.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    clients = clients.OrderByDescending(c => c.Head);
                    break;
                default:
                    clients = clients.OrderBy(c => c.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 2;
            return View(clients.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            ViewBag.MenuID = new SelectList(db.Menus, "MenuID", "Head");
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MenuID = new SelectList(db.Menus, "MenuID", "Head",client.MenuID);
            return View(client);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client==null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuID = new SelectList(db.Menus, "MenuID", "Head");
            return View(client);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(Client client)
        {
            try
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Client)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The client was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Client)databaseEntry.ToObject();
                    if (databaseValues.Image != clientValues.Image)
                        ModelState.AddModelError("Image", "Current Value:" + databaseValues.Image);
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.Head1 != clientValues.Head1)
                        ModelState.AddModelError("Head1", "Current Value:" + databaseValues.Head1);
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Description", "Current Value:" + databaseValues.Description);
                    if (databaseValues.MenuID != clientValues.MenuID)
                        ModelState.AddModelError("MenuID", "Current Value:" + db.Menus.Find(databaseValues.MenuID));
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user if you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you stil want to edit this record click the save button again,otherwise click the back to list hyperlink.");
                    client.RowVersion = databaseValues.RowVersion;
                }
               
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            ViewBag.MenuID = new SelectList(db.Menus, "MenuID", "Head",client.MenuID);
            return View(client);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                if (concurrencyError==true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (client==null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        +" Click the back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        +" The delete operation was cacelled and the current value in the database have been displayed"
                        +" If you still want to delete this record click the delete button again,otherwise click the back to list hyperlink.";
                }
            }
            return View(client);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(Client client)
        {
            try
            {
                db.Entry(client).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {

                return RedirectToAction("Delete", new { concurrencyError = true, id = client.ClientID });
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator. ");
            }
            return View(client);
        }
    }
}