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

            ViewBag.Congregations = db.Congregations.ToList();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile user)
        {
            if (ModelState.IsValid)
            {
                var model = db.UserProfiles.Find(id);
                model.UserName = user.UserName;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;

                if (User.Identity.Name.Equals("admin"))
                {
                    model.CongregationId = user.CongregationId;
                }

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Congregations = db.Congregations.ToList();

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
            ViewBag.UserIsAdmin = ((SimpleRoleProvider)Roles.Provider).IsUserInRole(model.UserName, "admin");

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
