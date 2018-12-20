using Newtonsoft.Json;
using SendGridEmailApplication.Common;
using SendGridEmailApplication.Factory;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using System;
using System.Net;
using System.Net.Http;
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
        INotificationSender m_NotificationSenderType = null;
        IEmailSender emailSender = null;
        ISmsSender smsSender = null;

        EmailSenderFactory emailSenderFactory = null;
        SmsSenderFactory smsSenderFactory = null;
        NotificationSenderFactory m_NotificationFactory = null;  

        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationController()
        {
            m_NotificationFactory = new NotificationSenderFactory();
            emailSenderFactory = new EmailSenderFactory();
            smsSenderFactory = new SmsSenderFactory();
        }

        //[HttpPost]
        //[Route("api/notification/{provider}")]
        //public async Task<HttpResponseMessage> SendEmail([FromUri]NotificationType notificationType, EmailContract contract)
        //{
        //    m_NotificationSenderType = null;
        //    m_NotificationSenderType = m_NotificationFactory.NotificationSender(notificationType);
        //    try
        //    {
        //        await this.m_NotificationSenderType.SendEmail(contract);
        //        var message = Request.CreateResponse(HttpStatusCode.OK);
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //        return message;
        //    }
        //}

        /// <summary>
        /// Method to Send Email 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="contract"></param>
        /// <returns>Task<HttpResponseMessage></returns>
        //[HttpPost]
        //[Route("api/email/{provider}")]
        //public async Task<HttpResponseMessage> SendEmail([FromUri]EmailProviders provider, [FromBody]EmailContract contract)
        //{
        //    emailSender = null;
        //    emailSender = emailSenderFactory.EmailSender(provider);
        //    try
        //    {
        //        await this.emailSender.SendEmail(contract);
        //        var message = Request.CreateResponse(HttpStatusCode.OK);
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //        return message;
        //    }
        //}

        /// <summary>
        /// Method to Send SMS
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="contract"></param>
        /// <returns>HttpResponseMessage</returns>
        //[HttpPost]
        //[Route("api/sms/{provider}")]
        //public async Task<HttpResponseMessage> SendSms([FromUri]SmsProviders provider, SmsContract contract)
        //{
        //    smsSender = null;
        //    smsSender = smsSenderFactory.SmsSender(provider);
        //    try
        //    {
        //        await this.smsSender.SendSms(contract);
        //        var message = Request.CreateResponse(HttpStatusCode.OK);
        //        return message;
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //        return message;
        //    }
        //}

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

            try
            {
                AttachmentUpload.UploadAttachment(httpRequest);
                await emailSender.SendEmail(contract);
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
