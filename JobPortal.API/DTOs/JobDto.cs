
using System;

namespace JobPortal.API.DTOs
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Skills { get; set; } = "";
        public string Location { get; set; } = "";
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = "";

        public bool Applied{get;set;}= false;

        public int ApplicationCount{get;set;} = 0;

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
