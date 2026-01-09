using System.Collections.Generic;

namespace JobPortal.API.DTOs
{
    public class JobCreateDto
    {
        public string CompanyEmail { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Salary { get; set; }
        public List<string>? Skills { get; set; }
    }
}