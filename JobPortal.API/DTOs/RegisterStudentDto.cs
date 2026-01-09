namespace JobPortal.API.DTOs
{
    public class RegisterStudentDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Education { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}