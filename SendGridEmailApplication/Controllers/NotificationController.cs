using Newtonsoft.Json;
using SendGridEmailApplication.Factory;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using SendGridEmailApplication.Validation;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SendGridEmailApplication.Controllers
{
    /// <summary>
    /// Class for sending notification for email and sms
    /// </summary>
    public class NotificationController : ApiController
    {
        IEmailSender emailSender = null;
        EmailSenderFactory emailSenderFactory = null;
        EmailValidation validation = new EmailValidation();

        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationController()
        {
            emailSenderFactory = new EmailSenderFactory();
        }

        /// <summary>
        /// Method to upload an attachment and Send Email
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("api/sendmail/{provider}")]
        public async Task<HttpResponseMessage> SendEmail([FromUri]EmailProviders provider)
        {
            emailSender = null;
            emailSender = emailSenderFactory.EmailSender(provider);

            var httpRequest = HttpContext.Current.Request;
            var stringContract = HttpContext.Current.Request.Params["content"];
            EmailContract contract = JsonConvert.DeserializeObject<EmailContract>(stringContract);

            long contentSize = httpRequest.ContentLength;
            if (contentSize > 16777216)
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File upload limit should not exceed 17 MB");
                return message;
            }

            #region Validation

            if (string.IsNullOrEmpty(contract.From))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "From Email Address is required");
                return response;
            }
            if (string.IsNullOrEmpty(contract.ToEmailAddress))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "To Email Address is required");
                return response;
            }

            if (!string.IsNullOrEmpty(contract.From))
            {
                bool isEmail = Regex.IsMatch(contract.From,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Email Address");
                    return response;
                }
            }

            #endregion

            try
            {
                //Validate(contract);
                await emailSender.SendEmail(contract, httpRequest);
                var message = Request.CreateResponse(HttpStatusCode.OK);
                return message;
            }
            catch (Exception ex)
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                return message;
            }
        }

        public void Validate(EmailContract contract)
        {
            if (string.IsNullOrEmpty(contract.From))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "From Email Address is required");
                throw new HttpResponseException(response);
            }
            if (string.IsNullOrEmpty(contract.ToEmailAddress))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "To Email Address is required");
                throw new HttpResponseException(response);
            }

            if (!string.IsNullOrEmpty(contract.From))
            {
                bool isEmail = Regex.IsMatch(contract.From,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Email Address");
                    throw new HttpResponseException(response);
                }
            }
        }
    }
}
