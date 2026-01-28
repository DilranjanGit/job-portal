using JobPortal.API.Data;
using JobPortal.API.Domain;
using JobPortal.API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobPortal.API.Application.Jobs;

public class JobService : IJobService
{
    private readonly AppDbContext _db;
    private readonly IJobRepository _repo;

    public JobService(AppDbContext db, IJobRepository repo)
    {
        _db=db;
        _repo=repo;
    }

    public async Task<Job> CreateJobAsync(string companyEmail, string title, string description, string location, decimal salary, List<string>? skills, CancellationToken ct = default)
    {
        //Ensure company exists 
        var company = await _db.Companies.FirstOrDefaultAsync(c=>c.Email==companyEmail,ct);
        if(company is null)
        {
            //Auto-create minimal company profile to keep step 2 functional
            company = new CompanyProfile{CompanyName=companyEmail.Split('@')[0],Email= companyEmail, Location=location};
            _db.Companies.Add(company);
            await _db.SaveChangesAsync(ct);
        }

        var job = new Job
        {
            CompanyProfileId=company.Id,
            Title=title,
            Description=description,
            Location=location,
            Salary=salary,
            SkillsCsv=string.Join(",",skills??[])
        };

        return await _repo.AddAsync(job,ct);
    }

    public Task<Job?> GetJobByIdAsync(int jobId, CancellationToken ct = default)
    {
        return _repo.GetAsync(jobId,ct);
    }
    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string location = "", int studentId=0 , CancellationToken ct = default)
    {
        var jobs = await _db.Jobs
            .Include(j => j.Company)
            .Where(j => string.IsNullOrEmpty(location) || j.Location == location)
            .GroupJoin(
            _db.JobApplications.Where(a => a.StudentProfileId == studentId),
            j => j.Id, a => a.JobId, (job, apps) => new { job, apps}
    )
    .Select(x => new JobDto
    {
        Id = x.job.Id,
        Title = x.job.Title,
        CompanyId = x.job.Company.Id,
        CompanyName = x.job.Company.CompanyName,
        Location = x.job.Location,
        Description = x.job.Description,
        Skills = x.job.SkillsCsv,
        Applied = x.apps.Any(),   // ← true if an application exists
        Scheduled = _db.JobApplications
            .Where(a => a.JobId == x.job.Id && a.StudentProfileId == studentId)
            .SelectMany(a => _db.Interviews.Where(i => i.JobApplicationId == a.Id))
            .Any()

    })
    .ToListAsync(ct);
    return jobs;
   }
    
    public async Task<IEnumerable<JobDto>> GetAllJobsByCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var jobs = await _db.Jobs
            .Include(j => j.Company)
            .Where(j => j.CompanyProfileId == companyId)
            .GroupJoin( _db.JobApplications, job=>job.Id, app=> app.JobId,
            (job,apps) => new { job=job, ApplicationCount=apps.Count()})
            .Select(x => new JobDto
            {
                Id = x.job.Id,
                Title = x.job.Title,
                CompanyId = x.job.Company.Id,
                CompanyName = x.job.Company.CompanyName,
                Location = x.job.Location,
                Description = x.job.Description,
                Skills = x.job.SkillsCsv,
                ApplicationCount = x.ApplicationCount   // 0 if no apllicaion for job
            })
            .ToListAsync(ct);
            return jobs;
    }
    
    public Task<bool> UpdateApplicationStatusAsync(int jobApplicationId, ApplicationStatus status, CancellationToken ct = default)
    {
        return _repo.UpdateApplicationStatusAsync(jobApplicationId, status, ct);
    }
}
