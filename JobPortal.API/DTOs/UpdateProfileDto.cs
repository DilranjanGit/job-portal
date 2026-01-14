// DTO for updating student profile

namespace JobPortal.API.DTOs
{   

public class UpdateProfileDto
{
    public required string Email { get; set; }
    public required string Education { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Location { get; set; }
    public required string[] Skills { get; set; }
}
}