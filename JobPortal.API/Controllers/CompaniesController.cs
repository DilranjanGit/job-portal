using JobPortal.API.Application.Company;
using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using JobPortal.API.DTOs;
using JobPortal.API.Application.Jobs;

namespace JobPortal.API.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AppRoles.Company)]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IJobService _jobService;
        public CompanyController(ICompanyService companyService, IJobService jobService)
        {
            _companyService = companyService;
            _jobService = jobService;
        }

        // Registration (open to all, not protected) allow anonymous but once registered company gets "Company" role and can access other endpoints

        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine("Registering company: " + dto.Email);
            var result = await _companyService.RegisterAsync(dto.CompanyName, dto.Email, dto.Location, dto.WebsiteUrl, dto.Password, cancellationToken);
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
            var profile = await _companyService.GetCompanyProfileAsync(email, cancellationToken);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        //Get jobs
        [HttpGet("jobs")]
       public async Task<ActionResult<IEnumerable<JobDto>>> GetAllJobs(string email, CancellationToken cancellationToken)
        {
            var companyProfile = await _companyService.GetCompanyProfileAsync(email,cancellationToken);
            var jobdto = await _jobService.GetAllJobsByCompanyAsync(companyProfile.Id, cancellationToken);
            return jobdto.ToList();
        }

        [HttpGet("jobsApplications")]
        public async Task<IActionResult> GetJobsWithApplications( string email, CancellationToken cancellationToken)
        {   
            
            var companyProfile = await _companyService.GetCompanyProfileAsync(email, cancellationToken);
            var jobs = await _jobService.GetAllJobsByCompanyAsync(companyProfile.Id, cancellationToken);

            var jobsWithApps = new List<object>();

            foreach (var job in jobs)
            {
                var applications = await _companyService.GetJobApplicationsAsync(job.Id, cancellationToken);

                var appsWithStudent = new List<object>();

                foreach (var app in applications)
                {
        
                   // var interview = await _companyService.GetScheduleInterviewsAsync(app.JobApplicationId, cancellationToken);

                    appsWithStudent.Add(new
                    {
                        id = app.JobApplicationId,
                        student = new
                        {
                            fullName = app.StudentFullName,
                            email = app.StudentEmail,
                            resumeUrl = "" // your logic
                        }
                        /*,
                        interview = interview == null ? null : new
                        {
                            interviewId = interview.,
                            interviewDate = interview.InterviewDate,
                            mode = interview.Mode,
                            locationOrLink = interview.LocationOrLink,
                            status = interview.Status
                        }*/
                    });
                }

                jobsWithApps.Add(new
                {
                    id = job.Id,
                    title = job.Title,
                    location = job.Location,
                    applications = appsWithStudent
                });
            }
  
             return Ok(jobsWithApps);
        }
        //Post a job
        [HttpPost("jobs")]
        public async Task<IActionResult> PostJob([FromBody] JobCreateDto dto, CancellationToken cancellationToken)
        {
           if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var job = await _companyService.PostToJobAsync(dto.CompanyEmail, dto.Title, dto.Description, dto.Location, dto.Skills, dto.Salary, cancellationToken);
            //return Created($"/api/jobs/{job.Id}", job);
            if (!job) return BadRequest("Job Posting Failed..");
            return Ok(job);
        }

        //Get applications for a job
        [HttpGet("jobs/{jobId}/applications")]
        public async Task<IActionResult> GetJobApplications([FromRoute] int jobId, CancellationToken cancellationToken)
        {
            var applications = await _companyService.GetJobApplicationsAsync(jobId, cancellationToken);
            if (applications == null) return NotFound();
            return Ok(applications);
        }

        //Send message to student
        [HttpPost("messages/send")] 
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _companyService.SendMessageAsync(dto.FromEmail, dto.ToEmail, dto.Content, cancellationToken);
            if (!result) return BadRequest("Message send failed");
            return Ok();
        }

        //Schedule interview
        [HttpPost("interviews/schedule")]
        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewDto dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var statusUpdate = await _jobService.UpdateApplicationStatusAsync(dto.JobApplicationId, ApplicationStatus.InterviewScheduled, cancellationToken);
            if (!statusUpdate) return BadRequest("Failed to update application status");
            var result = await _companyService.ScheduleInterviewAsync(dto.JobApplicationId, dto.InterviewDate, dto.LocationOrLink, dto.Mode, cancellationToken);
            if (!result) return BadRequest("Interview scheduling failed");
            return Ok();
        }
        //Get Schedule interview
        [HttpGet("interviews/getSchedule")]
        public async Task<IActionResult> GetScheduleInterview(int jobApplicationId,CancellationToken cancellationToken=default)
        {
            var scheduleNterview = await _companyService.GetScheduleInterviewsAsync(jobApplicationId,cancellationToken);
            return Ok(scheduleNterview);
        }

    }

}
