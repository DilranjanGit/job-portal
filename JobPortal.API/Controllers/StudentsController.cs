using JobPortal.API.Application.Students;
using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using JobPortal.API.DTOs;
using JobPortal.API.Application.Jobs;

namespace JobPortal.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AppRoles.Student)]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IJobService _jobService;
        public StudentsController(IStudentService studentService, IJobService jobService)
        {
            _studentService = studentService;
            _jobService = jobService;
        }

        // Registration (open to all, not protected) allow anonymous but once registered student gets "Student" role and can access other endpoints

        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterStudentDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine("Registering student: " + dto.Email);
            var result = await _studentService.RegisterAsync(dto.FullName, dto.Email, dto.Password, cancellationToken);
            if (!result) return BadRequest("Registration failed");
            return Ok();
        }

        // Get Profile by Email
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var profile = await _studentService.GetByUserEmailAsync(email, cancellationToken);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        //Update Profile details like name, skills, education, phonenumber etc.
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _studentService.UpdateProfileAsync(dto.Email, dto.Education, dto.PhoneNumber, dto.Location, dto.Skills, cancellationToken);
            if (!result) return BadRequest("Profile update failed");
            return Ok();
        }

        // Upsert Resume
        [HttpPost("resume")]
        public async Task<IActionResult> UpsertResume([FromForm] IFormFile resumeFile, [FromForm] string email, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var profile = await _studentService.UpsertResumeAsync(resumeFile, email, cancellationToken);
            return Ok(profile);
        }
  

        // Apply to Job
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToJob([FromBody] ApplyJobDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _studentService.ApplyToJobAsync(dto.StudentEmail, dto.JobId, cancellationToken);
            if (!result) return BadRequest("Application failed");
            return Ok();
        }

        //Get All Jobs
        [HttpGet("jobs")]
        public async Task<IActionResult> GetAllJobs(CancellationToken cancellationToken)
        {
            var jobs = await _jobService.GetAllJobsAsync(cancellationToken);
            return Ok(jobs);
        }

        // Get Messages
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string studentEmail, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var messages = await _studentService.GetMessagesAsync(studentEmail, cancellationToken);
            return Ok(messages);
        }

        // Send Message
        [HttpPost("messages/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _studentService.SendMessageAsync(dto.FromEmail, dto.ToEmail, dto.Content, cancellationToken);
            if (!result) return BadRequest("Message send failed");
            return Ok();
        }
    }

// add missing DTOs used in StudentsController
    
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