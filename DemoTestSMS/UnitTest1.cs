using System.Net.Http;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendGridEmailApplication.Controllers;
using SendGridEmailApplication.Models;

namespace DemoTestSMS
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Test_DemoSendSms()
        {
            //Arrange
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:56696/api/sms");
            //var mockSmsController = new Mock<SmsController> { CallBase = true };
            //mockSmsController.Object.Request = request;
            var contract = GetSmsContract();

            //using (var response = mockSmsController.Object.SendSms(contract))
            //{
            //    //Assert (Verify correct parameters are passed to Registration and response is no content)
            //    mockSmsController.Setup(x => x.SendSms(contract)).Returns(GetResponse());
            //    //Assert.AreEqual(HttpStatusCode, response.StatusCode);
            //}
            using (var mock = AutoMock.GetLoose())
            {
             mock.Mock<SmsController>()
                    .Setup(x => x.SendSms(contract))
                    .Returns(GetResponse());

                var cls = mock.Create<SmsController>();

                var expected = await GetResponse();
                var actual = await cls.SendSms(contract);

                Assert.IsTrue(actual != null);
            }

            // _mockController.Setup(x => x.SendSms(contract)).Returns(GetResponse());

            //Act
            //await _controller.SendSms(contract);

            //Assert

        }
        #region Private
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
            return Task.Run(() => new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }
        #endregion
    }
}
