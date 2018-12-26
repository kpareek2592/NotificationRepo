using SendGridEmailApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SendGridEmailApplication.Interface
{
    public interface IEmailSender
    {
        Task SendEmail(EmailContract contract, List<AttachmentContract> attachments); 
    }
}