using SendGridEmailApplication.Factory;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Twilio.Types;

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
            var myregex = new Regex(@"(,;)", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            smsSender = null;
            smsSender = smsSenderFactory.SmsSender(provider);

            if (!string.IsNullOrEmpty(contract.ToPhoneNumber))
            {
                var tos = new List<PhoneNumber>();
                var split_To = contract.ToPhoneNumber.Split(',', ';');
                foreach (var to in split_To)
                {
                    bool isPhoneNumber = Regex.IsMatch(to,
                    @"^([0|\+[0-9]{1,5})?([7-9][0-9]{9})$",
                    RegexOptions.IgnoreCase);
                    if (!isPhoneNumber)
                    {
                        var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Phone Number.");
                        return message;
                    }
                }
            }
            if (string.IsNullOrEmpty(contract.ToPhoneNumber))
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Atleast one receipient number is required.");
                return message;
            }
            if (string.IsNullOrEmpty(contract.Body))
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body is empty.");
                return message;
            }
            try
            {
                smsSender.SendSms(contract);
                var message = Request.CreateResponse(HttpStatusCode.OK, "Message Sent");
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
