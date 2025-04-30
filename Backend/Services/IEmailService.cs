using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendTemplatedEmailAsync(string to, string subject, string templateName, object model);
    }
}
