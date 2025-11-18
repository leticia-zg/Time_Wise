using Microsoft.AspNetCore.Mvc;
using TimeWise.API.Dtos;
using TimeWise.API.Helpers;
using TimeWise.Core.Models;
using TimeWise.Core.Enums;
using TimeWise.Service.Interfaces;
using System.Net;

namespace TimeWise.API.Controllers.v1
{
    /// <summary>
    /// Habits
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class HabitsController : ControllerBase
    {
        private readonly IHabitService _service;
        private readonly ILogger<HabitsController> _logger;

        public HabitsController(IHabitService service, ILogger<HabitsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo hábito
        /// </summary>
        /// <param name="dto">Dados do hábito a ser criado</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Hábito criado com links HATEOAS</returns>
        /// <response code="201">Hábito criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(HabitReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] HabitCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo)) return BadRequest("Titulo � obrigat�rio");

            var habit = new Habit
            {
                UsuarioId = dto.UsuarioId,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Tipo = Enum.TryParse<TipoHabit>(dto.Tipo, ignoreCase: true, out var tipoHabit) 
                    ? tipoHabit 
                    : throw new ArgumentException($"Tipo inválido. Valores aceitos: {string.Join(", ", Enum.GetNames(typeof(TipoHabit)))}")
            };

            var created = await _service.CreateAsync(habit, ct);
            _logger.LogInformation("Habit created {Id}", created.Id);

            var read = ToReadDto(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1.0" }, read);
        }

        /// <summary>
        /// Obtém um hábito específico pelo ID
        /// </summary>
        /// <param name="id">ID único do hábito (GUID)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Hábito encontrado com links HATEOAS</returns>
        /// <response code="200">Hábito encontrado</response>
        /// <response code="404">Hábito não encontrado</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(HabitReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(ToReadDto(item));
        }

        /// <summary>
        /// Lista hábitos com paginação e filtro opcional por usuário
        /// </summary>
        /// <param name="usuarioId">ID do usuário para filtrar (opcional)</param>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 50)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Lista paginada de hábitos</returns>
        /// <response code="200">Lista de hábitos retornada com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<HabitReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? usuarioId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            CancellationToken ct = default)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);
            var (items, total) = await _service.GetPagedAsync(pageNumber, pageSize, usuarioId, ct);

            var dtoItems = items.Select(ToReadDto);
            var result = new PagedResult<HabitReadDto>
            {
                Items = dtoItems,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Response.Headers.Append("X-Total-Count", total.ToString());
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um hábito existente
        /// </summary>
        /// <param name="id">ID único do hábito a ser atualizado (GUID)</param>
        /// <param name="dto">Dados atualizados do hábito</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Sem conteúdo (204)</returns>
        /// <response code="204">Hábito atualizado com sucesso</response>
        /// <response code="404">Hábito não encontrado</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] HabitUpdateDto dto, CancellationToken ct)
        {
            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            // Converter string para enum
            if (!Enum.TryParse<TipoHabit>(dto.Tipo, ignoreCase: true, out var tipoHabit))
            {
                return BadRequest($"Tipo inválido. Valores aceitos: {string.Join(", ", Enum.GetNames(typeof(TipoHabit)))}");
            }

            existing.Titulo = dto.Titulo;
            existing.Descricao = dto.Descricao;
            existing.Tipo = tipoHabit;
            existing.Concluido = dto.Concluido;

            await _service.UpdateAsync(existing, ct);
            return NoContent();
        }

        /// <summary>
        /// Remove um hábito pelo ID
        /// </summary>
        /// <param name="id">ID único do hábito a ser removido (GUID)</param>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Sem conteúdo (204)</returns>
        /// <response code="204">Hábito removido com sucesso</response>
        /// <response code="404">Hábito não encontrado</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id, ct);
            return NoContent();
        }

        private HabitReadDto ToReadDto(Habit h)
        {
            var links = new List<object>
            {
                new { rel = "self", href = Url.Action(nameof(GetById), new { id = h.Id, version = "1.0" }), method = "GET" },
                new { rel = "update", href = Url.Action(nameof(Update), new { id = h.Id, version = "1.0" }), method = "PUT" },
                new { rel = "delete", href = Url.Action(nameof(Delete), new { id = h.Id, version = "1.0" }), method = "DELETE" }
            };

            return new HabitReadDto(h.Id, h.UsuarioId, h.Titulo, h.Descricao, h.Tipo.ToString(), h.CriadoEm, h.Concluido, links);
        }
    }
}
