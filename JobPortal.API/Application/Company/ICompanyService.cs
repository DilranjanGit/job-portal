using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using JobPortal.API.DTOs;

namespace JobPortal.API.Application.Company
{
    public interface ICompanyService
    {
        // add	Registration 

        Task<bool> RegisterAsync(string fullName, string email, string location, string webUrl, string password, CancellationToken cancellationToken=default);
        Task<bool> PostToJobAsync(string email, string jobTitle,string description, string location, string skillsCsv, decimal salary, CancellationToken cancellationToken=default);
        Task<Message[]> GetMessagesAsync(string studentEmail, CancellationToken cancellationToken=default);
        Task<bool> SendMessageAsync(string fromEmail, string toEmail, string content, CancellationToken cancellationToken=default);
        Task<CompanyProfile> GetCompanyProfileAsync(string companyEmail, CancellationToken cancellationToken=default);
        Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobId, CancellationToken cancellationToken=default);
        Task<bool> ScheduleInterviewAsync(int jobApplicationId, DateTime interviewDate,string locationOrLink, int mode, CancellationToken cancellationToken = default);
    }
}       