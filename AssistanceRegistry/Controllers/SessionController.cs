using AssistanceRegistry.Application;
using AssistanceRegistry.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AssistanceRegistry.API
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionController : ControllerBase
    {
        private readonly SessionRegistrationService _registrationService;

        public SessionController(SessionRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("registerattendee")]
        public async Task<IActionResult> RegisterAttendee([FromBody] RegisterAttendeeCommand command)
        {
            if (command == null || string.IsNullOrEmpty(command.Name) || string.IsNullOrEmpty(command.Email))
            {
                return BadRequest("Invalid attendee registration data.");
            }

            try
            {
                var result = await _registrationService.RegisterAttendeeAsync(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(string sessionId)
        {
            var session = await _registrationService.FindByIdAsync(sessionId); 
            if (session == null)
            {
                return NotFound("Session not found");
            }

            return Ok(session);
        }

        [HttpDelete("cancel/{sessionId}/{attendeeId}")]
        public async Task<IActionResult> CancelAttendee(string sessionId, Guid attendeeId)
        {
            var success = await _registrationService.CancelAttendeeAsync(sessionId, attendeeId);
            return success ? Ok("Attendee canceled and promoted successfully.") : NotFound("Attendee not found.");
        }
    }
}