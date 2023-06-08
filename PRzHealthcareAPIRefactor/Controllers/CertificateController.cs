using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Models.DTO;
using PRzHealthcareAPIRefactor.Services;
using System.Security.Claims;

namespace PRzHealthcareAPIRefactor.Controllers
{
    [Route("certificate")]
    [ApiController]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

        public CertificateController(ICertificateService certificateService, IWebHostEnvironment hostingEnvironment)
        {
            _certificateService = certificateService;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        [HttpPost("covid")]
        public IActionResult ExportToPDF([FromQuery] int eventId)
        {
            var score = _certificateService.PrintCOVIDCertificateToPDF(eventId);
            return score;
        }

        
    }
}
