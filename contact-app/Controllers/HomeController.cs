using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using contact_app.Extensions;

namespace contact_app.Controllers
{
    public class HomeController : Controller
    {
        #region "Internal Constants"
        public const string cPersonalDetails = "PersonalDetails";
        public const string cSubmissionCompleted = "SubmissionCompleted";

        private const string cMailTo = "MailTo";
        private const string cMailFrom = "MailFrom";
        private const string cMailSubject = "MailSubject";
        #endregion "Internal Constants"

        #region "Controller Actions"
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Home/PersonalDetails
        [HttpGet()]
        public ActionResult PersonalDetails()
        {
            //Check if a Submission has already been done
            if (SessionIsComplete())
                return View("SubmissionCompleted");

            if (this.Session[cPersonalDetails] != null)
                return View(this.Session[cPersonalDetails] as Models.PersonalDetailsModel);

            return View();
        }

        //
        // POST: /Home/PersonalDetails
        [HttpPost()]
        public ActionResult PersonalDetails(Models.PersonalDetailsModel submittedPersonalDetails)
        {
            //Check if a Submission has already been done
            if (SessionIsComplete())
                return View("SubmissionCompleted");

            //Check if current Post details are valid.
            if (submittedPersonalDetails != null && this.ModelState.IsValid)
            {
                //Add the current PersonalDetails to the session.
                Session.Add(cPersonalDetails, submittedPersonalDetails);

                //Redirect users that have completed the form to "CoverDetails"
                return RedirectToAction("CoverDetails");
            }

            return View(submittedPersonalDetails);
        }

        //
        // GET: /Home/CoverDetails
        [HttpGet()]
        public ActionResult CoverDetails()
        {
            //Check if the current session has completed pre-req
            if (this.Session[cPersonalDetails] == null)
                return RedirectToAction("PersonalDetails");

            //Check if a Submission has already been done
            if (SessionIsComplete())
                return View("SubmissionCompleted");

            return View();
        }

        //
        // POST: /Home/CoverDetails
        [HttpPost()]
        public ActionResult CoverDetails(Models.CoverDetailsModel submittedCoverDetails)
        {
            //Check if the current session has completed pre-req
            if (this.Session[cPersonalDetails] == null)
                return RedirectToAction("PersonalDetails");

            //Check if a Submission has already been done
            if (SessionIsComplete())
                return View("SubmissionCompleted");

            //Check if current Post details are valid.
            if (submittedCoverDetails != null && this.ModelState.IsValid)
            {
                var personalDetails = Session[cPersonalDetails] as Models.PersonalDetailsModel;
                var mail = CreateMail(personalDetails, submittedCoverDetails);
                using (var mc = new System.Net.Mail.SmtpClient())
                {
                    mc.Send(mail);
                }
                CompleteSession();

                return View("SubmissionCompleted");
            }
            return View(submittedCoverDetails);
        }
        #endregion "Controller Actions"

        #region "Controller Internals"
        private static System.Net.Mail.MailMessage CreateMail(Models.PersonalDetailsModel personalDetails, Models.CoverDetailsModel coverDetails)
        {

            var mailTo = ConfigurationManager.AppSettings[cMailTo] ?? string.Empty;
            var mailFrom = ConfigurationManager.AppSettings[cMailFrom] ?? string.Empty;
            var mailSubject = ConfigurationManager.AppSettings[cMailSubject] ?? string.Empty;

            //Create a shorter objectRef for smaller code :)
            var attachFile = coverDetails.CurriculumVitae;

            //Create the Mail Object
            var mail = new System.Net.Mail.MailMessage(mailFrom, mailTo);
            mail.Subject = mailSubject;
            mail.IsBodyHtml = false;
            mail.Body = CreateMailBody(personalDetails, coverDetails);
            mail.Attachments.Add(new System.Net.Mail.Attachment(attachFile.InputStream, attachFile.FileName));

            return mail;
        }

        private static string CreateMailBody(Models.PersonalDetailsModel personalDetails, Models.CoverDetailsModel coverDetails)
        {
            //Gonna do a lot of append stuff for building a text email,
            // so totally not gonna do "string += string"
            var sb = new System.Text.StringBuilder();

            sb.AppendLineFormat("New submission from \"{0} {1}\"", personalDetails.FirstName, personalDetails.LastName);
            sb.AppendLine();
            sb.AppendLineFormat("Email address: {0}", personalDetails.EmailAddress);
            sb.AppendLineFormat("Primary phone Number: {0}", personalDetails.PrimaryPhoneNumber);
            sb.AppendLine();
            sb.AppendLine("Reason for applying:");
            sb.AppendLine(coverDetails.ReasonForApplying);
            sb.AppendLine();
            sb.AppendLine("Why should we hire you:");
            sb.AppendLine(coverDetails.ReasonForAcceptance);

            return sb.ToString();
        }

        /// <summary>
        /// Sets the ViewBag.FirstName & ViewBag.AlreadyCompleted properties, returns ViewBag.AlreadyCompleted property.
        /// </summary>
        /// <returns></returns>
        private bool SessionIsComplete()
        {
            var sessionIsComplete = this.Session[cSubmissionCompleted] != null && (bool)this.Session[cSubmissionCompleted];
            var personalDetails = Session[cPersonalDetails] as Models.PersonalDetailsModel;
            ViewBag.FirstName = string.Empty;

            if (personalDetails != null)
                ViewBag.FirstName = personalDetails.FirstName;

            ViewBag.AlreadyCompleted = sessionIsComplete;

            return sessionIsComplete;
        }

        private void CompleteSession()
        {
            this.Session[cSubmissionCompleted] = true;
        }

        #endregion
    }
}