using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class DistrictModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BelongsTo { get; set; }
    }
}