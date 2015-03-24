﻿using MVCApp.Models;
using MVCApp.Translate;
using Novacode;
using RazorPDF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebMatrix.WebData;
namespace MVCApp.Controllers
{
    public class DistrictAddressesController : BaseController
    {
        #region Index

        public ActionResult Index(int id, int page = 1)
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
                    kmlDoc.AddPlacemark(
                        String.Format("{0}. {1} {2}", counter++, person.Name, person.Lastname),
                        person.StreetAddress,
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
                Table table = doc.AddTable(personList.Count + 1, 4 + (district.IsMultiPostCode() ? 1 : 0));

                //table header
                var cellHeader = 0;
                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append("#");
                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.PersonName);
                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.PersonAddress);

                if (district.IsMultiPostCode())
                {
                    table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.DistrictPostCode);
                }

                table.Rows[0].Cells[cellHeader++].Paragraphs[0].Append(Strings.PersonTelephoneNum);

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

                    table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.Name + ' ' + person.Lastname);

                    table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.StreetAddress);

                    if (district.IsMultiPostCode())
                    {
                        table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.PostCode.ToString());
                    }

                    table.Rows[i].Cells[cell++].Paragraphs[0].Append(person.TelephoneNumber);

                    i++;
                }

                //table style
                table.Design = TableDesign.MediumList2Accent1;
                table.AutoFit = AutoFit.Contents;

                //table cell styles
                int rowNum = 0;
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (rowNum == 0)
                        {
                            cell.Paragraphs[0].Bold();
                        }

                        cell.Paragraphs[0].FontSize(12);
                        cell.Paragraphs[0].Font(new System.Drawing.FontFamily("Calibri"));
                        cell.MarginBottom = 2;
                        cell.MarginTop = 2;
                    }

                    rowNum++;
                }

                doc.InsertTable(table);

                doc.Save();
            }

            return File(stream.ToArray(), "application/octet-stream", GetDistrictAdressFileName(district) + ".docx");
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

            if (ModelState.IsValid)
            {
                person.Manual = true;
                person.AddedByUserId = WebSecurity.CurrentUserId;

                db.Persons.Add(person);
                db.SaveChanges();
                //return RedirectToAction("Index", new { id = person.District.Id });
                return Content("Person added", "text/html");
            }

            //return View(person);
            return Content("Error", "text/html");
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
                    && p.AddedByUserId == WebSecurity.CurrentUserId 
                    && (p.Selected == true || p.Manual == true))
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

        #endregion
    }
}
