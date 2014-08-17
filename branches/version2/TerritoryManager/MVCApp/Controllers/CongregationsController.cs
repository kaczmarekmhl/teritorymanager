using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    public class CongregationsController : BaseController
    {
        #region IndexAction

        public ActionResult Index()
        {
            return View(db.Congregations.OrderBy(c => c.Id));
        }

        #endregion

        #region CreateAction

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Congregation congregation)
        {
            if (ModelState.IsValid)
            {
                db.Congregations.Add(congregation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(congregation);
        }
        #endregion

        #region EditAction

        public ActionResult Edit(int id)
        {
            var congregation = db.Congregations.Find(id);

            if (congregation == null)
            {
                return new HttpNotFoundResult();
            }

            return View(congregation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Congregation congregation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(congregation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(congregation);
        }
        #endregion

        #region DeleteAction

        public ActionResult Delete(int id)
        {
            var model = db.Congregations.Find(id);

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
                Congregation model = db.Congregations.Find(id);
                db.Congregations.Remove(model);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id });
            }

            return RedirectToAction("Index");
        }
        #endregion

    }
}
