    public class ResumeDto
    {
        public required string Email { get; set; }
        public required IFormFile ResumeFile { get; set; }
        public required string ResumeFileName { get; set; }
        public required string ResumeContentType { get; set; }
    }
   