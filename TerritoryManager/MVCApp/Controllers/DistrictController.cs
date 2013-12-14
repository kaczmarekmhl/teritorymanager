using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCApp.Models;
using AddressSearch.AdressProvider;
using AddressSearch.AdressProvider.Filters;
using System.Collections;
using AddressSearch.AdressProvider.Filters.PersonFilter;
using MVCApp.Filters;
using System.Data.Entity;
using System.Net;
using WebMatrix.WebData;
using MVCApp.Exceptions;

namespace MVCApp.Controllers
{
    /*
    [Authorize]
    public class DistrictController : Controller
    {
        ITerritoryDb _db;
        HashSet<string> RolesAllowedToAccessAllDistricts = new HashSet<string>() { "Admin" };

        // The constructor will execute when the app is run on a web server with SQL Server
        public DistrictController()
        {
            _db = new TerritoryDb();
        }

        // The constructor used for unit tests to fake a db
        public DistrictController(ITerritoryDb db)
        {
            _db = db;
        }


        //
        // GET: /District/

        public ActionResult Index()
        {
            PopulateSearchDistrictMenu(User.Identity.Name);
            return View("_Select");
        }

        //
        // GET: /District/Search?UserDistrict=1&DisplayOnlyDeletedPersons=False
    
        public ActionResult Search([Bind(Prefix = "DistrictSelectListItem")]string selectedDistrictId, bool displayOnlyDeletedPersons = false)
        {
            string currentUserName = User.Identity.Name;
            try
            {
                ValidateDistrictExist(selectedDistrictId);
                if (!IsUserInRole(RolesAllowedToAccessAllDistricts, currentUserName))
                {
                    ValidateDistrictBelongsToUser(selectedDistrictId, currentUserName);
                }
            }
            catch (InvalidDistrictException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PopulateSearchDistrictMenu(currentUserName, selectedDistrictId);

            // To display the current state of the menu pass information about the selected options to the view
            ViewBag.DisplayOnlyDeletedPersons = displayOnlyDeletedPersons;
            ViewBag.SelectedDistrictId = selectedDistrictId;
            return View();
        }

     
        private void PopulateSearchDistrictMenu(string userName, string selectedDistrictId = null)
        {

            IEnumerable<District> districtList;

            if (IsUserInRole(RolesAllowedToAccessAllDistricts, userName))
            {
                districtList = _db.Query<District>();
            }
            else
            {
                districtList = _db.Query<District>()
                    .Where(d => d.BelongsToUser != null)
                    .Where(d => d.BelongsToUser.UserName == userName);
            }


            if (districtList == null ||
                districtList.Count() == 0)
            {
                ViewBag.ErrorMsg = "You don't have any district assigned to you.";
            }

            // Get select district list item with users's districts
            ViewBag.DistrictSelectListItem = GetDistrictSelectListItem(selectedDistrictId, districtList);
        }

        private void ValidateDistrictExist(string selectedDistrictId)
        {
            if (selectedDistrictId == null)
            {
                throw new InvalidDistrictException("District Id can not be null");
            }

            var actualSearchDistrict = _db.Query<District>().SingleOrDefault(d => d.Id == selectedDistrictId);
            if (actualSearchDistrict == null)
            {
                throw new InvalidDistrictException("District Id does not exist");
            }
        }

        private void ValidateDistrictBelongsToUser(string selectedDistrictId, string userName)
        {
            var actualSearchDistrict = _db.Query<District>().SingleOrDefault(d => d.Id == selectedDistrictId);
            var belongsToUser = actualSearchDistrict.BelongsToUser;
            if (belongsToUser == null || !userName.Equals(belongsToUser.UserName))
            {
                throw new InvalidDistrictException("User does not have rights to see the district");
            }
        }

        private bool IsUserInRole(HashSet<string> roles, string userName)
        {
            return roles.Any(role => User.IsInRole(role));
        }

        private List<SelectListItem> GetDistrictSelectListItem(string selectedDistrictId, IEnumerable<District> districtList)
        {
            var items = new List<SelectListItem>();
            foreach (var district in districtList.OrderBy(d => d.PostCode))
            {
                items.Add(
                    district.Id.Equals(selectedDistrictId)
                    ? new SelectListItem { Text = string.Format("{0} {1}", district.PostCode, district.Name), Value = district.Id, Selected = true }
                    : new SelectListItem { Text = string.Format("{0} {1}", district.PostCode, district.Name), Value = district.Id }
                    );
            }
            return items;
        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }*/
}
