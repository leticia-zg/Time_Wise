using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TimeWise.Data.Context
{
	/// <summary>
	/// Factory para criação do DbContext em design-time (migrations)
	/// </summary>
	public class TimeWiseDbContextFactory : IDesignTimeDbContextFactory<TimeWiseDbContext>
	{
		public TimeWiseDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<TimeWiseDbContext>();
			
			// Connection string padrão para design-time
			// Será sobrescrita pela connection string do appsettings.json em runtime
			var connectionString = "User Id=rm558464;Password=170106;Data Source=//oracle.fiap.com.br:1521/ORCL;";
			
			optionsBuilder.UseOracle(connectionString);
			
			return new TimeWiseDbContext(optionsBuilder.Options);
		}
	}
}

