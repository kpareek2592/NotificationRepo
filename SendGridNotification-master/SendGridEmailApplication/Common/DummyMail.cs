using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridEmailApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SendGridEmailApplication.Common
{
    public class DummyMail
    {
        public static async Task SendDummyMail(EmailContract contract)
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage();
            var from = new EmailAddress("Kaushal.Pareek@email.com", "Kaushal Pareek");
            

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("Kaushal.Pareek@email.com", "Kaushal Pareek"),
                Subject = "Hello World from the SendGrid CSharp Library Helper!",
                PlainTextContent = "Hello, Email from the helper [SendSingleEmailAsync]!",
                HtmlContent = "<strong>Hello, Email from the helper! [SendSingleEmailAsync]</strong>"
            };

            var tos = new List<EmailAddress>
            {
                //new EmailAddress("kpareek2592@gmail.com", "Kaushal"),
                new EmailAddress("surinder.kumar@happiestminds.com", "Surinder"),
                new EmailAddress("test3@example.com", "Example User3")
            };
            msg.AddTos(tos);

            //msg.SetBccSetting(true, "kaushal.pareek@happiestminds.com");

            var cc_emails = new List<EmailAddress>
            {
                new EmailAddress("kaushal.pareek@happiestminds.com", "Kaushal"),
                new EmailAddress("test6@example.com", "Example User6")
            };
            msg.AddCcs(cc_emails);

            //msg.AddBcc(new EmailAddress("snehi.raj@happiestminds.com", "Snehi Raj"));
            //msg.AddBcc(new EmailAddress("kpareek2592@gmail.com", "Kaushal"));
            var bcc_emails = new List<EmailAddress>
            {
                new EmailAddress("snehi.raj@happiestminds.com", "Snehi"),
                new EmailAddress("kpareek2592@gmail.com", "Kaushal Pareek")
            };

            using (var fileStream = File.OpenRead(@"D:\TestData\a1.txt"))
            {
                await msg.AddAttachmentAsync("a1.txt", fileStream);
            }

            var subject = "Sending with SendGrid is Fun";
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var showAllRecipients = true; // Set to true if you want the recipients to see each others email addresses

            var msg1 = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                                                                       tos,
                                                                       subject,
                                                                       plainTextContent,
                                                                       htmlContent,
                                                                       showAllRecipients
                                                                       );

            
            var response = await client.SendEmailAsync(msg);
        }

    }
}