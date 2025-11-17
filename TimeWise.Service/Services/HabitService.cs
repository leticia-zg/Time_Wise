using TimeWise.Core.Interfaces;
using TimeWise.Core.Models;
using TimeWise.Service.Interfaces;

namespace TimeWise.Service.Services
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _repo;
        public HabitService(IHabitRepository repo) => _repo = repo;

        public Task<Habit> CreateAsync(Habit habit, CancellationToken ct = default)
            => _repo.AddAsync(habit, ct);

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
            => _repo.DeleteAsync(id, ct);

        public Task<Habit?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _repo.GetByIdAsync(id, ct);

        public Task<(IEnumerable<Habit> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Guid? usuarioId, CancellationToken ct = default)
            => _repo.GetPagedAsync(pageNumber, pageSize, usuarioId, ct);

        public Task UpdateAsync(Habit habit, CancellationToken ct = default)
            => _repo.UpdateAsync(habit, ct);
    }
}

