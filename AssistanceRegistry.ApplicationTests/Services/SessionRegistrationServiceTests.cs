using AssistanceRegistry.Application.Services;
using AssistanceRegistry.Domain.Entities;
using AssistanceRegistry.Domain.Interfaces;
using Moq;

namespace AssistanceRegistry.Application.Tests.Services
{
    [TestClass]
    public class SessionRegistrationServiceTests
    {
        private Mock<ISessionRepository> _sessionRepositoryMock;
        private SessionRegistrationService _service;

        [TestInitialize]
        public void Setup()
        {
            _sessionRepositoryMock = new Mock<ISessionRepository>();
            _service = new SessionRegistrationService(_sessionRepositoryMock.Object);
        }

        [TestMethod]
        public async Task FindByIdAsync_ReturnsSession_WhenSessionExists()
        {
            var session = new Session("1", "Test Session", 10);
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(session);

            var result = await _service.FindByIdAsync("1");

            Assert.AreEqual(session, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task FindByIdAsync_Throws_WhenSessionNotFound()
        {
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("notfound")).ReturnsAsync((Session)null);

            await _service.FindByIdAsync("notfound");
        }

        [TestMethod]
        public async Task GetAllSessionsAsync_ReturnsAllSessions()
        {
            var sessions = new List<Session>
            {
                new Session("1", "Session 1", 10),
                new Session("2", "Session 2", 20)
            };
            _sessionRepositoryMock.Setup(r => r.FindAllAsync()).ReturnsAsync(sessions);

            var result = await _service.GetAllSessionsAsync();

            CollectionAssert.AreEqual(sessions, result.ToList());
        }

        [TestMethod]
        public async Task RegisterAttendeeAsync_RegistersAttendee_WhenCapacityAvailable()
        {
            var session = new Session("1", "Test", 1);
            var command = new RegisterAttendeeCommand
            {
                SessionId = "1",
                Name = "John",
                Email = "john@example.com",
                IsVip = false,
                RegistrationTier = "Standard"
            };
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(session);

            var result = await _service.RegisterAttendeeAsync(command);

            Assert.AreEqual(AttendeeStatus.Registered, result.Status);
            Assert.IsTrue(session.RegisteredAttendees.Any(a => a.Id == result.AttendeeId));
            _sessionRepositoryMock.Verify(r => r.SaveAsync(session), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAttendeeAsync_AddsToWaitlist_WhenCapacityFull()
        {
            var session = new Session("1", "Test", 0);
            var command = new RegisterAttendeeCommand
            {
                SessionId = "1",
                Name = "Jane",
                Email = "jane@example.com",
                IsVip = false,
                RegistrationTier = "Standard"
            };
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(session);

            var result = await _service.RegisterAttendeeAsync(command);

            Assert.AreEqual(AttendeeStatus.Waitlisted, result.Status);
            Assert.IsTrue(session.WaitlistedAttendees.Any(a => a.Id == result.AttendeeId));
            _sessionRepositoryMock.Verify(r => r.SaveAsync(session), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RegisterAttendeeAsync_Throws_WhenSessionNotFound()
        {
            var command = new RegisterAttendeeCommand
            {
                SessionId = "notfound",
                Name = "Test",
                Email = "test@example.com",
                IsVip = false,
                RegistrationTier = "Standard"
            };
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("notfound")).ReturnsAsync((Session)null);

            await _service.RegisterAttendeeAsync(command);
        }

        [TestMethod]
        public async Task CancelAttendeeAsync_ReturnsFalse_WhenSessionNotFound()
        {
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("notfound")).ReturnsAsync((Session)null);

            var result = await _service.CancelAttendeeAsync("notfound", Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CancelAttendeeAsync_CancelsAttendee_WhenSessionExists()
        {
            var session = new Session("1", "Test", 10);
            var attendee = new Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Standard");
            session.RegisterAttendee(attendee);
            _sessionRepositoryMock.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(session);

            var result = await _service.CancelAttendeeAsync("1", attendee.Id);

            Assert.IsTrue(result);
            Assert.IsTrue(session.RegisteredAttendees.All(a => a.Id != attendee.Id));
        }
    }
}