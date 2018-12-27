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
    public class SmsControllerTest
    {
        private SmsController _controller;
        private Mock<SmsController> _mockController;
        private Mock<ISmsSender> _smsSender;

        #region Initialize and Setup Methods

        [TestInitialize]
        public void Initialize()
        {
            _controller = new SmsController();
            _mockController = new Mock<SmsController>();
            _smsSender = new Mock<ISmsSender>();
        }

        #endregion

        [TestMethod]
        public async Task SendSms_Status_OK_Test()
        {
            //Arrange
            var contract = GetSmsContract();
            var expectedResult = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK };

            var smsProvider = It.IsAny<string>();
            var smsLength = It.IsAny<string>();
            var fromNumber = It.IsAny<string>();
            smsProvider = SmsProviders.Twilio.ToString();

            _mockController.Object.Request = new HttpRequestMessage();
            _mockController.Object.Request.SetConfiguration(new System.Web.Http.HttpConfiguration());

            //Act
            var actualResult = await _mockController.Object.SendSms(contract);

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
        }

        [TestMethod]
        public async Task SendSms_Exception_Test()
        {
            //Arrange
            var contract = GetSmsContract();
            contract.ToPhoneNumber = "123";
            var expectedResult = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest};

            var smsProvider = It.IsAny<string>();
            var smsLength = It.IsAny<string>();
            var fromNumber = It.IsAny<string>();
            smsProvider = SmsProviders.Twilio.ToString();

            _mockController.Object.Request = new HttpRequestMessage();
            _mockController.Object.Request.SetConfiguration(new System.Web.Http.HttpConfiguration());

            //Act
            var actualResult = await _mockController.Object.SendSms(contract);

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
        }

        [TestMethod]
        public async Task SendSms_MessageSizeExceeded_Test()
        {
            //Arrange
            var contract = GetSmsContract();
            contract.Body = "You discover what it is like to be hungry. " +
                "With bread and margarine in your belly, you go out and look into the shop windows. " +
                "Everywhere there is food insulting you in huge, wasteful piles; whole dead pigs, " +
                "baskets of hot loaves, great yellow blocks of butter, strings of sausages, mountains of " +
                "potatoes, vast Gruyère cheeses like grindstones. A snivelling self-pity comes over you at the sight of so much food. " +
                "You plan to grab a loaf and run, swallowing it before they catch you; and you refrain, from pure funk." +
                "The flavor that salt imparts to food is just one of the attributes that manufacturers rely on. " +
                "For them, salt is nothing less than a miracle worker in processed foods. It makes sugar taste sweeter. It adds crunch to crackers and frozen waffles. " +
                "It delays spoilage so that the products can sit longer on the shelf. And, just as importantly, " +
                "it masks the otherwise bitter or dull taste that hounds so many processed foods before salt is added." +
                "You discover what it is like to be hungry. " +
                "With bread and margarine in your belly, you go out and look into the shop windows. " +
                "Everywhere there is food insulting you in huge, wasteful piles; whole dead pigs, " +
                "baskets of hot loaves, great yellow blocks of butter, strings of sausages, mountains of " +
                "potatoes, vast Gruyère cheeses like grindstones. A snivelling self-pity comes over you at the sight of so much food. " +
                "You plan to grab a loaf and run, swallowing it before they catch you; and you refrain, from pure funk." +
                "The flavor that salt imparts to food is just one of the attributes that manufacturers rely on. " +
                "For them, salt is nothing less than a miracle worker in processed foods. It makes sugar taste sweeter. It adds crunch to crackers and frozen waffles. " +
                "It delays spoilage so that the products can sit longer on the shelf. And, just as importantly, " +
                "it masks the otherwise bitter or dull taste that hounds so many processed foods before salt is added.";
            var expectedResult = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest };

            var smsProvider = It.IsAny<string>();
            var smsLength = It.IsAny<string>();
            var fromNumber = It.IsAny<string>();
            smsProvider = SmsProviders.Twilio.ToString();

            _mockController.Object.Request = new HttpRequestMessage();
            _mockController.Object.Request.SetConfiguration(new System.Web.Http.HttpConfiguration());

            //Act
            var actualResult = await _mockController.Object.SendSms(contract);

            //Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
        }

        #region Private
        private SmsContract GetSmsContract()
        {
            return new SmsContract()
            {
                Body = "This is a test sms sent via twilio",
                ToPhoneNumber = "+919529625298"
            };
        }

        private Task<HttpResponseMessage> GetResponse()
        {
            return Task.Run(() => new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }
        #endregion
    }
}
