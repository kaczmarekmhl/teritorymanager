using MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Data;
using System.IO;
using System.Xml;
using MapLibrary;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DistrictManageController : BaseController
    {
        #region IndexAction

        public ActionResult Index(int page = 1, string searchTerm = null)
        {
            IQueryable<District> model = FilterDistrictQuery(db.Districts, searchTerm);

            var modelPaged = model.OrderBy(t => t.PostCodeFirst)
                .ThenBy(t => t.Name)
                .ToPagedList(page, 100);

            ViewBag.SearchTerm = searchTerm;

            return View(modelPaged);
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
                district.Congregation = CurrentCongregation;

                district.LoadExternalDistrictBoundaryKml();

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
            DistrictQueryType queryType = DetectDistrictQueryType(term);
            IQueryable<District> model = FilterDistrictQuery(db.Districts, term)
                .OrderBy(t => t.PostCodeFirst)
                .ThenBy(t => t.Name)
                .Take(10);

            term = ExtractSearchTerm(term, queryType);

            var prefix = GetDistrictFilterPrefix(queryType);

            switch (queryType)
            {
                case DistrictQueryType.Number:
                    modelJson = model.Select(d => new
                    {
                        label = prefix + d.Number
                    });
                    break;
                case DistrictQueryType.User:
                    modelJson = db.UserProfiles
                        .Where(u => u.UserName.StartsWith(term))
                        .Where(u => u.CongregationId == CurrentCongregation.Id)
                        .OrderBy(u => u.UserName)
                        .Take(10)
                        .Select(u => new
                        {
                            label = prefix + u.UserName
                        });
                    break;
                default:
                    modelJson = model.Select(d => new
                    {
                        label = d.Name
                    });
                    break;
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

        protected enum DistrictQueryType { None, Name, Number, User };

        /// <summary>
        /// Filters district query.
        /// </summary>
        /// <param name="model">District query.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <returns>District query with filter.</returns>
        protected IQueryable<District> FilterDistrictQuery(IQueryable<District> model, string searchTerm)
        {
            model = SetCurrentCongregationFilter(model);

            DistrictQueryType queryType = DetectDistrictQueryType(searchTerm);
            searchTerm = ExtractSearchTerm(searchTerm, queryType);

            if (!String.IsNullOrEmpty(searchTerm))
            {
                switch (queryType)
                {
                    case DistrictQueryType.Name:
                        return model.Where(t => t.Name.StartsWith(searchTerm));

                    case DistrictQueryType.Number:
                        return model.Where(t => t.Number.StartsWith(searchTerm));

                    case DistrictQueryType.User:
                        return model.Where(t => t.AssignedTo.UserName.StartsWith(searchTerm));
                }
            }
            else
            {
                if (queryType == DistrictQueryType.User)
                {
                    return model.Where(t => t.AssignedTo.Equals(null));
                }
            }

            return model;
        }

        protected DistrictQueryType DetectDistrictQueryType(string searchTerm)
        {
            if (!String.IsNullOrEmpty(searchTerm))
            {
                if (searchTerm.StartsWith(GetDistrictFilterPrefix(DistrictQueryType.User)))
                {
                    return DistrictQueryType.User;
                }
                else if (searchTerm.StartsWith(GetDistrictFilterPrefix(DistrictQueryType.Number)))
                {
                    return DistrictQueryType.Number;
                }
                else
                {
                    return DistrictQueryType.Name;
                }
            }

            return DistrictQueryType.None;
        }

        protected string ExtractSearchTerm(string searchTerm, DistrictQueryType queryType)
        {
            if (searchTerm == null)
            {
                return null;
            }

            return searchTerm.Substring(GetDistrictFilterPrefix(queryType).Length);
        }

        protected string GetDistrictFilterPrefix(DistrictQueryType queryType)
        {
            switch (queryType)
            {
                case DistrictQueryType.Number:
                    return "n:";
                case DistrictQueryType.User:
                    return "u:";
                default:
                    return "";
            }
        }

        #endregion

    }
}
