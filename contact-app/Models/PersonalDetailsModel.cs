using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace contact_app.Models
{
    public sealed class PersonalDetailsModel
    {
        [Required()]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required()]
        [Display(Name = "Forename")]
        public string FirstName { get; set; }

        [Required()]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Required()]
        [Display(Name = "Phone Number")]
        public string PrimaryPhoneNumber { get; set; }
    }
}