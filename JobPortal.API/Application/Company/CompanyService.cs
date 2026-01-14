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
                UserName = fullName,
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

        public Task<bool> PostToJobAsync(string companyEmail, string jobTitle, string description, string location, string skillsCsv, decimal salary, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }




    }
}