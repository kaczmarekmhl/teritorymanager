using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace MVCApp.Helpers
{
    public static class UserContext
    {
        public static bool IsSharingAdressesEnabled(DistictManagerDb db)
        {
            return Congregation(db).SharingAddressesEnabled;
        }

        public static Congregation Congregation(DistictManagerDb db)
        {
            var currentUser = db.UserProfiles.Find(WebSecurity.CurrentUserId);

            if (currentUser == null)
            {
                throw new Exception("Current congregation not found!");
            }

            return currentUser.Congregation;
        }
    }
}