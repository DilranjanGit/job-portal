using JobPortal.API.Data;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JobPortal.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Application.Company
{
     public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompanyService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //implement method RegisterAsync
        public async Task<bool> RegisterAsync(string fullName, string email, string location, string webUrl, string password, CancellationToken cancellationToken = default)
        {
            // Check if company already exists
            var existingCompany = await _dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
            if (existingCompany != null) return false;

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
               
            };
            
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return false;
            await _userManager.AddToRoleAsync(user, AppRoles.Company);
            
            // Create CompanyProfile
            var companyProfile = new CompanyProfile
            {
                Email = email,
                CompanyName = fullName,
                Location = location,
                WebsiteUrl = webUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.Companies.Add(companyProfile);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Implement other methods as needed
        public Task<Message[]> GetMessagesAsync(string studentEmail, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SendMessageAsync(string fromEmail, string toEmail, string content, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PostToJobAsync(string email, string jobTitle, string description, string location, string skillsCsv, decimal salary, CancellationToken cancellationToken = default)
        {
            //get company profile by email
            var companyProfile = await _dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
            if (companyProfile == null) return false;
            //check if company already posted the job with same title

            var existingJob = await _dbContext.Jobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.CompanyProfileId == companyProfile.Id && j.Title == jobTitle && j.Location == location && j.IsActive == true, cancellationToken);
            if (existingJob != null) return false;

            // Create Job
            var job = new Job
            {
                CompanyProfileId = companyProfile.Id,
                Title = jobTitle,
                Description = description,
                Location = location,
                SkillsCsv = skillsCsv,
                Salary = salary,
                PostedUtc = DateTime.UtcNow
            };
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<CompanyProfile> GetCompanyProfileAsync(string companyEmail, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == companyEmail, cancellationToken);
        }
        public async Task<IEnumerable<CompanyProfile>> GetAllCompany(CancellationToken cancellationToken)
        {
            return await _dbContext.Companies.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobId, CancellationToken cancellationToken = default)
        {
            var applications = await _dbContext.JobApplications 
                .AsNoTracking()
                .Where(ja => ja.JobId == jobId)
                .Select(s => new JobApplicationDto
                {
                    JobApplicationId=s.Id,
                    JobTitle = s.Job.Title,
                    JobDescription = s.Job.Description,
                    JobLocation = s.Job.Location,
                    JobSkills = s.Job.SkillsCsv,
                    StudentFullName = s.Student.FullName,
                    StudentEmail = s.Student.Email,
                    FileData=s.Student.ResumeFile,
                    FileName=s.Student.ResumeFileName,
                    ContentType=s.Student.ResumeContentType,
                    AppliedUtc = s.AppliedUtc,
                    Status =  s.Status.ToString()
                })
                .ToListAsync(cancellationToken);

            return applications;
        }
        public async Task<bool> ScheduleInterviewAsync(int jobApplicationId, DateTime interviewDate,string locationOrLink, int mode, CancellationToken cancellationToken = default)
        {
            var job = await _dbContext.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobApplicationId, cancellationToken);
            if (job == null) return false;

            var interview = new Interview
            {
                JobApplicationId=jobApplicationId,
                ScheduledAtLocal = interviewDate,
                LocationOrLink = locationOrLink,
                Mode = (InterviewMode)mode,
                Status = InterviewStatus.Scheduled
            };
            _dbContext.Interviews.Add(interview);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IEnumerable<ScheduleInterviewDto>> GetScheduleInterviewsAsync(int jobApplicationId,CancellationToken cancellationToken=default)
        {
            var scheduleInterviewDto =await _dbContext.Interviews.AsNoTracking()
            .Where(i=>i.JobApplicationId==jobApplicationId)
            .Select(s=> new ScheduleInterviewDto{
                JobApplicationId=s.JobApplicationId,
                Id=s.Id,
                LocationOrLink=s.LocationOrLink,
                Mode=(int)s.Mode,
                InterviewDate=s.ScheduledAtLocal

            })
            .ToListAsync(cancellationToken);

            return scheduleInterviewDto;
        }
    }

    
}