using AssistanceRegistry.Domain.Entities;

namespace AssistanceRegistry.Domain.Interfaces
{
    public interface ISessionRepository
    {
        Task<Session?> FindByIdAsync(string id);

        Task SaveAsync(Session session);

        Task<IEnumerable<Session>> FindAllAsync();
    }
}