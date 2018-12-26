using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SendGridEmailApplication.Common;
using SendGridEmailApplication.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SendGridEmailApplication.Tests.Common
{
    [TestClass]
    public class SendGridEmailServiceTest
    {
        private Mock<SendGridEmailService> _sendGridEmailService;

        #region Initialize and Setup Methods

        [TestInitialize]
        public void Initialize()
        {
            _sendGridEmailService = new Mock<SendGridEmailService>();
        }

        #endregion

        [TestMethod]
        public void SendEmail()
        {
            //Arrange
            var contract = GetEmailContract();
            var attachments = GetAttachments();
            _sendGridEmailService.Setup(x => x.SendEmail(contract, attachments)).Returns(GetResponse());

            //Act
            var response = _sendGridEmailService.Object.SendEmail(contract, attachments);

            //Assert
        }

        #region Private
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

        private List<AttachmentContract> GetAttachments()
        {
            List<AttachmentContract> attachments = new List<AttachmentContract>
            {
                new AttachmentContract
                {
                    
                }
            };
            return attachments;
        }

        private Task<HttpResponseMessage> GetResponse()
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }
        #endregion
    }
}
