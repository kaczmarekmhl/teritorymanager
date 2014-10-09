using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        #region IndexAction

        public ActionResult Index()
        {
            var model =
                SetCurrentCongregationFilter(db.UserProfiles)
                .OrderBy(u => u.UserName);

            return View(model);
        }

        #endregion

        #region EditAction

        public ActionResult Edit(int id)
        {
            var user = db.UserProfiles.Find(id);

            if (user == null)
            {
                return new HttpNotFoundResult();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
        #endregion

        #region DeleteAction

        public ActionResult Delete(int id)
        {
            var model = db.UserProfiles.Find(id);

            if (model == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.UserHasDistricts = db.Districts.Count(d => d.AssignedToUserId == id) > 0;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                UserProfile model = db.UserProfiles.Find(id);
                var membership = (SimpleMembershipProvider)Membership.Provider;
                membership.DeleteUser(model.UserName, true);
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
