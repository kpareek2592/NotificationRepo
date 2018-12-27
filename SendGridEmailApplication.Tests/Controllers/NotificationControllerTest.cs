using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private Mock<INotificationSender> _notificationSender;
        private Mock<NotificationController> _mockController;

        [TestInitialize]
        public void Init()
        {
            _mockController = new Mock<NotificationController>();
            _notificationSender = new Mock<INotificationSender>();
        }

        [TestMethod]
        public async Task SendEmail_Test1()
        {
            //Arrange
            var contract = GetEmailContract();
            var expectedResult = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK };

            var emailProvider = It.IsAny<string>();
            var attachmentLength = It.IsAny<string>();
            emailProvider = EmailProviders.SendGrid.ToString();

            _mockController.Object.Request = new HttpRequestMessage();
            _mockController.Object.Request.SetConfiguration(new System.Web.Http.HttpConfiguration());

            //Act
            var actualResult = await _mockController.Object.SendEmail();

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
        }


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

        private Task<HttpResponseMessage> GetResponse()
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        #endregion
    }
}
