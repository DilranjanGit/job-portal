using System.Data;

namespace JobPortal.API.Domain
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; } // Unique
        public required string PhoneNumber { get; set; }// Unique
        public required string Education { get; set; }="";
        public string SkillsCsv { get; set; } = ""; //e.g., "C#,ASP.NET,SQL"
        public string? ResumeText { get; set; } // simple text for Step2

        public DateTime CreatedUtc { get; set; }=DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; }=DateTime.UtcNow;
        
    }
}