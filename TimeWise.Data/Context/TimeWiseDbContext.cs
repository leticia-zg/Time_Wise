using Microsoft.EntityFrameworkCore;
using TimeWise.Core.Models;

namespace TimeWise.Data.Context
{
    public class TimeWiseDbContext : DbContext
    {
        public TimeWiseDbContext(DbContextOptions<TimeWiseDbContext> options) : base(options) { }

        public DbSet<Habit> Habits { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Habit>(eb =>
            {
                eb.ToTable("HABITS");
                eb.HasKey(h => h.Id);
                
                // Configurar GUIDs - Oracle EF Core faz a conversão automaticamente
                eb.Property(h => h.Id)
                    .HasColumnType("RAW(16)");
                
                eb.Property(h => h.UsuarioId)
                    .HasColumnType("RAW(16)");
                
                // Configurar boolean - Oracle EF Core converte automaticamente bool para NUMBER(1)
                eb.Property(h => h.Concluido)
                    .HasColumnType("NUMBER(1)");
                
                // Configurar strings
                eb.Property(h => h.Titulo)
                    .HasMaxLength(200)
                    .IsRequired()
                    .HasColumnType("NVARCHAR2(200)");
                
                // Configurar enum - converter para string no banco
                eb.Property(h => h.Tipo)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasColumnType("NVARCHAR2(50)");
                
                eb.Property(h => h.Descricao)
                    .HasMaxLength(1000)
                    .HasColumnType("NVARCHAR2(1000)");
                
                // Configurar DateTime
                eb.Property(h => h.CriadoEm)
                    .HasColumnType("TIMESTAMP(7)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

