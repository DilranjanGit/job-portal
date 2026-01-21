namespace JobPortal.API.Domain
{
    public class CompanyProfile
    {
        public int Id { get; set; }
        public required string CompanyName { get; set; }
        public required string Email { get; set; } // Unique
        public string Location { get; set; } = "";
        public string WebsiteUrl { get; set; } = "";

        public bool InActive{get;set;}= true; // default vaule true
        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }=DateTime.UtcNow;

        public List<Job> Jobs { get; set; } = [];
    }
}