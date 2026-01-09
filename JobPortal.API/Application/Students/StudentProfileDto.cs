
using System;

namespace JobPortal.API.Application.Students
{
    public class StudentProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Education { get; set; } = "";
        public string Skills { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Location { get; set; } = "";
    }

    public class UpdateStudentProfileDto
    {
        public string? Education { get; set; }
        public string? Skills { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
    }
}

