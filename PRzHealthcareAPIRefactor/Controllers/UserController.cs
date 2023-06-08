using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Models.DTO;
using PRzHealthcareAPIRefactor.Services;

namespace PRzHealthcareAPIRefactor.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto dto)
        {
            _userService.Register(dto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            var loggedUser = _userService.GenerateToken(dto);
            return Ok(loggedUser);
        }
        [HttpGet("getdoctorslist")]
        public ActionResult GetDoctorsList()
        {
            var doctors = _userService.GetDoctorsList();
            return Ok(doctors);
        }
        [HttpGet("getpatientslist")]
        public ActionResult GetPatientsList()
        {
            var patients = _userService.GetPatientsList();
            return Ok(patients);
        }
    }
}
