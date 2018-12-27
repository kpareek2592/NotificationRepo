using SendGridEmailApplication.Controllers;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SendGridEmailApplication.Common
{
    /// <summary>
    /// Class for sending Sms Notification
    /// </summary>
    public class TwilioSmsService : ISmsSender
    {
        private TwilioRestClient _twilioClient;
        private string accountSID = ConfigurationManager.AppSettings["accountSID"];
        private string authToken = ConfigurationManager.AppSettings["authToken"];

        /// <summary>
        /// Method to initialize Twilio instance
        /// </summary>
        private void Initializer()
        {
            TwilioClient.Init(accountSID, authToken);
            _twilioClient = new TwilioRestClient(accountSID, authToken);
        }
        /// <summary>
        /// Method to send email
        /// </summary>
        /// <param name="contract"></param>
        public async Task SendSms(SmsContract contract)
        {
            Initializer();
            var fromNumber = ConfigurationManager.AppSettings["From"];
            List<Task<MessageResource>> tasks = new List<Task<MessageResource>>();
           
                var tos = new List<PhoneNumber>();
                var split_To = contract.ToPhoneNumber.Split(',', ';');
                foreach (var to in split_To)
                {
                    var res = Task.Run(() => MessageResource.CreateAsync(to: new PhoneNumber(to),
                        from: new PhoneNumber(fromNumber),
                        body: contract.Body));

                    tasks.Add(res);
                }
                    var results = await Task.WhenAll(tasks);

                foreach (var item in results)
                {
                    if (item.Status == MessageResource.StatusEnum.Undelivered)
                    {
                        //Logging
                    }
                }
            
        }
    }
}