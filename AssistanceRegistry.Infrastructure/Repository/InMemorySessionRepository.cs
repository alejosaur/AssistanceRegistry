using AssistanceRegistry.Domain.Entities;
using AssistanceRegistry.Domain.Interfaces;
using System.Collections.Concurrent;

namespace AssistanceRegistry.Infrastructure.Repository
{
    public class InMemorySessionRepository : ISessionRepository
    {
        private readonly ConcurrentDictionary<string, Session> _sessions = new(
            [
                new( "123", new Session("123", "Session 1", 3))   
            ]
        );


        public Task<Session?> FindByIdAsync(string id)
        {
            _sessions.TryGetValue(id, out var session);
            return Task.FromResult(session);
        }

        public Task SaveAsync(Session session)
        {
            _sessions[session.Id] = session;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Session>> FindAllAsync()
        {
            return Task.FromResult((IEnumerable<Session>)_sessions.Values);
        }
    }
}