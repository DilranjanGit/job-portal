   namespace JobPortal.API.DTOs
    {   
    public class ResumeDto
    {
        public required string Email { get; set; }
        
         public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

    }
    }
   