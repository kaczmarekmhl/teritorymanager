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
    public class DistrictManageController : Controller
    {
        #region IndexAction

        public ActionResult Index(int page = 1)
        {
            var model =
                db.Districts
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
        public ActionResult Create(District district)
        {
            if (ModelState.IsValid)
            {
                db.Districts.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }          

            return View(district);
        }
        #endregion

        #region EditAction
        //
        // GET: /DistrictManage/Edit/5

        public ActionResult Edit(int id)
        {
            var model = db.Districts.Find(id);

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
        public ActionResult Edit(int id, District district)
        {
            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(district);
        }
        #endregion

        #region DeleteAction

        public ActionResult Delete(int id)
        {
            var model = db.Districts.Find(id);

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
                District model = db.Districts.Find(id);
                db.Districts.Remove(model);
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

        DistictManagerDb db;

        public DistrictManageController()
        {
            db = new DistictManagerDb();
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
