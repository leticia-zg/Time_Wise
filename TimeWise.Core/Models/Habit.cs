using TimeWise.Core.Enums;

namespace TimeWise.Core.Models
{
    public class Habit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descricao { get; set; }
        public TipoHabit Tipo { get; set; } = TipoHabit.PAUSA;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public bool Concluido { get; set; } = false;
    }
}
