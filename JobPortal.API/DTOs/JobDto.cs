
using System;

namespace JobPortal.API.DTOs
{
    public class JobDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string RequiredSkills { get; set; } = "";
        public string Location { get; set; } = "";
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = "";
    }

    public class JobFilter
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public string? Skill { get; set; }
    }

    public class JobApplyRequest
    {
        public Guid JobId { get; set; }
        public string? CoverLetter { get; set; }
    }
}
