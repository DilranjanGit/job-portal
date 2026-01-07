using Microsoft.AspNetCore.Identity;

namespace JobPortal.API.Domain
{
    public class ApplicationUser : IdentityUser
    {
        // Additional properties can be added here in the future
        public bool IsActive{get;set;}=true;
    }
}