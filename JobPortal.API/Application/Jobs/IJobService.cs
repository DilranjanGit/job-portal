using Humanizer;
using JobPortal.API.Domain;
using JobPortal.API.DTOs;

namespace JobPortal.API.Application.Jobs
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(string companyEmail,string title, string description, string location, decimal salary, List<string>? skills, CancellationToken ct = default);
        Task<Job?> GetJobByIdAsync(int jobId, CancellationToken ct = default);
        Task<IEnumerable<JobDto>> GetAllJobsAsync(string location="", int studentId=0, CancellationToken ct = default);
        Task<IEnumerable<JobDto>> GetAllJobsByCompanyAsync(int companyId, CancellationToken ct = default);
        Task<bool> UpdateApplicationStatusAsync(int jobApplicationId, ApplicationStatus status, CancellationToken ct = default);
    }

}