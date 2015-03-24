using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Returns current congregation based on authenticated user.
        /// </summary>
        /// <returns></returns>
        protected Congregation GetCurrentCongregation()
        {
            var currentUser = db.UserProfiles.Find(WebSecurity.CurrentUserId);

            if (currentUser == null)
            {
                throw new Exception("Current congregation not found!");
            }

            return currentUser.Congregation;
        }

        /// <summary>
        /// Sets current congregation filter on given query.
        /// </summary>
        /// <returns></returns>
        protected IQueryable<District> SetCurrentCongregationFilter(IQueryable<District> model)
        {
            var currentCongregation = GetCurrentCongregation();

            return model.Where(t => t.Congregation.Id.Equals(currentCongregation.Id));
        }

        /// <summary>
        /// Sets current congregation filter on given query.
        /// </summary>
        /// <returns></returns>
        protected IQueryable<UserProfile> SetCurrentCongregationFilter(IQueryable<UserProfile> model)
        {
            var currentCongregation = GetCurrentCongregation();

            return model.Where(u => u.Congregation.Id.Equals(currentCongregation.Id));
        }

        /// <summary>
        /// Sets current congregation filter on given query.
        /// </summary>
        /// <returns></returns>
        protected IQueryable<DistrictReport> SetCurrentCongregationFilter(IQueryable<DistrictReport> model)
        {
            var currentCongregation = GetCurrentCongregation();

            return model.Where(u => u.District.Congregation.Id.Equals(currentCongregation.Id));
        }

        #region Database Access

        protected DistictManagerDb db;

        public BaseController()
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

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
