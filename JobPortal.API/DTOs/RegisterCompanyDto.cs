namespace JobPortal.API.DTOs
{
    public class RegisterCompanyDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;
    }
}