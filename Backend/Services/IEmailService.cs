using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IEmailService
    {
        Task<Result<bool, string>> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        Task<Result<bool, string>> SendTemplatedEmailAsync(string to, string subject, string templateName, object model);
    }
}
