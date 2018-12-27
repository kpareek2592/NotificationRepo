using Newtonsoft.Json;
using SendGridEmailApplication.Factory;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using SendGridEmailApplication.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        EmailValidation validation = new EmailValidation();
        DummyController dummyController = new DummyController();
        EmailSenderFactory _emailSenderFactory = null;
        private IEmailSender _emailSender = null;

        /// <summary>
        /// Initializing Email Sender Factory
        /// </summary>
        private void Initialize()
        {
            _emailSenderFactory = new EmailSenderFactory();
        }


        /// <summary>
        /// Method to upload an attachment and Send Email
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("api/sendmail")]
        public async Task<HttpResponseMessage> SendEmail()
        {
            var emailProvider = ConfigurationManager.AppSettings["EmailProvider"];
            var attachmentLength = ConfigurationManager.AppSettings["AttachmentLength"];
            EmailProviders provider = (EmailProviders)Enum.Parse(typeof(EmailProviders), emailProvider);
            Initialize();
            _emailSender = _emailSenderFactory.EmailSender(provider);

            var result = await Request.Content.ReadAsMultipartAsync();

            var requestJson = await result.Contents[0].ReadAsStringAsync();
            var contract = JsonConvert.DeserializeObject<EmailContract>(requestJson);
            List<AttachmentContract> attachments = new List<AttachmentContract>();

            if (result.Contents.Count > 1)
            {
                for (int i = 1; i < result.Contents.Count; i++)
                {
                    var fileByteArray = await result.Contents[i].ReadAsByteArrayAsync();
                    var fileName = result.Contents[i].Headers.ContentDisposition.FileName.Replace("\"", "");
                    var contentType = result.Contents[i].Headers.ContentType.MediaType;
                    attachments.Add(new AttachmentContract { fileBytes = fileByteArray, FileName = fileName, ContentType = contentType });

                }
            }

            dummyController.ParseString(contract.Body, nameof(contract.Body));
            dummyController.ParseString(contract.From, nameof(contract.From));

            dummyController.ValidateParameterForNull(contract.From, contract.From);
            dummyController.ValidateParameterForNull(contract.ToEmailAddress, contract.ToEmailAddress);
            //contract.ToEmailAddress = dummyController.ValidateEmail1(contract.ToEmailAddress);

            try
            {
                //Validate(contract);
                await _emailSender.SendEmail(contract, attachments);
                var message = Request.CreateResponse(HttpStatusCode.OK);
                return message;
            }
            catch (Exception ex)
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                return message;
            }
        }
    }
}
