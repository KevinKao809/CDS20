using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExternalApplicationAgent.Models
{
    public class ERPModel
    {
        public string equipmentName { get; set; }
        [Required]
        public string equipmentId { get; set; }
        [Required]
        public string eventCode { get; set; }
        [Required]
        public string dateTimeString { get; set; }
        [Required]
        public string eventMessage { get; set; }
    }
}