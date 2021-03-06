﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVCApp.Models;
using MVCApp.Translate;
using Novacode;
using RazorPDF;
using WebMatrix.WebData;

namespace MVCApp.Controllers
{
    using System.Data.Entity;

    [Authorize]
    public class DistrictAddressesController : BaseController
    {
        #region Index

        public ActionResult Index(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            ViewBag.DistrictId = district.Id;
            ViewBag.DistrictName = district.Name;
            ViewBag.DistrictPostCode = district.PostCodeFirst;
            ViewBag.DistrictCountry = district.Congregation.Country;
            ViewBag.PersonListOutdated = district.IsPersonListOutdated();
            ViewBag.IsMultiPostCode = district.IsMultiPostCode();

            return View(GetPersonList(district.Id));
        }

        #endregion

        #region AdressesMap

        public ActionResult AdressesMap(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            return View(district);
        }

        #endregion

        #region AdressesMapKml

        public ActionResult AdressesMapKml(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return new HttpNotFoundResult();
            }

            var kmlDoc = district.GetDistrictBoundaryKmlDoc();

            var counter = 1;
            string lastStreetAddress = "";
            foreach (var person in GetPersonList(district.Id))
            {
                if (lastStreetAddress != person.StreetAddress)
                {
                    string googleMapNavigationLink = String.Empty;
                    if(Request.UserAgent.ToLower().Contains("android"))
                    {
                        googleMapNavigationLink = String.Format("<p class='googleMapNavigationAndroid'><a href='{0}'>{1}</a></p>", this.GetGoolgleMapAndroidUrl(person), "Nawigacja Google Map (Android)");
                    }

                    kmlDoc.AddPlacemark(
                        String.Format("{0}. <a href='{1}'>{2}</a>", counter++, this.Url.Action("EditPerson", new { id = person.Id, toMap = 1 }), person.StreetAddress),                        
                        (person.DoNotVisit ? " - " + Strings.PersonDoNotVisit : "") 
                        + (person.IsVisitedByOtherPublisher ? " - " + @String.Format(Strings.PersonVisitedBy, person.VisitingPublisher) : "")
                        + (String.IsNullOrEmpty(person.Remarks)? "" : String.Format("<p><b>{0}:</b> {1}</p>", Strings.PersonRemarks, person.Remarks))
                        + googleMapNavigationLink,
                        person.Longitude,
                        person.Latitude);
                }
                
                lastStreetAddress = person.StreetAddress;
            }

            return this.Content(kmlDoc.GetKmlWithPlacemarks().ToString(), "text/xml");
        }

        #endregion

        #region AdressesPdfFile

        public ActionResult AdressesPdfFile(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return null;
            }

            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.pdf", GetDistrictAdressFileName(district)));

            var pdf = new PdfResult(GetPersonList(district.Id), "AdressesPdf");
            
            pdf.ViewBag.DistrictName = district.Name;
            pdf.ViewBag.DistrictPostCode = district.PostCode;
            pdf.ViewBag.IsMultiPostCode = district.IsMultiPostCode();

            return pdf;
        }        

        #endregion

        #region AdressesDocFile

        public ActionResult AdressesDocFile(int id)
        {
            var district = db.Districts.Find(id);

            if (district == null)
            {
                return null;
            }

            return File(GetDocFile(district).ToArray(), "application/octet-stream", GetDistrictAdressFileName(district) + ".docx");
        }

        protected MemoryStream GetDocFile(District district)
        {
            var personList = GetPersonList(district.Id);

            MemoryStream stream = new MemoryStream();
            using (DocX doc = DocX.Create(stream))
            {
                //Header
                doc.AddHeaders();

                doc.Headers.odd
                    .Paragraphs[0]
                    .Append(String.Format("{0}, {1}", district.Name, district.PostCode))
                    .FontSize(16)
                    .Color(Color.DarkGray)
                    .Bold()
                    .UnderlineStyle(UnderlineStyle.singleLine);

                //Table with adresses
                Table table = doc.AddTable(personList.Count + 1, 3 + (district.IsMultiPostCode() ? 1 : 0));

                //table header
                var cellHeader = 0;
                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append("#");
                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.PersonAddress);

                if (district.IsMultiPostCode())
                {
                    table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.DistrictPostCode);
                }

                table.Rows[0].Cells[cellHeader].Paragraphs[0].Append(Strings.PersonRemarks);

                int i = 1;
                int counter = 1;
                string lastStreetAddress = "";
                foreach (var person in personList)
                {
                    int cell = 0;

                    if (lastStreetAddress != person.StreetAddress)
                    {
                        table.Rows[i].Cells[cell++].Paragraphs[0].Append(String.Format("{0}.", counter));
                        counter++;
                    }
                    else
                    {
                        cell++;
                    }
                    lastStreetAddress = person.StreetAddress;

                   table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.StreetAddress);

                    if (district.IsMultiPostCode())
                    {
                        table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.PostCodeFormat);
                    }

                    if (person.DoNotVisit)
                    {
                        table.Rows[i].Cells[cell].Paragraphs[0]
                            .Append(string.Format("({0}) ", Strings.PersonDoNotVisit))
                            .Color(Color.Red)
                            .Bold();
                    }

                    if (person.IsVisitedByOtherPublisher)
                    {
                        table.Rows[i].Cells[cell].Paragraphs[0]
                            .Append(string.Format("({0}) ", String.Format(Strings.PersonVisitedBy, person.VisitingPublisher)))
                            .Color(Color.CornflowerBlue)
                            .Bold();
                    }

                    table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.Remarks);

                    i++;
                }

                //table style
                table.Design = TableDesign.MediumList2Accent1;
                table.AutoFit = AutoFit.Contents;

                //table cell styles
                int rowNum = 0;
                foreach (var row in table.Rows)
                {
                    int cellNum = 0;
                    foreach (var cell in row.Cells)
                    {
                        if (rowNum == 0)
                        {
                            cell.Paragraphs[0].Bold();
                        }

                        if (cellNum > 2 && rowNum > 0)
                        {
                            cell.Paragraphs[0].FontSize(8);
                        }
                        else
                        {
                            cell.Paragraphs[0].FontSize(11);
                        }

                        cell.Paragraphs[0].Font(new FontFamily("Calibri"));
                        cell.MarginBottom = 2;
                        cell.MarginTop = 2;

                        cellNum++;
                    }

                    rowNum++;
                }

                doc.InsertTable(table);

                doc.Save();
            }

            return stream;
        }

        #endregion

        #region AddPerson

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerson(Person person)
        {
            int latlngMaxLength = 15;

            if (person.District.Id > 0)
            {
                person.District = db.Districts.Find(person.District.Id);

                person.Latitude = person.Latitude.Truncate(latlngMaxLength);
                person.Longitude = person.Longitude.Truncate(latlngMaxLength);

                ModelState.Clear();
                TryValidateModel(person);
            }

            if (!IsUserAuthorizedForDistrict(person.District))
            {
                return new HttpNotFoundResult();
            }

            if (ModelState.IsValid)
            {
                person.Manual = true;
                person.Selected = true; //If false it is not displayed
                person.AddedByUserId = WebSecurity.CurrentUserId;

                db.Persons.Add(person);
                db.SaveChanges();

                ViewBag.IsMultiPostCode = person.District.IsMultiPostCode();
                ViewBag.PersonCounter = String.Empty;
                ViewBag.DistrictId = person.District.Id;
                ViewBag.ShowEditButtons = true;
                return PartialView("_SelectedPersonRow", person);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        #endregion

        #region EditPerson
        public ActionResult EditPerson(int id)
        {
            var person = db.Persons.Include("District").Single(p => p.Id == id);
            
            if (!IsUserAuthrorizedForPerson(person))
            {
                return new HttpNotFoundResult();
            }
            
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPerson(int id, Person person, int? toMap, int? toAddressListWithAdd)
        {
            if (ModelState.IsValid)
            {
                var personDb = db.Persons.Include("District").Single(p => p.Id == id);
                personDb.Remarks = person.Remarks;
                personDb.TelephoneNumber = person.TelephoneNumber;
                
                personDb.DoNotVisit = person.DoNotVisit;
                personDb.DoNotVisitReportDate = personDb.DoNotVisit ? person.DoNotVisitReportDate : null;
                
                personDb.IsVisitedByOtherPublisher = person.IsVisitedByOtherPublisher;
                personDb.VisitingPublisher = personDb.IsVisitedByOtherPublisher ? person.VisitingPublisher : string.Empty;

                db.Entry(personDb).State = EntityState.Modified;
                db.SaveChanges();

                if(toMap == 1)
                {
                    return new RedirectResult(Url.Action("AdressesMap", new { personDb.District.Id }));
                }

                return new RedirectResult(Url.Action("Index", new { personDb.District.Id } ) + (toAddressListWithAdd == 1 ? "#add" : ""));
            }

            return View(person);
        }
        #endregion

        #region DeletePerson

        [HttpPost]
        public ActionResult DeletePerson(int personId, int districtId)
        {
            Person person = db.Persons.Find(personId);
            db.Entry(person).Reference(p => p.District).Load();

            if (!IsUserAuthrorizedForPerson(person))
            {
                return new HttpNotFoundResult();
            }

            if (person == null || person.District.Id != districtId)
            {
                return new HttpNotFoundResult();
            }

            if (person.DoNotVisit || person.IsVisitedByOtherPublisher)
            {
                return new HttpNotFoundResult(Strings.PersonDoNotVisitOrVisitedDeleteError);
            }

            //Person addresses are not removed just deselected, also for manually added
            person.Selected = false;
            db.SaveChanges();

            return new JsonResult()
            {
                Data = new { personId = person.Id }
            };
        }

        #endregion

        #region GetAddressPinFromGoogleChart
        [OutputCache(Duration = 3600, VaryByParam = "parameters")]
        public ActionResult GetAddressPinFromGoogleChart(string parameters)
        {
            using (var webClient = new WebClient())
            {
                var bytes = webClient.DownloadData("http://chart.apis.google.com/chart?" + parameters);

                return File(bytes, "application/octet-stream", parameters + ".png");
            }
        }
        #endregion

            #region Helpers
            /// <summary>
            /// Loads persisted and selected person list.
            /// </summary>
            /// <param name="district">District that the search will be done for.</param>
            /// <returns>Selected person list.</returns>
        private List<Person> GetPersonList(int districtId)
        {
            var list = db.Persons
                .Where(p => p.District.Id == districtId
                    && (p.AddedByUserId == WebSecurity.CurrentUserId || IsSharingAdressesEnabled || p.DoNotVisit || p.IsVisitedByOtherPublisher)
                    && (p.Selected == true))
                .ToList();

            return list.OrderBy(p => p.PostCode).ThenBy(p => p.StreetAddress).ToList();
        }

        /// <summary>
        /// Returns string that should be used as a file name of adresses file.
        /// </summary>
        /// <param name="district">Distris for which the file name will be returned.</param>
        /// <returns>File name.</returns>
        private string GetDistrictAdressFileName(District district)
        {
            if (district.PostCodeFirst == 0)
            {
                return district.Name;
            }
            else
            {
                return String.Format("{0}_{1}", district.Name, district.PostCode);
            }
        }

        protected bool IsUserAuthrorizedForPerson(Person person)
        {
            return person.AddedByUserId == WebSecurity.CurrentUserId || IsSharingAdressesEnabled || person.DoNotVisit || person.IsVisitedByOtherPublisher;
        }

        protected bool IsUserAuthorizedForDistrict(District district)
        {
            return district.AssignedToUserId == WebSecurity.CurrentUserId || IsSharingAdressesEnabled;
        }

        #endregion

        private string GetGoolgleMapAndroidUrl(Person person)
        {
            return String.Format("google.navigation:q={0},{1}",
                    person.Latitude,
                    person.Longitude);
        }
    }
}
