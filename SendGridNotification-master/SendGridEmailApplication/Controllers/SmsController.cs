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
    public class SmsController : ApiController
    {
        ISmsSender smsSender = null;
        SmsSenderFactory smsSenderFactory = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SmsController()
        {
            smsSenderFactory = new SmsSenderFactory();
        }


        /// <summary>
        /// Method to Send SMS
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="contract"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("api/sms/{provider}")]
        public HttpResponseMessage SendSms([FromUri]SmsProviders provider, SmsContract contract)
        {
            smsSender = null;
            smsSender = smsSenderFactory.SmsSender(provider);
            try
            {
                smsSender.SendSms(contract);
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
