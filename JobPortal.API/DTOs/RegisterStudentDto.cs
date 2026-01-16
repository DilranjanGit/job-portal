namespace JobPortal.API.DTOs
{
    public class RegisterStudentDto
    {
        public string Email { get; set; } = ""!;
        public string Password { get; set; } = ""!;
        public string FullName { get; set; } = ""!;
        public string Education { get; set; } = ""!;
        public string PhoneNumber { get; set; } = ""!;
    }
}