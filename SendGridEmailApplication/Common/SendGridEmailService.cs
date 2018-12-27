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
using System.Linq;

namespace SendGridEmailApplication.Common
{
    /// <summary>
    /// Service to send email using SendGrid
    /// </summary>
    public class SendGridEmailService : IEmailSender
    {
        //private SendGridEmailService _sendGridEmailService;
        private SendGridClient _client;
        private readonly string apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
        DummyController dummyController = new DummyController();

        /// <summary>
        /// Constructor for initializing SendGrid Instance
        /// </summary>
        public SendGridEmailService()
        {
            
            //TODO :  Move to Initialize method
        }

        private void Initialize()
        {
            _client = new SendGridClient(apikey);
        }

        /// <summary>
        /// Method to send email using SendGrid
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="attachments"></param>
        public async Task SendEmail(EmailContract contract, List<AttachmentContract> attachments)
        {
            try
            {


                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(contract.From),
                    Subject = contract.Subject,
                    HtmlContent = contract.Body,
                };

                if (!string.IsNullOrWhiteSpace(contract.ToEmailAddress))
                {
                    var split_To = SplitEmail(contract.ToEmailAddress);

                    var toos = split_To.Select(toEmail => new EmailAddress(toEmail)).ToList();
                    msg.AddTos(toos);
                }

                if (!string.IsNullOrWhiteSpace(contract.CcEmailAddress))
                {
                    var split_Cc = SplitEmail(contract.CcEmailAddress);
                    var ccs = split_Cc.Select(ccEmail => new EmailAddress(ccEmail)).ToList();
                    msg.AddCcs(ccs);
                }

                if (!string.IsNullOrWhiteSpace(contract.BccEmailAddress))
                {
                    var split_Bcc = SplitEmail(contract.BccEmailAddress);
                    var bccs = split_Bcc.Select(bccEmail => new EmailAddress(bccEmail)).ToList();
                    msg.AddBccs(bccs);
                }

                //if (httpRequest.Files.Count > 0)
                //{
                //var docfiles = new List<string>();
                //foreach (string files in httpRequest.Files)
                //{
                //    var postedFile = httpRequest.Files[files];
                //    var data = postedFile.InputStream;

                //    StreamReader reader = new StreamReader(data);
                foreach (var item in attachments)
                {
                    //byte[] bytedata = System.Text.Encoding.Default.GetBytes(item);
                    string strBase64 = Convert.ToBase64String(item.fileBytes);
                    msg.AddAttachment(item.FileName, strBase64);
                }

                Initialize();
                //Sending the email
                var response = await _client.SendEmailAsync(msg);
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