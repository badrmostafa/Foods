using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Net;
using Foods.Models.Classes;
using PagedList;

namespace Foods.Controllers
{
    public class ReservationsController : Controller
    {
        private FoodContext db = new FoodContext();
        // GET: Reservations
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
            var reservations = db.Reservations.Include(r => r.Client);
            if (!string.IsNullOrEmpty(search))
            {
                reservations = reservations.Where(r => r.Head.ToUpper().Contains(search.ToUpper()));
            }
            switch (sort)
            {
                case "head_desc":
                    reservations = reservations.OrderByDescending(r => r.Head);
                    break;
                default:
                    reservations = reservations.OrderBy(r => r.Head);
                    break;
            }
            int PageNumber = (page ?? 1);
            int PageSize = 2;
            return View(reservations.ToPagedList(PageNumber,PageSize));
        }
        //Get Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Head");
            return View();
        }
        //Post Create
        [HttpPost]
        public ActionResult Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Head",reservation.ClientID);
            return View(reservation);
        }
        //Get Edit
        public ActionResult Edit(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation==null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Head");
            return View(reservation);
        }
        //Post Edit
        [HttpPost]
        public ActionResult Edit(Reservation reservation)
        {
            try
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Reservation)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry==null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes.The reservation was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Reservation)databaseEntry.ToObject();
                    if (databaseValues.Head != clientValues.Head)
                        ModelState.AddModelError("Head", "Current Value:" + databaseValues.Head);
                    if (databaseValues.ClientID != clientValues.ClientID)
                        ModelState.AddModelError("ClientID", "Current Value:" + db.Clients.Find(databaseValues.ClientID));
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value"
                        +" The edit operation was cancelled and the current value in the database have been displayed"
                        +" If you still want to edit this record click the save button again,otherwise click the back to list hyperlink.");
                    reservation.RowVersion = databaseValues.RowVersion;
                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.Try again,and if the problem persists contact your system administrator.");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Head",reservation.ClientID);
            return View(reservation);
        }
        //Get Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                
                return HttpNotFound();
            }
          
            return View(reservation);
        }
        //Get Delete
        public ActionResult Delete(int? id,bool? concurrencyError)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                if (concurrencyError == true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                if (reservation == null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was deleted by another user after you got the original value"
                        + " Click the back to list hyperlink.";
                }
                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete was modified by another user after you got the original value"
                        + " The delete operation was cancelled and the current value in the database have been displayed"
                        + " If you still want to delete this record click the delete button again,otherwise click the back to list hyperlink.";
                }
            }
            return View(reservation);
        }
        //Post Delete
        [HttpPost]
        public ActionResult Delete(Reservation reservation)
        {
            try
            {
                db.Entry(reservation).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = reservation.ReservationID });
                
            }
            catch(DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete.Try again,and if the problem persists contact your system administrator.");
            }
            return View(reservation);
        }
        
    }
}