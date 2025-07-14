namespace AssistanceRegistry.Domain.Entities
{
    public class Session
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int MaxCapacity { get; private set; }
        private List<Attendee> _registeredAttendees = new List<Attendee>();
        public List<Attendee> _waitlistedAttendees = new List<Attendee>();

        public IReadOnlyList<Attendee> RegisteredAttendees => _registeredAttendees.AsReadOnly();
        public IReadOnlyList<Attendee> WaitlistedAttendees => _waitlistedAttendees.AsReadOnly();

        public Session(string id, string name, int maxCapacity)
        {
            Id = id;
            Name = name;
            MaxCapacity = maxCapacity;
        }

        public bool IsCapacityAvailable() => _registeredAttendees.Count < MaxCapacity;

        public void RegisterAttendee(Attendee newAttendee)
        {
            if (!IsCapacityAvailable())
            {
                throw new InvalidOperationException("No capacity available.");
            }
            newAttendee.MarkAsRegistered(DateTime.Now);
            _registeredAttendees.Add(newAttendee);
        }

        public void AddAttendeeToWaitlist(Attendee newAttendee)
        {
            newAttendee.MarkAsWaitlisted(DateTime.Now);
            _waitlistedAttendees.Add(newAttendee);
        }

        public bool CancelAttendee(Guid attendeeId)
        {
            var attendee = _registeredAttendees.Find(a => a.Id == attendeeId);

            if (attendee != null)
            {
                attendee.MarkAsCancelled();
                if (_registeredAttendees.Remove(attendee))
                {
                    // Attempt to promote the next attendee from the waitlist if available
                    PromoteFromWaitlist();
                    return true;
                }
            }
            return false;
        }

        public Attendee? PromoteFromWaitlist()
        {
            if (_waitlistedAttendees.Count > 0 && IsCapacityAvailable())
            {
                var attendeeToPromote = _waitlistedAttendees.OrderByDescending(i => i.PriorityScore).First();
                _waitlistedAttendees.Remove(attendeeToPromote);
                attendeeToPromote.MarkAsRegistered(DateTime.Now);
                _registeredAttendees.Add(attendeeToPromote);
                return attendeeToPromote;
            }
            return null;
        }

        // Proposed new method for capacity warning
        public bool getWarningStatus80()
        {
            if (MaxCapacity > 0)
            {
                if (_registeredAttendees.Count > 0)
                {
                    if (_registeredAttendees != null)
                    {
                        double currentOccupancyPercentage = (double)_registeredAttendees.Count / MaxCapacity;

                        if (currentOccupancyPercentage >= 0.8)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool getWarningStatus50()
        {
            if (MaxCapacity > 0)
            {
                if (_registeredAttendees.Count > 0)
                {
                    if (_registeredAttendees != null)
                    {
                        double currentOccupancyPercentage = (double)_registeredAttendees.Count / MaxCapacity;

                        if (currentOccupancyPercentage >= 0.5)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}