using JobPortal.API.Application.Students;
using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace JobPortal.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Student")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Registration (open to all, not protected) allow anonymous but once registered student gets "Student" role and can access other endpoints

        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
        {
            var result = await _studentService.RegisterAsync(dto.FullName, dto.Email, dto.Password, cancellationToken);
            if (!result) return BadRequest("Registration failed");
            return Ok();
        }

        // Get Profile by Email
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email, CancellationToken cancellationToken)
        {
            var profile = await _studentService.GetByUserEmailAsync(email, cancellationToken);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        // Upsert Resume
        [HttpPost("resume")]
        public async Task<IActionResult> UpsertResume([FromBody] ResumeDto dto, CancellationToken cancellationToken)
        {
            var profile = await _studentService.UpsertResumeAsync(dto.FullName, dto.Email, dto.PhoneNumber, dto.Education, dto.ResumeText, dto.Skills, cancellationToken);
            return Ok(profile);
        }

        // Apply to Job
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToJob([FromBody] ApplyJobDto dto, CancellationToken cancellationToken)
        {
            var result = await _studentService.ApplyToJobAsync(dto.StudentEmail, dto.JobId, cancellationToken);
            if (!result) return BadRequest("Application failed");
            return Ok();
        }

        // Get Messages
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string studentEmail, CancellationToken cancellationToken)
        {
            var messages = await _studentService.GetMessagesAsync(studentEmail, cancellationToken);
            return Ok(messages);
        }

        // Send Message
        [HttpPost("messages/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            var result = await _studentService.SendMessageAsync(dto.FromEmail, dto.ToEmail, dto.Content, cancellationToken);
            if (!result) return BadRequest("Message send failed");
            return Ok();
        }
    }

    // DTOs
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class ResumeDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Education { get; set; }
        public string? ResumeText { get; set; }
        public string[]? Skills { get; set; }
    }
    public class ApplyJobDto
    {
        public required string StudentEmail { get; set; }
        public required string JobId { get; set; }
    }
    public class SendMessageDto
    {
        public required string FromEmail { get; set; }
        public required string ToEmail { get; set; }
        public required string Content { get; set; }
    }
}
