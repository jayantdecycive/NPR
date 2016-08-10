using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace cfares.Areas.Tours.Models
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

    }

    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string email { get; set; }
    }
}