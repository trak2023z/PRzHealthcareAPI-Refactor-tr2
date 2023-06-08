using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Models.DTO;
using PRzHealthcareAPIRefactor.Services;
using System.Security.Claims;

namespace PRzHealthcareAPIRefactor.Controllers
{
    [Route("vaccination")]
    [ApiController]
    [Authorize]
    public class VaccinationController : ControllerBase
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationController(IVaccinationService vaccinationService)
        {
            this._vaccinationService = vaccinationService;
        }

        [HttpGet("getall")]
        public ActionResult GetVaccinationList()
        {
            var vaccinationDtos = _vaccinationService.GetAll();
            return Ok(vaccinationDtos);
        }
    }
}
