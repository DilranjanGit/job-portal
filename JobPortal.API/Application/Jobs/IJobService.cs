using JobPortal.API.Domain;

namespace JobPortal.API.Application.Jobs
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(string companyEmail,string title, string description, string location, decimal salary, List<string>? skills, CancellationToken ct = default);
        Task<Job> GetJobByIdAsync(int jobId, CancellationToken ct = default);
        Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken ct = default);
        Task<bool> UpdateApplicationStatusAsync(int jobApplicationId, ApplicationStatus status, CancellationToken ct = default);
    }

}