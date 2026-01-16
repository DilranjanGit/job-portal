using System.Collections.Generic;

namespace JobPortal.API.DTOs
{
    public class JobCreateDto
    {
        public string CompanyEmail { get; set; }="";
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Salary { get; set; } = 0.00M;
        public string Skills { get; set; } = null!;
    }
}