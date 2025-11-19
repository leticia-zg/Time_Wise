#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TimeWise.Core.Enums;
using TimeWise.Core.Interfaces;
using TimeWise.Core.Models;
using TimeWise.Service.Services;
using TimeWise.Service.Interfaces;
using Xunit;

namespace TimeWise.Tests
{
	/// <summary>
	/// Testes unitários para HabitService
	/// Testa a lógica de negócio do serviço de hábitos usando mocks do repositório
	/// </summary>
	public class HabitServiceTests
	{
		private readonly Mock<IHabitRepository> _mockRepository;
		private readonly IHabitService _service;

		public HabitServiceTests()
		{
			_mockRepository = new Mock<IHabitRepository>();
			_service = new HabitService(_mockRepository.Object);
		}

		/// <summary>
		/// Testa se o método CreateAsync chama corretamente o repositório AddAsync
		/// </summary>
		[Fact]
		public async Task CreateAsync_Should_Call_Repository_AddAsync()
		{
			// Arrange - Preparar dados de teste
			var habit = new Habit
			{
				Id = Guid.NewGuid(),
				UsuarioId = Guid.NewGuid(),
				Titulo = "Teste",
				Tipo = TipoHabit.PAUSA
			};

			_mockRepository.Setup(r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(habit);

			// Act
			var result = await _service.CreateAsync(habit);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(habit.Id, result.Id);
			_mockRepository.Verify(r => r.AddAsync(habit, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se GetByIdAsync retorna o hábito quando ele existe no repositório
		/// </summary>
		[Fact]
		public async Task GetByIdAsync_Should_Return_Habit_When_Exists()
		{
			// Arrange - Preparar dados de teste
			var habitId = Guid.NewGuid();
			var habit = new Habit
			{
				Id = habitId,
				UsuarioId = Guid.NewGuid(),
				Titulo = "Teste",
				Tipo = TipoHabit.PAUSA
			};

			_mockRepository.Setup(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(habit);

			// Act
			var result = await _service.GetByIdAsync(habitId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(habitId, result!.Id);
			_mockRepository.Verify(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se GetByIdAsync retorna null quando o hábito não existe no repositório
		/// </summary>
		[Fact]
		public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
		{
			// Arrange - Preparar dados de teste
			var habitId = Guid.NewGuid();

			_mockRepository.Setup(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Habit?)null);

			// Act
			var result = await _service.GetByIdAsync(habitId);

			// Assert
			Assert.Null(result);
			_mockRepository.Verify(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se GetPagedAsync retorna resultados paginados corretamente
		/// </summary>
		[Fact]
		public async Task GetPagedAsync_Should_Return_Paged_Results()
		{
			// Arrange - Preparar dados de teste
			var habits = new List<Habit>
			{
				new Habit { Id = Guid.NewGuid(), Titulo = "Hábito 1", Tipo = TipoHabit.PAUSA },
				new Habit { Id = Guid.NewGuid(), Titulo = "Hábito 2", Tipo = TipoHabit.POSTURA }
			};

			_mockRepository.Setup(r => r.GetPagedAsync(1, 10, null, It.IsAny<CancellationToken>()))
				.ReturnsAsync((habits, 2));

			// Act
			var (items, totalCount) = await _service.GetPagedAsync(1, 10, null);

			// Assert
			Assert.Equal(2, items.Count());
			Assert.Equal(2, totalCount);
			_mockRepository.Verify(r => r.GetPagedAsync(1, 10, null, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se GetPagedAsync filtra corretamente por UsuarioId quando fornecido
		/// </summary>
		[Fact]
		public async Task GetPagedAsync_With_UsuarioId_Should_Filter_By_User()
		{
			// Arrange - Preparar dados de teste
			var usuarioId = Guid.NewGuid();
			var habits = new List<Habit>
			{
				new Habit { Id = Guid.NewGuid(), UsuarioId = usuarioId, Titulo = "Hábito 1", Tipo = TipoHabit.PAUSA }
			};

			_mockRepository.Setup(r => r.GetPagedAsync(1, 10, usuarioId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((habits, 1));

			// Act
			var (items, totalCount) = await _service.GetPagedAsync(1, 10, usuarioId);

			// Assert
			Assert.Single(items);
			Assert.Equal(1, totalCount);
			_mockRepository.Verify(r => r.GetPagedAsync(1, 10, usuarioId, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se o método UpdateAsync chama corretamente o repositório UpdateAsync
		/// </summary>
		[Fact]
		public async Task UpdateAsync_Should_Call_Repository_UpdateAsync()
		{
			// Arrange - Preparar dados de teste
			var habit = new Habit
			{
				Id = Guid.NewGuid(),
				UsuarioId = Guid.NewGuid(),
				Titulo = "Hábito atualizado",
				Tipo = TipoHabit.PAUSA
			};

			_mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await _service.UpdateAsync(habit);

			// Assert
			_mockRepository.Verify(r => r.UpdateAsync(habit, It.IsAny<CancellationToken>()), Times.Once);
		}

		/// <summary>
		/// Testa se o método DeleteAsync chama corretamente o repositório DeleteAsync
		/// </summary>
		[Fact]
		public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
		{
			// Arrange - Preparar dados de teste
			var habitId = Guid.NewGuid();

			_mockRepository.Setup(r => r.DeleteAsync(habitId, It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await _service.DeleteAsync(habitId);

			// Assert
			_mockRepository.Verify(r => r.DeleteAsync(habitId, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}

