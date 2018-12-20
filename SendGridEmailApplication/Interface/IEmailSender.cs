using SendGridEmailApplication.Models;
using System.Threading.Tasks;
using System.Web;

namespace SendGridEmailApplication.Interface
{
    public interface IEmailSender
    {
        Task SendEmail(EmailContract contract, HttpRequest httpRequest); 
    }
}