using AssistanceRegistry.Domain.Entities;

namespace AssistanceRegistry.Domain.Tests.Entities
{
    [TestClass]
    public class SessionTests
    {
        [TestMethod]
        public void RegisterAttendee_WhenCapacityAvailable_AddsAttendeeToRegisteredList()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 2);
            var attendee = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");

            // Act
            session.RegisterAttendee(attendee);

            // Assert
            Assert.AreEqual(1, session.RegisteredAttendees.Count);
            Assert.AreEqual(attendee, session.RegisteredAttendees[0]);
            Assert.AreEqual(AttendeeStatus.Registered, attendee.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterAttendee_WhenCapacityFull_ThrowsException()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 1);
            var attendee1 = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");
            var attendee2 = new Attendee(Guid.NewGuid(), "Jane Smith", "jane@example.com", false, "Standard");
            session.RegisterAttendee(attendee1);

            // Act
            session.RegisterAttendee(attendee2);
        }

        [TestMethod]
        public void AddAttendeeToWaitlist_AddsAttendeeToWaitlist()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 1);
            var attendee = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");

            // Act
            session.AddAttendeeToWaitlist(attendee);

            // Assert
            Assert.AreEqual(1, session.WaitlistedAttendees.Count);
            Assert.AreEqual(attendee, session.WaitlistedAttendees[0]);
            Assert.AreEqual(AttendeeStatus.Waitlisted, attendee.Status);
        }

        [TestMethod]
        public void CancelAttendee_WhenAttendeeRegistered_RemovesAttendeeAndPromotesFromWaitlist()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 1);
            var attendee1 = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");
            var attendee2 = new Attendee(Guid.NewGuid(), "Jane Smith", "jane@example.com", false, "Standard");
            session.RegisterAttendee(attendee1);
            session.AddAttendeeToWaitlist(attendee2);

            // Act
            var result = session.CancelAttendee(attendee1.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, session.RegisteredAttendees.Count);
            Assert.AreEqual(attendee2, session.RegisteredAttendees[0]);
            Assert.AreEqual(AttendeeStatus.Registered, attendee2.Status);
            Assert.AreEqual(0, session.WaitlistedAttendees.Count);
            Assert.AreEqual(AttendeeStatus.Cancelled, attendee1.Status);
        }

        [TestMethod]
        public void CancelAttendee_WhenAttendeeNotRegistered_ReturnsFalse()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 1);
            var attendee = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");

            // Act
            var result = session.CancelAttendee(attendee.Id);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PromoteFromWaitlist_WhenWaitlistNotEmptyAndCapacityAvailable_PromotesAttendee()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 2);
            var attendee1 = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");
            var attendee2 = new Attendee(Guid.NewGuid(), "Jane Smith", "jane@example.com", false, "Standard");
            attendee1.PriorityScore = 10;
            attendee2.PriorityScore = 20;
            session.AddAttendeeToWaitlist(attendee1);
            session.AddAttendeeToWaitlist(attendee2);

            // Act
            var promoted = session.PromoteFromWaitlist();

            // Assert
            Assert.AreEqual(attendee2, promoted);
            Assert.AreEqual(1, session.WaitlistedAttendees.Count);
            Assert.AreEqual(1, session.RegisteredAttendees.Count);
            Assert.AreEqual(AttendeeStatus.Registered, attendee2.Status);
        }

        [TestMethod]
        public void PromoteFromWaitlist_WhenWaitlistEmpty_ReturnsNull()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 2);

            // Act
            var promoted = session.PromoteFromWaitlist();

            // Assert
            Assert.IsNull(promoted);
        }

        [TestMethod]
        public void IsCapacityAvailable_ReturnsTrueWhenCapacityNotFull()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 2);
            var attendee = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");
            session.RegisterAttendee(attendee);

            // Act
            var result = session.IsCapacityAvailable();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsCapacityAvailable_ReturnsFalseWhenCapacityFull()
        {
            // Arrange
            var session = new AssistanceRegistry.Domain.Entities.Session("1", "Test Session", 1);
            var attendee = new Attendee(Guid.NewGuid(), "John Doe", "john@example.com", false, "Standard");
            session.RegisterAttendee(attendee);

            // Act
            var result = session.IsCapacityAvailable();

            // Assert
            Assert.IsFalse(result);
        }
    }
}