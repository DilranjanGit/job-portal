using JobPortal.API.Application.Company;
using JobPortal.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using JobPortal.API.DTOs;

namespace JobPortal.API.Controllers
{
    [ApiController]
    [Route("api/company")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AppRoles.Company)]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // Registration (open to all, not protected) allow anonymous but once registered company gets "Company" role and can access other endpoints

        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto, CancellationToken cancellationToken)
        {
            Console.WriteLine("Registering company: " + dto.Email);
            var result = await _companyService.RegisterAsync(dto.CompanyName, dto.Email, dto.Location, dto.WebsiteUrl, dto.Password, cancellationToken);
            if (!result) return BadRequest("Registration failed");
            return Ok();
        }

        
    }

}
