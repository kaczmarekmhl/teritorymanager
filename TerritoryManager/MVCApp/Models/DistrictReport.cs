using MVCApp.Translate;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MVCApp.Models
{
    public class DistrictReport
    {
        #region Properties

        /// <summary>
        /// Dictrict report Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dictrict that has been reported.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "DistrictNumber")]
        public virtual District District { get; set; }

        /// <summary>
        /// Report types enum.
        /// </summary>
        public enum ReportTypes
        {
            [Display(ResourceType = typeof(Strings), Name = "DistrictReportTypes_Complete")]
            Complete = 0,
            [Display(ResourceType = typeof(Strings), Name = "DistrictReportTypes_Return")]
            Return = 1,
            [Display(ResourceType = typeof(Strings), Name = "DistrictReportTypes_Request")]
            Request = 2
        };

        [NotMapped]
        public string ReportTypeString
        {
            get
            {
                FieldInfo field = Type.GetType().GetField(Type.ToString());

                var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
                string displayName = Strings.ResourceManager.GetString(attribute.Name);

                return attribute == null ? Type.ToString() : displayName;
            }
        }

        /// <summary>
        /// Report statuses enum.
        /// </summary>
        public enum ReportStates
        {
            [Display(ResourceType = typeof(Strings), Name = "Pending")]
            Pending = 0,
            [Display(ResourceType = typeof(Strings), Name = "Approved")]
            Approved = 1,
            [Display(ResourceType = typeof(Strings), Name = "Rejected")]
            Rejected = 2,
        };

        /// <summary>
        /// State of the district report.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "State")]
        public ReportStates State { get; set; }

        /// <summary>
        /// Type of the district report.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DistrictReportType")]
        public ReportTypes Type { get; set; }

        /// <summary>
        /// Date of the action.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(Strings), Name = "Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The user that submits the report.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "AccountUserName")]
        public virtual UserProfile User { get; set; }

        /// <summary>
        /// The id of the user that submits the report.
        /// </summary>
        public int UserId { get; set; }

        #endregion

        public DistrictReport(District district, int userId, ReportTypes type, ReportStates state = ReportStates.Pending)
        {
            this.District = district;
            this.UserId = userId;
            this.Type = type;
            this.State = state;
            this.Date = DateTime.Now;
        }

        public DistrictReport()
        {
        }
    }
}