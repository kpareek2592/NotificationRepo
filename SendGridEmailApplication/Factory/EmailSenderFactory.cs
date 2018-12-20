using SendGridEmailApplication.Common;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SendGridEmailApplication.Factory
{
    public class EmailSenderFactory
    {
        public IEmailSender EmailSender(EmailProviders providers)
        {
            IEmailSender emailSender = null;

            switch (providers)
            {
                case EmailProviders.SendGrid:
                    emailSender = SendGridEmailService.InstanceCreation;
                    break;
                             
                default:
                    throw new ArgumentException("Invalid notification type");
            }

            return emailSender;
        }
    }
}