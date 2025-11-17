using TimeWise.Core.Models;

namespace TimeWise.Service.Interfaces
{
    public interface IHabitService
    {
        Task<Habit> CreateAsync(Habit habit, CancellationToken ct = default);
        Task<Habit?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<(IEnumerable<Habit> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Guid? usuarioId, CancellationToken ct = default);
        Task UpdateAsync(Habit habit, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
