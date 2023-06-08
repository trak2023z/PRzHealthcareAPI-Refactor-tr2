using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPIRefactor.Models.DTO;
using PRzHealthcareAPIRefactor.Services;
using System.Security.Claims;

namespace PRzHealthcareAPIRefactor.Controllers
{
    [Route("event")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            this._eventService = eventService;
        }

        [AllowAnonymous]
        [HttpPost("seed")]
        public async Task<IActionResult> Seed([FromQuery] int doctorId)
        {
            _eventService.SeedDates(doctorId);
            return Ok();
        }

        [HttpPatch("takeeventterm")]
        public ActionResult TakeTerm([FromBody] EventDto dto)
        {
            string accountId = "";
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = identity.FindFirst(ClaimTypes.SerialNumber).Value;
            }

            _eventService.TakeTerm(dto, accountId);
            return Ok();
        }

        [HttpPatch("finishterm")]
        public ActionResult FinishTerm([FromBody] EventDto dto)
        {
            string accountId = "";
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = identity.FindFirst(ClaimTypes.SerialNumber).Value;
            }

            _eventService.FinishTerm(dto, accountId);
            return Ok();
        }

        [HttpGet("getnurseevents")]
        public ActionResult GetNurseEvents()
        {
            var availableEvents = _eventService.GetNurseEvents();
            return Ok(availableEvents);
        }

        [HttpGet]
        public ActionResult GetSelectedEvent([FromQuery] int eventId)
        {
            var selectedEvent = _eventService.GetSelectedEvent(eventId);
            return Ok(selectedEvent);
        }
    }
}
