using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using SendGridEmailApplication.Controllers;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;
using SendGridEmailApplication.Models.Enums;
using System.Net.Http;
using System.Threading.Tasks;

namespace SendGridEmailApplication.Tests.Controllers
{
    [TestClass]
    public class NotificationControllerTest
    {
        private Mock<EmailContract> _emailContract;
        private Mock<INotificationSender> _notificationSender;
        private Mock<NotificationController> _controller;

        [TestInitialize]
        public void Init()
        {
            _controller = new Mock<NotificationController>();
            _emailContract = new Mock<EmailContract>();
            _notificationSender = new Mock<INotificationSender>();
        }

        public void SendEmail_Test1()
        {
            //Arrange
            var _mockController = new NotificationController();

            //var _controller = _mockController.Object;
            _controller.Setup(x => x.SendEmail()).Returns(GetResponse());

            //Act
            var result = _mockController.SendEmail();

            //Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
        }

        //[TestCase(SmsProviders.Twillio)]
        //public void SendSms_Test1(SmsProviders providers)
        //{
        //    //Arrange
        //    var _mockController = new NotificationController();

        //    var smsContract = GetSmsContract();

        //    //var _controller = _mockController.Object;
        //    _controller.Setup(x => x.SendSms(providers, smsContract)).Returns(GetResponse());


        //    //Act
        //    var result = _mockController.SendSms(providers, smsContract);

        //    //Assert
        //    NUnit.Framework.Assert.IsNotNull(result);
        //}

        #region Private Field
        private EmailContract GetEmailContract()
        {
            return new EmailContract()
            {
              From = "Kaushal.Pareek@email.com",
              Subject = "Test Email from Send Grid",
              Body = "This is a test email sent via test grid",
              ToEmailAddress = "surinder.kumar@happiestminds.com,test3@example.com",
              CcEmailAddress = "kpareek2592@gmail.com;test6@example.com",
              BccEmailAddress = "snehi.raj@happiestminds.com,kaushal.pareek@happiestminds.com"
            };
        }

        private SmsContract GetSmsContract()
        {
            return new SmsContract()
            {
                Body = "This is a test sms sent via twilio",
                ToPhoneNumber = "1234567"
            };
        }

        private Task<HttpResponseMessage> GetResponse()
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        #endregion
    }
}
