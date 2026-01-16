using JobPortal.API.Domain;

namespace JobPortal.API.Application.Jobs
{
    public interface IJobRepository
    {
    Task<List<Job>> ListActiveAsync(string? q = null, string? location = null, CancellationToken ct = default);
    Task<Job?> GetAsync(int id, CancellationToken ct = default);
    Task<Job> AddAsync(Job job, CancellationToken ct = default);
    Task<bool> DeactivateAsync(int id, CancellationToken ct = default);

    Task<bool> UpdateApplicationStatusAsync(int jobId, ApplicationStatus status, CancellationToken ct = default);

    }
}