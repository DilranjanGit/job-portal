namespace JobPortal.API.Domain
{
    public class Job
    {   
        public int Id { get; set; }
        public int CompanyProfileId { get; set; }
        public CompanyProfile Company { get; set; }=default!;
        public required string Title { get; set; }
        public required string Description { get; set; }
        public  string Location { get; set; }="";
        public  string SkillsCsv { get; set; }=""; // e.g., "C#,ASP.NET,SQL"
        public decimal Salary { get; set; }=0;
        public bool IsActive { get; set; }=true;
        public DateTime PostedUtc    { get; set; }=DateTime.UtcNow;
        
        public List<JobApplication> Applications { get; set; } = [];
    }
}