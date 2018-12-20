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
    public class SmsSenderFactory
    {
        public ISmsSender SmsSender(SmsProviders provider)
        {
            ISmsSender smsSender = null;

            switch (provider)
            {
                case SmsProviders.Twillio:
                    smsSender = SMSNotificationService.InstanceCreation;
                    break;
                              
                default:
                    throw new ArgumentException("Invalid notification type");
            }

            return smsSender;
        }
    }
}