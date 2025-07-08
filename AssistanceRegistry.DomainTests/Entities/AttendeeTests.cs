namespace AssistanceRegistry.Domain.Tests.Entities
{
    [TestClass]
    public class AttendeeTests
    {
        [TestMethod]
        public void Constructor_SetsPropertiesCorrectly_AndCalculatesPriorityScore()
        {
            var id = Guid.NewGuid();
            var name = "John Doe";
            var email = "john@example.com";
            var isVip = true;
            var registrationTier = "Gold";

            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(id, name, email, isVip, registrationTier);

            Assert.AreEqual(id, attendee.Id);
            Assert.AreEqual(name, attendee.Name);
            Assert.AreEqual(email, attendee.Email);
            Assert.AreEqual(AssistanceRegistry.Domain.Entities.AttendeeStatus.Pending, attendee.Status);
            Assert.AreEqual(isVip, attendee.IsVip);
            Assert.AreEqual(AssistanceRegistry.Domain.Entities.RegistrationTier.Gold, attendee.RegistrationTier);

            Assert.AreEqual(540, attendee.PriorityScore);
        }

        [TestMethod]
        public void Constructor_InvalidRegistrationTier_DefaultsToStandard()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Jane", "jane@example.com", false, "InvalidTier");
            Assert.AreEqual(AssistanceRegistry.Domain.Entities.RegistrationTier.Standard, attendee.RegistrationTier);
            Assert.AreEqual(100, attendee.PriorityScore);
        }

        [TestMethod]
        public void MarkAsRegistered_ChangesStatusAndSetsDate()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Standard");
            var regDate = DateTime.UtcNow;

            attendee.MarkAsRegistered(regDate);

            Assert.AreEqual(AssistanceRegistry.Domain.Entities.AttendeeStatus.Registered, attendee.Status);
            Assert.AreEqual(regDate, attendee.RegistrationDate);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarkAsRegistered_ThrowsIfAlreadyRegistered()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Standard");
            attendee.MarkAsRegistered(DateTime.UtcNow);
            attendee.MarkAsRegistered(DateTime.UtcNow.AddMinutes(1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarkAsRegistered_ThrowsIfCancelled()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Standard");
            attendee.MarkAsCancelled();
            attendee.MarkAsRegistered(DateTime.UtcNow);
        }

        [TestMethod]
        public void MarkAsWaitlisted_ChangesStatusAndSetsDate()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Premium");
            var regDate = DateTime.UtcNow;

            attendee.MarkAsWaitlisted(regDate);

            Assert.AreEqual(AssistanceRegistry.Domain.Entities.AttendeeStatus.Waitlisted, attendee.Status);
            Assert.AreEqual(regDate, attendee.RegistrationDate);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarkAsWaitlisted_ThrowsIfAlreadyWaitlisted()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Premium");
            attendee.MarkAsWaitlisted(DateTime.UtcNow);
            attendee.MarkAsWaitlisted(DateTime.UtcNow.AddMinutes(1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarkAsWaitlisted_ThrowsIfCancelled()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Premium");
            attendee.MarkAsCancelled();
            attendee.MarkAsWaitlisted(DateTime.UtcNow);
        }

        [TestMethod]
        public void MarkAsCancelled_ChangesStatus()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Gold");
            attendee.MarkAsCancelled();
            Assert.AreEqual(AssistanceRegistry.Domain.Entities.AttendeeStatus.Cancelled, attendee.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarkAsCancelled_ThrowsIfAlreadyCancelled()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Gold");
            attendee.MarkAsCancelled();
            attendee.MarkAsCancelled();
        }

        [TestMethod]
        public void CalculatePriorityScore_StandardNonVip()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", false, "Standard");
            Assert.AreEqual(100, attendee.PriorityScore);
        }

        [TestMethod]
        public void CalculatePriorityScore_PremiumVip()
        {
            var attendee = new AssistanceRegistry.Domain.Entities.Attendee(Guid.NewGuid(), "Test", "test@example.com", true, "Premium");

            Assert.AreEqual(360, attendee.PriorityScore);
        }
    }
}