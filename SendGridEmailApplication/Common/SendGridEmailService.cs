using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridEmailApplication.Controllers;
using SendGridEmailApplication.Interface;
using SendGridEmailApplication.Models;

namespace SendGridEmailApplication.Common
{
    /// <summary>
    /// Service to send email using SendGrid
    /// </summary>
    public class SendGridEmailService : IEmailSender
    {
        private static SendGridEmailService sendGridEmailService;
        DummyController dummyController = new DummyController();
        private SendGridEmailService(){ }

        /// <summary>
        /// Singleton instance creation
        /// </summary>
        public static SendGridEmailService InstanceCreation
        {            
            get
            {
                if (sendGridEmailService == null)
                {
                    sendGridEmailService = new SendGridEmailService();
                }
                return sendGridEmailService;
            }
        }

        /// <summary>
        /// Method to send email using SendGrid
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="httpRequest"></param>
        public async Task SendEmail(EmailContract contract, HttpRequest httpRequest) 
        {
            DummyController dummyController = new DummyController();
            try
            {
                var apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
                var client = new SendGridClient(apikey);

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(contract.From),
                    Subject = contract.Subject,
                    HtmlContent = contract.Body,
                };

                if (!string.IsNullOrWhiteSpace(contract.ToEmailAddress))
                {
                    var split_To = SplitEmail(contract.ToEmailAddress);
                    var toos = new List<EmailAddress>();
                    foreach (var toEmail in split_To)
                    {
                        dummyController.ValidateEmail(toEmail, "EmailInfo");

                        toos.Add(new EmailAddress(toEmail));
                    }
                    msg.AddTos(toos);
                }

                if (!string.IsNullOrWhiteSpace(contract.CcEmailAddress))
                {
                    var split_Cc = SplitEmail(contract.CcEmailAddress);
                    var ccs = new List<EmailAddress>();
                    foreach (var ccEmail in split_Cc)
                    {
                        dummyController.ValidateEmail(ccEmail, "EmailInfo");
                        ccs.Add(new EmailAddress(ccEmail));
                    }
                    msg.AddCcs(ccs); 
                }

                if (!string.IsNullOrWhiteSpace(contract.BccEmailAddress))
                {
                    var split_Bcc = SplitEmail(contract.BccEmailAddress);
                    var bccs = new List<EmailAddress>();
                    foreach (var bccEmail in split_Bcc)
                    {
                        dummyController.ValidateEmail(bccEmail, "EmailInfo");
                        bccs.Add(new EmailAddress(bccEmail));
                    }

                    msg.AddBccs(bccs); 
                }

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        if (DummyController.ValidateAttachment(postedFile.FileName))
                        {
                            Stream filestream = postedFile.InputStream;
                            await msg.AddAttachmentAsync(postedFile.FileName, filestream);
                        }
                    }
                }

                //Sending the email
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Method to split string based on ',' and ';'
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string[] SplitEmail(string email)
        {
            string[] split_emails = email.Split(new Char[] { ',', ';' });
            return split_emails;
        }
    }
}