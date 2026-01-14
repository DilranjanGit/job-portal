using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace JobPortal.API.Application.Company
{
    public interface ICompanyService
    {
        // add	Registration 

        Task<bool> RegisterAsync(string fullName, string email, string location, string webUrl, string password, CancellationToken cancellationToken=default);
        Task<bool> PostToJobAsync(string companyEmail, string jobTitle,string description, string location, string skillsCsv, decimal salary, CancellationToken cancellationToken=default);
        Task<Message[]> GetMessagesAsync(string studentEmail, CancellationToken cancellationToken=default);
        Task<bool> SendMessageAsync(string fromEmail, string toEmail, string content, CancellationToken cancellationToken=default);
	/*Login Module
•	Post Job Module
•	Interview Module
•	Send Message
*/
        
    }
}