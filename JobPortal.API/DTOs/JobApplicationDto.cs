using JobPortal.API.DTOs;

namespace JobPortal.API.DTOs
{
    public class JobApplicationDto
    {
        public int JobApplicationId{get;set;}=0;
        public string JobTitle { get; set; } = "";
        public string JobDescription { get; set; } = "";
        public string JobLocation { get; set; } = "";
        public string JobSkills { get; set; } = "";
        public string StudentFullName { get; set; } = default!;
        public string StudentEmail { get; set; } = default!;
        public DateTime AppliedUtc { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "";
    }
}