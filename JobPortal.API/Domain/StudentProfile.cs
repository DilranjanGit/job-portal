using System.Data;

namespace JobPortal.API.Domain
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; } // Unique
        public string PhoneNumber { get; set; } = ""; // Consider making unique in future
        public required string Education { get; set; } = "";
        public string Location { get; set; } = ""; // city/state/country
        public string SkillsCsv { get; set; } = ""; //e.g., "C#,ASP.NET,SQL"
        
        public byte[]? ResumeFile { get; set; }                 // VARBINARY(MAX)
        public string? ResumeFileName { get; set; }             // "resume.pdf/docx"
        public string? ResumeContentType { get; set; }          // "application/pdf"
        public DateTime? ResumeUploadedAt { get; set; }
        public bool InActive{get;set;}= true; // default vaule true
        public DateTime CreatedUtc { get; set; }=DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; }=DateTime.UtcNow;
        
    }
}