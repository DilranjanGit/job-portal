using JobPortal.API.Data;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;

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
}
