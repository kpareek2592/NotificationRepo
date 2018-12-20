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
    public class NotificationSenderFactory
    {
        //SendGridEmailService sendGridEmailService = SendGridEmailService.InstanceCreation;
        public INotificationSender NotificationSender(NotificationType notificationType)
        {
            INotificationSender notificationSender = null;

            //switch (notificationType)
            //{
            //    case NotificationType.Email:
            //        notificationSender = SendGridEmailService.InstanceCreation;
            //        break;
            //    case NotificationType.SMS:
            //        notificationSender = SMSNotificationService.InstanceCreation;
            //        break;               
            //    default:
            //        throw new ArgumentException("Invalid notification type");
            //}

            return notificationSender;
        }
    }
}