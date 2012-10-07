using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace contact_app.Models
{
    public sealed class CoverDetailsModel
    {
        [Required()]
        [Display(Name = "Please attach an up to date CV")]
        public HttpPostedFileBase CurriculumVitae { get; set; }

        [Required()]
        [StringLength(255, MinimumLength = 20)]
        [Display(Name = "What makes you suitable for the job?")]
        public string ReasonForAcceptance { get; set; }

        [Required()]
        [StringLength(255, MinimumLength = 20)]
        [Display(Name = "Why do you want the job?")]
        public string ReasonForApplying { get; set; }
    }
}