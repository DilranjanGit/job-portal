
using JobPortal.API.Data;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.API.Application.Jobs;

public class EfJobRepository : IJobRepository
{
    private readonly AppDbContext _db;
    public EfJobRepository(AppDbContext db) => _db = db;

    public async Task<List<Job>> ListActiveAsync(string? q = null, string? location = null, CancellationToken ct = default)
    {
        var query = _db.Jobs.Include(j => j.Company).Where(j => j.IsActive);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(j => j.Title.Contains(q) || j.Description.Contains(q));
        if (!string.IsNullOrWhiteSpace(location)) query = query.Where(j => j.Location.Contains(location));
        return await query.OrderByDescending(j => j.PostedUtc).ToListAsync(ct);
    }

    public Task<Job?> GetAsync(int id, CancellationToken ct = default)
        => _db.Jobs.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id, ct);

    public async Task<Job> AddAsync(Job job, CancellationToken ct = default)
    {
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync(ct);
        return job;
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var job = await _db.Jobs.FindAsync([id], ct);
        if (job is null) return false;
        job.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> UpdateApplicationStatusAsync(int jobApplicationId, ApplicationStatus status, CancellationToken ct = default)
    {
        var application = await _db.JobApplications.FindAsync([jobApplicationId], ct);
        if (application is null) return false;
        application.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
