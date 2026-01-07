using JobPortal.API.Domain;

namespace JobPortal.API.Application.Jobs
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(string companyEmail,string title, string description, string location, decimal salary, List<string>? skills, CancellationToken ct = default);
    }
}