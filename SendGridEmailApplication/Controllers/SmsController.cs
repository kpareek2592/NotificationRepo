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
using System.Configuration;

namespace SendGridEmailApplication.Controllers
{
    /// <summary>
    /// Class for sending notification for email and sms
    /// </summary>
    public class SmsController : ApiController
    {
        ISmsSender smsSender = null;
        SmsSenderFactory smsSenderFactory = null;
        DummyController dummyController = new DummyController();

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
        /// <param name="contract"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("api/sms")]
        public HttpResponseMessage SendSms(SmsContract contract)
        {
            var smsProvider = ConfigurationManager.AppSettings["SmsProvider"];
            var smsLength = ConfigurationManager.AppSettings["SmsLength"];
            var fromNumber = ConfigurationManager.AppSettings["From"];

            contract.From = fromNumber;
            SmsProviders provider = (SmsProviders)Enum.Parse(typeof(SmsProviders), smsProvider);
            smsSender = null;
            smsSender = smsSenderFactory.SmsSender(provider);


            dummyController.ValidateParameterForNull(contract.ToPhoneNumber, contract.ToPhoneNumber);
            dummyController.ValidateParameterForNull(contract.Body, contract.Body);
            dummyController.ValidateParameterForNull(contract.From, contract.From);
            dummyController.ParseString(contract.From, nameof(contract.From));
            dummyController.ParseString(contract.Body, nameof(contract.Body));

            if (contract.Body.Length > Convert.ToInt32(smsLength))
            {
                var message = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Message size only upto 1600 characters is allowed.");
                return message;
            }

            if (!string.IsNullOrEmpty(contract.ToPhoneNumber))
            {
                var tos = new List<PhoneNumber>();
                var split_To = contract.ToPhoneNumber.Split(',', ';');
                foreach (var to in split_To)
                {
                    dummyController.ParseString(to, nameof(to));

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
