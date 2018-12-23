using SendGridEmailApplication.Controllers;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class SMSNotificationService: ISmsSender
    {
        //private volatile static SendGridEmailService sendGridEmailService;
        private static SMSNotificationService m_smsNotificationService;
        DummyController dummyController = new DummyController();
        private SMSNotificationService() { }

        public static SMSNotificationService InstanceCreation
        {
            get
            {
                if (m_smsNotificationService == null)
                {
                    m_smsNotificationService = new SMSNotificationService();
                }
                return m_smsNotificationService;
            }
        }

        /// <summary>
        /// Method to send email
        /// </summary>
        /// <param name="contract"></param>
        public async void SendSms(SmsContract contract)
        {
            // Use your account SID and authentication token instead
            // of the placeholders shown here.

            var accountSID = ConfigurationManager.AppSettings["accountSID"];
            var authToken = ConfigurationManager.AppSettings["authToken"];
            
            // Initialize the TwilioClient.
            TwilioClient.Init(accountSID, authToken);
            TwilioRestClient twilio = new TwilioRestClient(accountSID, authToken);

            try
            {
                // Send an SMS message.
                //var message = MessageResource.Create(
                //    to: new PhoneNumber("+919529625298"),
                //    //to: new PhoneNumber("+919343538518"),
                //    from: new PhoneNumber("+13022020271"),
                //    body: "This is my SMS message.");

                var tos = new List<PhoneNumber>();
                var split_To = contract.ToPhoneNumber.Split(',', ';');
                foreach (var to in split_To)
                {
                    dummyController.ValidatePhoneNumber(to, contract.ToPhoneNumber);

                    MessageResource.Create(
                    to: new PhoneNumber(to),
                    from: new PhoneNumber(contract.From),
                    body: contract.Body);
                }
            }
            catch (TwilioException ex)
            {
                // An exception occurred making the REST call
                Console.WriteLine(ex.Message);
            }
        }
    }
}