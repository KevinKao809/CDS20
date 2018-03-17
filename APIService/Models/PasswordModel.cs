using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class PasswordModel
    {
        public class Format_Change
        {
            [Required]
            public string OldPassword { get; set; }
            [Required]
            public string NewPassword { get; set; }
        }
        public class Format_Reset
        {
            [Required]
            public string NewPassword { get; set; }
        }
    }
}