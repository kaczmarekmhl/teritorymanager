using MapLibrary;
using MVCApp.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DistrictManageController : BaseController
    {
        #region IndexAction

        public ActionResult Index(int page = 1, string searchTerm = null, int? searchItemId = null)
        {
            District.QueryType queryType = DetectDistrictQueryType(searchTerm);
            var searchTermTruncated = ExtractSearchTerm(searchTerm, queryType);

            IQueryable<District> model = FilterDistrictQuery(db.Districts, queryType, searchTermTruncated, searchItemId);           
            model = District.OrderDistrictsForQueryType(model, queryType);

            ViewBag.SearchTerm = searchTerm;

            return View(model.ToPagedList(page, 100));
        }

        #endregion

        #region CreateAction

        public ActionResult Create()
        {
            //Select dropdown values
            ViewBag.AssignedToUserId = GetUserSelectionList(null);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(District district)
        {
            if (ModelState.IsValid)
            {
                district.Congregation = CurrentCongregation;

                district.LoadExternalDistrictBoundaryKml();

                db.Districts.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Select dropdown values
            ViewBag.AssignedToUserId = GetUserSelectionList(district.AssignedToUserId);

            return View(district);
        }
        #endregion

        #region EditAction
        //
        // GET: /DistrictManage/Edit/5

        public ActionResult Edit(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            district.LoadExternalDistrictBoundaryKml();

            //Select dropdown values
            ViewBag.AssignedToUserId = GetUserSelectionList(district.AssignedToUserId);

            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, District district)
        {
            var file = Request.Files[0];

            if (file != null && file.ContentLength > 0)
            {
                bool validKmlFile = false;

                if (Path.GetExtension(file.FileName) == ".kml")
                {
                    string xml = new StreamReader(file.InputStream).ReadToEnd();

                    if (KmlValidator.isValid(xml))
                    {
                        district.DistrictBoundaryKml = xml;
                        validKmlFile = true;
                    }
                }

                if (!validKmlFile)
                {
                    ModelState.AddModelError("DistrictBoundaryKml", "Invalid .kml file uploaded.");
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Select dropdown values
            ViewBag.AssignedToUserId = GetUserSelectionList(district.AssignedToUserId);

            return View(district);
        }
        #endregion

        #region AutocompleteAction

        public ActionResult Autocomplete(string term)
        {
            IQueryable modelJson;
            District.QueryType queryType = DetectDistrictQueryType(term);

            var searchTermTruncated = ExtractSearchTerm(term, queryType);
            var prefix = GetDistrictFilterPrefix(queryType);

            if (queryType == District.QueryType.User)
            {
                modelJson = FilterUserQuery(db.UserProfiles, searchTermTruncated)
                    .ToList()
                    .OrderBy(u => u.FullName)
                    .Take(10)
                    .Select(u => new
                    {
                        id = u.UserId,
                        label = prefix + u.FullName
                    })
                    .AsQueryable();
            }
            else
            {
                IQueryable<District> model = FilterDistrictQuery(db.Districts, queryType, searchTermTruncated, null);
                model = District.OrderDistrictsForQueryType(model, queryType)
                    .Take(10);

                switch (queryType)
                {
                    case District.QueryType.Number:
                        modelJson = model.Select(d => new
                        {
                            id = d.Id,
                            label = prefix + d.Number
                        });
                        break;
                    default:
                        modelJson = model.Select(d => new
                        {
                            id = d.Id,
                            label = d.Name
                        });
                        break;
                }
            }

            return Json(modelJson, JsonRequestBehavior.AllowGet);
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

        #region Helpers

        /// <summary>
        /// Filters user query.
        /// </summary>
        /// <param name="model">User profile query.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <returns>User profile query with filter.</returns>
        protected IQueryable<UserProfile> FilterUserQuery(IQueryable<UserProfile> model, string searchTerm)
        {
            var searchTermParts = searchTerm.Split(' ');
            var firstTerm = searchTermParts[0];

            var usersFiltered = SetCurrentCongregationFilter(db.UserProfiles);

            if (searchTermParts.Length > 1 && !string.IsNullOrEmpty(searchTermParts[1]))
            {
                var secondTerm = searchTermParts[1];
                usersFiltered = usersFiltered.Where(u =>
                    (u.FirstName.StartsWith(firstTerm) && u.LastName.StartsWith(secondTerm)) ||
                     u.UserName.StartsWith(firstTerm));
            }
            else
            {
                usersFiltered = usersFiltered.Where(u =>
                    u.FirstName.StartsWith(firstTerm) ||
                    u.UserName.StartsWith(firstTerm) ||
                    u.LastName.StartsWith(firstTerm));
            }

            return usersFiltered;
        }

        /// <summary>
        /// Filters district query.
        /// </summary>
        /// <param name="model">District query.</param>
        /// <param name="queryType">Query type.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <param name="searchItemId">Search item id.</param>
        /// <returns>District query with filter.</returns>
        protected IQueryable<District> FilterDistrictQuery(IQueryable<District> model, District.QueryType queryType, string searchTerm, int? searchItemId)
        {
            model = SetCurrentCongregationFilter(model);

            if (searchItemId.HasValue)
            {
                switch (queryType)
                {
                    case District.QueryType.Name:
                    case District.QueryType.Number:
                        return model.Where(t => t.Id == searchItemId.Value);

                    case District.QueryType.User:
                        return model.Where(u => u.AssignedTo.UserId == searchItemId.Value);
                }
            }

            if (!String.IsNullOrEmpty(searchTerm))
            {
                switch (queryType)
                {
                    case District.QueryType.Name:
                        return model.Where(t => t.Name.StartsWith(searchTerm));

                    case District.QueryType.Number:
                        return model.Where(t => t.Number.StartsWith(searchTerm));

                    case District.QueryType.User:
                        var usersFiltered = FilterUserQuery(db.UserProfiles, searchTerm);
                        return model.Join(usersFiltered, d => d.AssignedTo, u => u, (d, u) => d);
                }
            }
            else
            {
                if (queryType == District.QueryType.User)
                {
                    return model.Where(t => t.AssignedTo.Equals(null));
                }
            }

            return model;
        }

        protected District.QueryType DetectDistrictQueryType(string searchTerm)
        {
            if (!String.IsNullOrEmpty(searchTerm))
            {
                if (searchTerm.StartsWith(GetDistrictFilterPrefix(District.QueryType.User)))
                {
                    return District.QueryType.User;
                }
                else if (searchTerm.StartsWith(GetDistrictFilterPrefix(District.QueryType.Number)))
                {
                    return District.QueryType.Number;
                }
                else
                {
                    return District.QueryType.Name;
                }
            }

            return District.QueryType.None;
        }

        protected string ExtractSearchTerm(string searchTerm, District.QueryType queryType)
        {
            if (searchTerm == null)
            {
                return null;
            }

            return searchTerm.Substring(GetDistrictFilterPrefix(queryType).Length);
        }

        protected string GetDistrictFilterPrefix(District.QueryType queryType)
        {
            switch (queryType)
            {
                case District.QueryType.Number:
                    return "n:";
                case District.QueryType.User:
                    return "u:";
                default:
                    return "";
            }
        }

        #endregion

    }
}
