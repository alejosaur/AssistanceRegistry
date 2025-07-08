using AssistanceRegistry.Domain.Entities;
using AssistanceRegistry.Domain.Interfaces;

namespace AssistanceRegistry.Application.Services
{
    public class SessionRegistrationService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionRegistrationService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        // Find a session by ID
        public async Task<Session> FindByIdAsync(string sessionId)
        {
            var sessionObj = await _sessionRepository.FindByIdAsync(sessionId);
            if (sessionObj == null)
            {
                throw new InvalidOperationException("Session not found");
            }

            return sessionObj;
        }

        // Get all sessions
        public async Task<IEnumerable<Session>> GetAllSessionsAsync()
        {
            return await _sessionRepository.FindAllAsync();
        }

        public async Task<RegisterAttendeeResult> RegisterAttendeeAsync(RegisterAttendeeCommand command)
        {
            var sessionObj = await _sessionRepository.FindByIdAsync(command.SessionId);
            if (sessionObj == null)
            {
                throw new InvalidOperationException("Session not found");
            }

            var newAttendee = new Attendee(Guid.NewGuid(), command.Name, command.Email, command.IsVip, command.RegistrationTier);
            if (sessionObj.IsCapacityAvailable())
            {
                sessionObj.RegisterAttendee(newAttendee);
            }
            else
            {
                sessionObj.AddAttendeeToWaitlist(newAttendee);
            }

            await _sessionRepository.SaveAsync(sessionObj);
            return new RegisterAttendeeResult { AttendeeId = newAttendee.Id, Status = newAttendee.Status };
        }

        // Cancel an attendee and promote the next one from the waitlist based on priority score
        public async Task<bool> CancelAttendeeAsync(string sessionId, Guid attendeeId)
        {
            var session = await _sessionRepository.FindByIdAsync(sessionId);
            if (session == null) return false;

            session.CancelAttendee(attendeeId);

            return true;
        }
    }


    public class RegisterAttendeeCommand
    {
        public string SessionId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVip { get; set; }
        public string RegistrationTier { get; set; }
    }

    public class RegisterAttendeeResult
    {
        public Guid AttendeeId { get; set; }
        public AttendeeStatus Status { get; set; }
    }
}