using System;

namespace AssistanceRegistry.Domain.Entities
{
    public enum AttendeeStatus
    {
        Pending,
        Registered,
        Waitlisted,
        Cancelled
    }

    public enum RegistrationTier
    {
        Standard = 1,
        Premium = 2,
        Gold = 3
    }

    public class Attendee
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public AttendeeStatus Status { get; private set; }
        public bool IsVip { get; set; }
        public int PriorityScore { get; set; }
        public RegistrationTier RegistrationTier { get; set; }

        public Attendee(Guid id, string name, string email, bool isVip, string registrationTier)
        {
            Id = id;
            Name = name;
            Email = email;
            Status = AttendeeStatus.Pending;
            IsVip = isVip;
            RegistrationTier = Enum.TryParse<RegistrationTier>(registrationTier, true, out var tier) ? tier : RegistrationTier.Standard;

            CalculatePriorityScore();
        }

        public void MarkAsRegistered(DateTime registrationDate)
        {
            if (Status == AttendeeStatus.Registered || Status == AttendeeStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot register attendee.");
            }
            Status = AttendeeStatus.Registered;
            RegistrationDate = registrationDate;
        }

        public void MarkAsWaitlisted(DateTime registrationDate)
        {
            if (Status == AttendeeStatus.Waitlisted || Status == AttendeeStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot waitlist attendee.");
            }
            Status = AttendeeStatus.Waitlisted;
            RegistrationDate = registrationDate;
        }

        public void MarkAsCancelled()
        {
            if (Status == AttendeeStatus.Cancelled)
            {
                throw new InvalidOperationException("Attendee is already cancelled.");
            }
            Status = AttendeeStatus.Cancelled;
        }

        private void CalculatePriorityScore()
        {
            double currentScore = 0;

            currentScore += (int)RegistrationTier * 100; 

            if (IsVip)
            {
                currentScore = currentScore * 0.8; 
            }

            PriorityScore = (int)Math.Round(currentScore);
        }
    }
}