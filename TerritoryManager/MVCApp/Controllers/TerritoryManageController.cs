using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Data;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TerritoryManageController : Controller
    {
        #region IndexAction

        public ActionResult Index(int page = 1)
        {
            var model =
                db.Territories
                .OrderBy(t => t.PostCodeFirst)
                .ToPagedList(page, 10);

            return View(model);
        }
        #endregion

        #region CreateAction

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Territory territory)
        {
            if (ModelState.IsValid)
            {
                db.Territories.Add(territory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }          

            return View(territory);
        }
        #endregion

        #region EditAction
        //
        // GET: /TerritoryManage/Edit/5

        public ActionResult Edit(int id)
        {
            var model = db.Territories.Find(id);

            if (model == null)
            {
                return new HttpNotFoundResult();
            }

            //Select dropdown values
            ViewBag.AssignedToUserId = new SelectList(db.UserProfiles, "UserId", "UserName", model.AssignedToUserId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Territory territory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(territory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(territory);
        }
        #endregion

        #region DeleteAction

        public ActionResult Delete(int id)
        {
            var model = db.Territories.Find(id);

            if (model == null)
            {
                return new HttpNotFoundResult();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Territory model = db.Territories.Find(id);
                db.Territories.Remove(model);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id });
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Database Access

        TerritoryDb db;

        public TerritoryManageController()
        {
            db = new TerritoryDb();
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
