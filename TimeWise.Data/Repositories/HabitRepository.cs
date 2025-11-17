using Microsoft.EntityFrameworkCore;
using TimeWise.Core.Interfaces;
using TimeWise.Core.Models;
using TimeWise.Data.Context;

namespace TimeWise.Data.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private readonly TimeWiseDbContext _db;
        public HabitRepository(TimeWiseDbContext db) => _db = db;

        public async Task<Habit> AddAsync(Habit habit, CancellationToken ct = default)
        {
            _db.Habits.Add(habit);
            await _db.SaveChangesAsync(ct);
            return habit;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var found = await _db.Habits.FindAsync(new object[] { id }, ct);
            if (found != null)
            {
                _db.Habits.Remove(found);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<Habit?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Habits.FindAsync(new object[] { id }, ct) as Habit;

        public async Task<(IEnumerable<Habit> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Guid? usuarioId, CancellationToken ct = default)
        {
            var query = _db.Habits.AsNoTracking().AsQueryable();
            if (usuarioId.HasValue) 
            {
                query = query.Where(h => h.UsuarioId == usuarioId.Value);
            }
            query = query.OrderByDescending(h => h.CriadoEm);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task UpdateAsync(Habit habit, CancellationToken ct = default)
        {
            _db.Habits.Update(habit);
            await _db.SaveChangesAsync(ct);
        }
    }
}
