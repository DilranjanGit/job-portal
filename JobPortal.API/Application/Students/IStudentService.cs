using JobPortal.API.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace JobPortal.API.Application.Students
{
    public interface IStudentService
    {
        Task<StudentProfile?> GetByUserEmailAsync(string email, CancellationToken cancellationToken=default);
        Task<StudentProfile> UpsertResumeAsync(string fullName,string email, string phoneNumber, string education, string? resumeText, string[]? skills, CancellationToken cancellationToken=default);
        Task<bool> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken=default);
        Task<bool> ApplyToJobAsync(string studentEmail, string jobId, CancellationToken cancellationToken=default);
        Task<Message[]> GetMessagesAsync(string studentEmail, CancellationToken cancellationToken=default);
        Task<bool> SendMessageAsync(string fromEmail, string toEmail, string content, CancellationToken cancellationToken=default);
    }
}