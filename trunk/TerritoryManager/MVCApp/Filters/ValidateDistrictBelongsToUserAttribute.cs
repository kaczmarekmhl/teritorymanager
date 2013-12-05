using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Filters
{
    public class ValidateDistrictBelongsToUserAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public string ViewName;
        private TerritoryDb _db = new TerritoryDb();

        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            var searchDistrictID = (string)filterContext.ActionParameters["UserDistrict"];
            if (searchDistrictID != null)
            {
                var userDistrict = from dist in _db.Districts
                                    where dist.BelongsToUser == "Bartek"
                                    where dist.Id == searchDistrictID
                                    select dist;

                if (userDistrict.Count() == 0)
                {
                    filterContext.ActionParameters["UserDistrict"] = null;  
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}