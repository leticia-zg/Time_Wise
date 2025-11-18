using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TimeWise.Data.Context;
using Xunit;

namespace TimeWise.Tests
{
	public class HabitsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public HabitsIntegrationTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					// Replace DbContext with SQLite in-memory for testing
					var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TimeWiseDbContext>));
					if (descriptor != null) services.Remove(descriptor);

					services.AddDbContext<TimeWiseDbContext>(options =>
					{
						options.UseSqlite("DataSource=:memory:");
					});

					// Build provider and create DB
					var sp = services.BuildServiceProvider();
					using var scope = sp.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<TimeWiseDbContext>();
					db.Database.OpenConnection();
					db.Database.EnsureCreated();
				});
			});
		}

		[Fact]
		public async Task Post_CreateAndGet_Returns_Created_And_Can_Get()
		{
			var client = _factory.CreateClient();

			var payload = new
			{
				UsuarioId = Guid.NewGuid(),
				Titulo = "Teste de pausa",
				Descricao = "Levantar e alongar",
				Tipo = "PAUSA"
			};

			var response = await client.PostAsJsonAsync("/api/v1/Habits", payload);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var created = await response.Content.ReadFromJsonAsync<dynamic>();
			((string)response.Headers.Location!.OriginalString).Should().NotBeNullOrEmpty();

			// Get paged
			var getResp = await client.GetAsync("/api/v1/Habits?pageNumber=1&pageSize=10");
			getResp.StatusCode.Should().Be(HttpStatusCode.OK);
			var page = await getResp.Content.ReadFromJsonAsync<dynamic>();
			((int)page.totalCount).Should().BeGreaterOrEqualTo(1);
		}

		[Fact]
		public async Task Post_CreateHabit_WithInvalidData_Returns_BadRequest()
		{
			var client = _factory.CreateClient();

			var payload = new
			{
				UsuarioId = Guid.NewGuid(),
				Titulo = "", // Título vazio deve retornar BadRequest
				Descricao = "Teste",
				Tipo = "PAUSA"
			};

			var response = await client.PostAsJsonAsync("/api/v1/Habits", payload);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Post_CreateHabit_WithoutUsuarioId_GeneratesAutomatically()
		{
			var client = _factory.CreateClient();

			// Criar hábito sem fornecer UsuarioId
			var payload = new
			{
				Titulo = "Hábito sem UsuarioId",
				Descricao = "Teste de geração automática",
				Tipo = "PAUSA"
			};

			var response = await client.PostAsJsonAsync("/api/v1/Habits", payload);
			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var created = await response.Content.ReadFromJsonAsync<dynamic>();
			// Verificar se o UsuarioId foi gerado automaticamente (não deve ser vazio)
			Guid usuarioId = Guid.Parse((string)created.usuarioId);
			usuarioId.Should().NotBeEmpty();
		}

		[Fact]
		public async Task Get_GetById_WithInvalidId_Returns_NotFound()
		{
			var client = _factory.CreateClient();
			var invalidId = Guid.NewGuid();

			var response = await client.GetAsync($"/api/v1/Habits/{invalidId}");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Put_UpdateHabit_Returns_NoContent()
		{
			var client = _factory.CreateClient();
			var usuarioId = Guid.NewGuid();

			// Criar um hábito primeiro
			var createPayload = new
			{
				UsuarioId = usuarioId,
				Titulo = "Hábito original",
				Descricao = "Descrição original",
				Tipo = "PAUSA"
			};

			var createResponse = await client.PostAsJsonAsync("/api/v1/Habits", createPayload);
			createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			var created = await createResponse.Content.ReadFromJsonAsync<dynamic>();
			var habitId = Guid.Parse((string)created.id);

			// Atualizar o hábito
			var updatePayload = new
			{
				UsuarioId = usuarioId,
				Titulo = "Hábito atualizado",
				Descricao = "Descrição atualizada",
				Tipo = "POSTURA",
				Concluido = true
			};

			var updateResponse = await client.PutAsJsonAsync($"/api/v1/Habits/{habitId}", updatePayload);
			updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			// Verificar se foi atualizado
			var getResponse = await client.GetAsync($"/api/v1/Habits/{habitId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			var updated = await getResponse.Content.ReadFromJsonAsync<dynamic>();
			((string)updated.titulo).Should().Be("Hábito atualizado");
			((bool)updated.concluido).Should().BeTrue();
		}

		[Fact]
		public async Task Put_UpdateHabit_WithInvalidId_Returns_NotFound()
		{
			var client = _factory.CreateClient();
			var invalidId = Guid.NewGuid();

			var updatePayload = new
			{
				UsuarioId = Guid.NewGuid(),
				Titulo = "Título",
				Descricao = "Descrição",
				Tipo = "PAUSA",
				Concluido = false
			};

			var response = await client.PutAsJsonAsync($"/api/v1/Habits/{invalidId}", updatePayload);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Delete_DeleteHabit_Returns_NoContent()
		{
			var client = _factory.CreateClient();

			// Criar um hábito primeiro
			var createPayload = new
			{
				UsuarioId = Guid.NewGuid(),
				Titulo = "Hábito para deletar",
				Descricao = "Será deletado",
				Tipo = "PAUSA"
			};

			var createResponse = await client.PostAsJsonAsync("/api/v1/Habits", createPayload);
			createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			var created = await createResponse.Content.ReadFromJsonAsync<dynamic>();
			var habitId = Guid.Parse((string)created.id);

			// Deletar o hábito
			var deleteResponse = await client.DeleteAsync($"/api/v1/Habits/{habitId}");
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			// Verificar se foi deletado
			var getResponse = await client.GetAsync($"/api/v1/Habits/{habitId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Delete_DeleteHabit_WithInvalidId_Returns_NotFound()
		{
			var client = _factory.CreateClient();
			var invalidId = Guid.NewGuid();

			var response = await client.DeleteAsync($"/api/v1/Habits/{invalidId}");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Get_GetPaged_WithPagination_Returns_CorrectResults()
		{
			var client = _factory.CreateClient();
			var usuarioId = Guid.NewGuid();

			// Criar múltiplos hábitos
			for (int i = 1; i <= 5; i++)
			{
				var payload = new
				{
					UsuarioId = usuarioId,
					Titulo = $"Hábito {i}",
					Descricao = $"Descrição {i}",
					Tipo = "PAUSA"
				};
				await client.PostAsJsonAsync("/api/v1/Habits", payload);
			}

			// Testar paginação
			var response = await client.GetAsync("/api/v1/Habits?pageNumber=1&pageSize=2");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var page = await response.Content.ReadFromJsonAsync<dynamic>();
			((int)page.pageNumber).Should().Be(1);
			((int)page.pageSize).Should().Be(2);
			((int)page.totalCount).Should().BeGreaterOrEqualTo(5);

			// Verificar header X-Total-Count
			response.Headers.Contains("X-Total-Count").Should().BeTrue();
		}

		[Fact]
		public async Task Get_GetPaged_WithUsuarioIdFilter_Returns_FilteredResults()
		{
			var client = _factory.CreateClient();
			var usuarioId1 = Guid.NewGuid();
			var usuarioId2 = Guid.NewGuid();

			// Criar hábitos para usuário 1
			for (int i = 1; i <= 3; i++)
			{
				var payload = new
				{
					UsuarioId = usuarioId1,
					Titulo = $"Hábito Usuario1 {i}",
					Tipo = "PAUSA"
				};
				await client.PostAsJsonAsync("/api/v1/Habits", payload);
			}

			// Criar hábitos para usuário 2
			for (int i = 1; i <= 2; i++)
			{
				var payload = new
				{
					UsuarioId = usuarioId2,
					Titulo = $"Hábito Usuario2 {i}",
					Tipo = "PAUSA"
				};
				await client.PostAsJsonAsync("/api/v1/Habits", payload);
			}

			// Filtrar por usuarioId1
			var response = await client.GetAsync($"/api/v1/Habits?usuarioId={usuarioId1}&pageNumber=1&pageSize=10");
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var page = await response.Content.ReadFromJsonAsync<dynamic>();
			((int)page.totalCount).Should().Be(3);
		}
	}
}
