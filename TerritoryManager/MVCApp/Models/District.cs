using MapLibrary;
using MVCApp.Translate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace MVCApp.Models
{
    public class District
    {
        #region Properties

        /// <summary>
        /// Dictrict record identificator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dictrict number.
        /// </summary>
        [StringLength(20)]
        [Display(ResourceType = typeof(Strings), Name = "DistrictNumber")]
        public string Number { get; set; }

        /// <summary>
        /// Dictrict name.
        /// </summary>
        [Required]
        [StringLength(30)]
        [Display(ResourceType = typeof(Strings), Name = "DistrictName")]
        public string Name { get; set; }

        /// <summary>
        /// First post code of dictrict.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "DistrictFirstPostCode")]
        public int PostCodeFirst { get; set; }

        /// <summary>
        /// Last post code of dictrict
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DistrictLastPostCode")]
        public int? PostCodeLast { get; set; }

        /// <summary>
        /// Post code range used for display.
        /// </summary>
        [NotMapped]
        [Display(ResourceType = typeof(Strings), Name = "DistrictPostCode")]
        public string PostCode
        {
            get
            {
                if (!PostCodeLast.HasValue || PostCodeFirst == PostCodeLast.Value)
                {
                    return string.Format("{0:0000}", PostCodeFirst);
                }
                else
                {
                    return string.Format("{0:0000}-{1:0000}", PostCodeFirst, PostCodeLast);
                }
            }

            private set { }
        }

        /// <summary>
        /// The UserProfile.UserId that is assigned to this dictrict.
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        /// The UserProfile that is assigned to this dictrict.
        /// </summary>        
        public virtual UserProfile AssignedTo { get; set; }

        /// <summary>
        /// The list of Person found in this dictrict.
        /// </summary>
        public virtual ICollection<Person> PersonList { get; set; }

        /// <summary>
        /// Kml file with district boundary.
        /// </summary>
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Strings), Name = "DistrictBoundaryKml")]
        public string DistrictBoundaryKml { get; set; }

        /// <sumary>
        /// The list of all reports submited for this district by all users.
        /// </sumary>
        public virtual ICollection<DistrictReport> DistrictReports { get; set; }

        /// <summary>
        /// Congregation that this district belongs to.
        /// </summary>
        public virtual Congregation Congregation { get; set; }

        /// <summary>
        /// Search Phrase
        /// </summary>
        [StringLength(30)]
        [Display(ResourceType = typeof(Strings), Name = "DistrictSearchPhrase")]
        public string SearchPhrase { get; set; }

        #endregion


        #region DistrictReport Area
       
        [NotMapped]
        public DistrictReport Reports_LatestCompleteReport
        {
            get
            {
                if (DistrictReports != null)
                {
                    return DistrictReports
                        .Where(dr => dr.Type == DistrictReport.ReportTypes.Complete)
                        .OrderByDescending(dr => dr.Date)
                        .FirstOrDefault();
                }

                return null;
            }
        }

        /// <sumary>
        /// The list of district complete reports submited by the user.
        /// The user may have the district not the first time, so we only show the dates that he submited since he last received the district.
        /// </sumary>
        [NotMapped]
        public List<DistrictReport> UserReports_DistrictComplete
        {
            get
            {
                return GetUserCompleteReportsSinceLastRequest();
            }
        }

        /// <sumary>
        /// The min date the user can report the he completed the district.
        /// </sumary>
        [NotMapped]
        public DateTime UserReport_MinDateForCompletion
        {
            get
            {
                // if this is not the first time the user completed this district
                if (UserReports_DistrictComplete.Count > 0)
                {
                    return UserReports_DistrictComplete.First().Date.AddDays(1);
                }
                
                // to be safe return default value
                return DateTime.Now.AddYears(-1);
            }
        }

        /// <sumary>
        /// The list of district complete reports submited by the active user since last request.
        /// </sumary>
        public List<DistrictReport> GetUserCompleteReportsSinceLastRequest()
        {
            var districtReports = new List<DistrictReport>();

            if (DistrictReports != null)
            {
                var lastRequest = DistrictReports
                    .Where(dr =>
                        dr.UserId == WebSecurity.CurrentUserId 
                        && dr.Type == DistrictReport.ReportTypes.Request)
                        .OrderByDescending(dr => dr.Date)
                        .FirstOrDefault();

                districtReports = DistrictReports.Where(dr =>
                            dr.UserId == WebSecurity.CurrentUserId
                            && dr.Type == DistrictReport.ReportTypes.Complete
                            && (lastRequest == null || dr.Date.Date >= lastRequest.Date.Date))
                            .OrderByDescending(dr => dr.Date)
                            .ToList();
            }

            return districtReports;
        }

        #endregion

        /// <summary>
        /// Loads Kml file from external service.
        /// </summary>
        public void LoadExternalDistrictBoundaryKml(Boolean overrideKml = false)
        {
            if ((overrideKml || string.IsNullOrEmpty(DistrictBoundaryKml)) && Congregation.Country == Enums.Country.Denmark)
            {
                DistrictBoundaryKml = ExternalKmlProvider.LoadKml(PostCodeFirst);
            }
        }

        /// <summary>
        /// Returns KmlDocument with preprocessed district boundary.
        /// </summary>
        /// <param name="completedDateColor">If true boundary color will be based on last completed date</param>
        /// <returns>KmlDocument with district boundary</returns>
        public KmlDocument GetDistrictBoundaryKmlDoc(bool completedDateColor = false)
        {
            string borderColor = "ff0000ff";

            string greenColor = "5950c24a";
            string yellowColor = "5919d9ff";
            string redColor = "594b6fff";

            LoadExternalDistrictBoundaryKml();

            var kmlDoc = new KmlDocument(DistrictBoundaryKml);

            if (completedDateColor)
            {
                var completedReport = Reports_LatestCompleteReport;

                if (completedReport == null || completedReport.Date < DateTime.Now.Date.AddMonths(-12))
                {
                    kmlDoc.ChangeBoundaryColor(borderColor, redColor);
                }
                else if (completedReport.Date < DateTime.Now.Date.AddMonths(-6))
                {
                    kmlDoc.ChangeBoundaryColor(borderColor, yellowColor);
                }
                else
                {
                    kmlDoc.ChangeBoundaryColor(borderColor, greenColor);
                }
            }
            else
            {
                kmlDoc.ChangeBoundaryColor(borderColor, greenColor);
            }

            kmlDoc.SetBoundaryName(Name);

            return kmlDoc;
        }

        /// <summary>
        /// Specifies if district has multiple post codes in it.
        /// </summary>
        public bool IsMultiPostCode()
        {
            return PostCodeFirst == 0 || PostCodeLast.HasValue && PostCodeFirst != PostCodeLast.Value;
        }

        #region Constructors

        public District(string postCode)
        {
            this.PostCode = postCode;
        }

        public District()
        {
            PostCodeFirst = 0;
        }

        #endregion
    }
}