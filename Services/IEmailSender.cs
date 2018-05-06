// IEmailSender
using System.Threading.Tasks;

namespace sklep.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}