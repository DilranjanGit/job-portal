using JobPortal.API.Data;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JobPortal.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Application.Students
{
     public class StudentService : IStudentService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public Task<StudentProfile?> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.Email == email, cancellationToken);
        }
        public async Task<bool> UpdateProfileAsync(string email,string education, string phoneNumber, string location, string[] skills, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Students
                .FirstOrDefaultAsync(sp => sp.Email == email, cancellationToken);
            if (student == null) return false;
            student.PhoneNumber = phoneNumber;
            student.Location = location;
            student.Education = education;
            student.SkillsCsv = string.Join(",", skills);
            student.UpdatedUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<StudentProfile> UpsertResumeAsync([FromForm]IFormFile resumeFile,[FromForm] string email, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Students
                .FirstOrDefaultAsync(sp => sp.Email == email, cancellationToken);
            if (student == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                await resumeFile.CopyToAsync(memoryStream, cancellationToken);
                student.ResumeFile = memoryStream.ToArray();
            }
            student.ResumeFileName = resumeFile.FileName;
            student.ResumeContentType = resumeFile.ContentType;
            student.ResumeUploadedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return student;
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default)
        {
            // Check if student already exists
            var exists = await _dbContext.Students.AnyAsync(s => s.Email == email, cancellationToken);
            if (exists) return false;
            var student = new StudentProfile
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = "", // default empty, can be updated later
                Education = "",
                Location = "",
                SkillsCsv = "",
                ResumeFile = null,
                ResumeFileName = null,
                ResumeContentType = null
            };
             _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync(cancellationToken);
           
            // create ApplicationUser for login
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return false;
            await _userManager.AddToRoleAsync(user, AppRoles.Student);
            return true;
        }

        public async Task<bool> ApplyToJobAsync(string studentEmail, string jobId, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == studentEmail, cancellationToken);
            if (student == null) return false;
            if (!int.TryParse(jobId, out var jobIdInt)) return false;
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobIdInt, cancellationToken);
            if (job == null) return false;
            var alreadyApplied = await _dbContext.JobApplications.AnyAsync(a => a.StudentProfileId == student.Id && a.JobId == jobIdInt, cancellationToken);
            if (alreadyApplied) return false;
            var application = new JobApplication
            {
                StudentProfileId = student.Id,
                JobId = jobIdInt,
                AppliedUtc = DateTime.UtcNow,
                Status = ApplicationStatus.Applied
            };
            _dbContext.JobApplications.Add(application);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<Message[]> GetMessagesAsync(string studentEmail, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Messages
                .Where(m => m.ReceiverEmail == studentEmail || m.SenderEmail == studentEmail)
                .OrderByDescending(m => m.SentUtc)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<bool> SendMessageAsync(string fromEmail, string toEmail, string content, CancellationToken cancellationToken = default)
        {
            var sender = await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == fromEmail, cancellationToken);
            if (sender == null) return false;
            var receiver = await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == toEmail, cancellationToken);
            if (receiver == null) return false;
            var message = new Message
            {
                Sender = SenderRole.Student,
                SenderEmail = fromEmail,
                ReceiverEmail = toEmail,
                Content = content,
                SentUtc = DateTime.UtcNow,
                IsRead = false
            };
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}