using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IEmailService
    {
        Task<Result<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        Task<Result<bool>> SendTemplatedEmailAsync(string to, string subject, string templateName, object model);
    }
}
