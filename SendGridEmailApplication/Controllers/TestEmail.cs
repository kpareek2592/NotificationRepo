using Newtonsoft.Json;
using SendGridEmailApplication.Common;
using SendGridEmailApplication.Factory;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using SendGridEmailApplication.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    public class TestEmailController : ApiController
    {
        IEmailSender emailSender = null;
        EmailSenderFactory emailSenderFactory = null;
        EmailValidation validation = new EmailValidation();
        DummyController dummyController = new DummyController();
        private TestEmailService _testEmailService;
        /// <summary>
        /// Constructor
        /// </summary>
        public TestEmailController()
        {
            _testEmailService = new TestEmailService();
        }

        //[HttpPost]
        //[Route("api/SendGrid")]
        ////[ValidateInput(false)]
        //public HttpResponseMessage SendGrid()
        //{
        //    System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
        //    string rawSendGridJSON = reader.ReadToEnd();
        //    List<SendGridEvents> sendGridEvents = JsonConvert.DeserializeObject<List<SendGridEvents>>(rawSendGridJSON);
        //    string count = sendGridEvents.Count.ToString();
        //    System.Diagnostics.Trace.TraceError(rawSendGridJSON); // For debugging to the Azure Streaming logs
        //    foreach (SendGridEvents sendGridEvent in sendGridEvents)
        //    {
        //        // Here is where you capture the event data
        //        System.Diagnostics.Trace.TraceError(sendGridEvent.email); // For debugging to the Azure Streaming logs
        //    }
        //    return new HttpStatusCodeResult(200);
        //}

        /// <summary>
        /// Method to upload an attachment and Send Email
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("api/sendmail1")]
        public async Task<HttpResponseMessage> SendEmail()
        {
            var emailProvider = ConfigurationManager.AppSettings["EmailProvider"];
            var attachmentLength = ConfigurationManager.AppSettings["AttachmentLength"];
            EmailProviders provider = (EmailProviders)Enum.Parse(typeof(EmailProviders), emailProvider);
            emailSender = null;
            //emailSender = emailSenderFactory.EmailSender(provider);
            #region trying
            var result = await Request.Content.ReadAsMultipartAsync();

            var requestJson = await result.Contents[0].ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<EmailContract>(requestJson);
            List<AttachmentContract> list = new List<AttachmentContract>();

            if (result.Contents.Count > 1)
            {
                for (int i = 1; i < result.Contents.Count; i++)
                {
                    var fileByteArray = await result.Contents[i].ReadAsByteArrayAsync();
                    var fileName = result.Contents[i].Headers.ContentDisposition.FileName.Replace("\"", "");
                    var contentType = result.Contents[i].Headers.ContentType.MediaType;
                    list.Add(new AttachmentContract { fileBytes = fileByteArray, FileName = fileName, ContentType = contentType});
                    
                }
            }

            #endregion
            //var httpRequest = HttpContext.Current.Request;
            //var stringContract = HttpContext.Current.Request.Params["content"];
            //EmailContract contract = JsonConvert.DeserializeObject<EmailContract>(stringContract);

            
            //dummyController.ParseString(contract.Body, nameof(contract.Body));
            //dummyController.ParseString(contract.From, nameof(contract.From));

            //dummyController.ValidateParameterForNull(contract.From, contract.From);
            //dummyController.ValidateParameterForNull(contract.ToEmailAddress, contract.ToEmailAddress);
            //contract.ToEmailAddress = dummyController.ValidateEmail1(contract.ToEmailAddress);

            try
            {
                var response = _testEmailService.SendEmail(request, list);
                var message = Request.CreateResponse(HttpStatusCode.OK);
                return message;
            }
            catch (Exception ex)
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                return message;
            }
        }

        [HttpPost]
        [Route("api/inbound")]
        public async Task<HttpResponseMessage> Post()
        {
            var root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            var email = new Email
            {
                Dkim = provider.FormData.GetValues("dkim").FirstOrDefault(),
                To = provider.FormData.GetValues("to").FirstOrDefault(),
                Html = provider.FormData.GetValues("html").FirstOrDefault(),
                From = provider.FormData.GetValues("from").FirstOrDefault(),
                Text = provider.FormData.GetValues("text").FirstOrDefault(),
                SenderIp = provider.FormData.GetValues("sender_ip").FirstOrDefault(),
                Envelope = provider.FormData.GetValues("envelope").FirstOrDefault(),
                Attachments = int.Parse(provider.FormData.GetValues("attachments").FirstOrDefault()),
                Subject = provider.FormData.GetValues("subject").FirstOrDefault(),
                Charsets = provider.FormData.GetValues("charsets").FirstOrDefault(),
                Spf = provider.FormData.GetValues("spf").FirstOrDefault()
            };

            // The email is now stored in the email variable

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
