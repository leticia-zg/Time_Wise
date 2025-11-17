using FluentAssertions;
using Moq;
using TimeWise.Core.Interfaces;
using TimeWise.Core.Models;
using TimeWise.Service.Services;
using TimeWise.Service.Interfaces;
using Xunit;

namespace TimeWise.Tests
{
	/// <summary>
	/// Testes unitários para HabitService
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

		[Fact]
		public async Task CreateAsync_Should_Call_Repository_AddAsync()
		{
			// Arrange
			var habit = new Habit
			{
				Id = Guid.NewGuid(),
				UsuarioId = Guid.NewGuid(),
				Titulo = "Teste",
				Tipo = "PAUSA"
			};

			_mockRepository.Setup(r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(habit);

			// Act
			var result = await _service.CreateAsync(habit);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(habit.Id);
			_mockRepository.Verify(r => r.AddAsync(habit, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetByIdAsync_Should_Return_Habit_When_Exists()
		{
			// Arrange
			var habitId = Guid.NewGuid();
			var habit = new Habit
			{
				Id = habitId,
				UsuarioId = Guid.NewGuid(),
				Titulo = "Teste",
				Tipo = "PAUSA"
			};

			_mockRepository.Setup(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(habit);

			// Act
			var result = await _service.GetByIdAsync(habitId);

			// Assert
			result.Should().NotBeNull();
			result!.Id.Should().Be(habitId);
			_mockRepository.Verify(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
		{
			// Arrange
			var habitId = Guid.NewGuid();

			_mockRepository.Setup(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Habit?)null);

			// Act
			var result = await _service.GetByIdAsync(habitId);

			// Assert
			result.Should().BeNull();
			_mockRepository.Verify(r => r.GetByIdAsync(habitId, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetPagedAsync_Should_Return_Paged_Results()
		{
			// Arrange
			var habits = new List<Habit>
			{
				new Habit { Id = Guid.NewGuid(), Titulo = "Hábito 1", Tipo = "PAUSA" },
				new Habit { Id = Guid.NewGuid(), Titulo = "Hábito 2", Tipo = "POSTURA" }
			};

			_mockRepository.Setup(r => r.GetPagedAsync(1, 10, null, It.IsAny<CancellationToken>()))
				.ReturnsAsync((habits, 2));

			// Act
			var (items, totalCount) = await _service.GetPagedAsync(1, 10, null);

			// Assert
			items.Should().HaveCount(2);
			totalCount.Should().Be(2);
			_mockRepository.Verify(r => r.GetPagedAsync(1, 10, null, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetPagedAsync_With_UsuarioId_Should_Filter_By_User()
		{
			// Arrange
			var usuarioId = Guid.NewGuid();
			var habits = new List<Habit>
			{
				new Habit { Id = Guid.NewGuid(), UsuarioId = usuarioId, Titulo = "Hábito 1", Tipo = "PAUSA" }
			};

			_mockRepository.Setup(r => r.GetPagedAsync(1, 10, usuarioId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((habits, 1));

			// Act
			var (items, totalCount) = await _service.GetPagedAsync(1, 10, usuarioId);

			// Assert
			items.Should().HaveCount(1);
			totalCount.Should().Be(1);
			_mockRepository.Verify(r => r.GetPagedAsync(1, 10, usuarioId, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_Should_Call_Repository_UpdateAsync()
		{
			// Arrange
			var habit = new Habit
			{
				Id = Guid.NewGuid(),
				UsuarioId = Guid.NewGuid(),
				Titulo = "Hábito atualizado",
				Tipo = "PAUSA"
			};

			_mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			await _service.UpdateAsync(habit);

			// Assert
			_mockRepository.Verify(r => r.UpdateAsync(habit, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
		{
			// Arrange
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

