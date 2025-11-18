using System.ComponentModel.DataAnnotations;
using TimeWise.Core.Enums;

namespace TimeWise.API.Dtos
{
    /// <summary>
    /// DTO para criação de um novo hábito
    /// </summary>
    public record HabitCreateDto(
        /// <summary>
        /// ID do usuário proprietário do hábito (opcional - será gerado automaticamente se não fornecido)
        /// </summary>
        Guid? UsuarioId,
        
        /// <summary>
        /// Título do hábito (obrigatório, máximo 200 caracteres)
        /// </summary>
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        string Titulo,
        
        /// <summary>
        /// Descrição detalhada do hábito (opcional, máximo 1000 caracteres)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        string? Descricao,
        
        /// <summary>
        /// Tipo do hábito (PAUSA, POSTURA, HIDRATACAO)
        /// </summary>
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [EnumDataType(typeof(TipoHabit), ErrorMessage = "Tipo deve ser PAUSA, POSTURA ou HIDRATACAO")]
        string Tipo
    );

    /// <summary>
    /// DTO para atualização de um hábito existente
    /// </summary>
    public record HabitUpdateDto(
        /// <summary>
        /// ID do usuário proprietário do hábito
        /// </summary>
        [Required(ErrorMessage = "UsuarioId é obrigatório")]
        Guid UsuarioId,
        
        /// <summary>
        /// Título do hábito (obrigatório, máximo 200 caracteres)
        /// </summary>
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        string Titulo,
        
        /// <summary>
        /// Descrição detalhada do hábito (opcional, máximo 1000 caracteres)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        string? Descricao,
        
        /// <summary>
        /// Tipo do hábito (PAUSA, POSTURA, HIDRATACAO)
        /// </summary>
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [EnumDataType(typeof(TipoHabit), ErrorMessage = "Tipo deve ser PAUSA, POSTURA ou HIDRATACAO")]
        string Tipo,
        
        /// <summary>
        /// Indica se o hábito foi concluído
        /// </summary>
        bool Concluido
    );

    /// <summary>
    /// DTO para leitura de um hábito (resposta da API)
    /// </summary>
    public record HabitReadDto(
        /// <summary>
        /// ID único do hábito
        /// </summary>
        Guid Id,
        
        /// <summary>
        /// ID do usuário proprietário do hábito
        /// </summary>
        Guid UsuarioId,
        
        /// <summary>
        /// Título do hábito
        /// </summary>
        string Titulo,
        
        /// <summary>
        /// Descrição detalhada do hábito
        /// </summary>
        string? Descricao,
        
        /// <summary>
        /// Tipo do hábito (ex: PAUSA, POSTURA, HIDRATACAO)
        /// </summary>
        string Tipo,
        
        /// <summary>
        /// Data e hora de criação do hábito (UTC)
        /// </summary>
        DateTime CriadoEm,
        
        /// <summary>
        /// Indica se o hábito foi concluído
        /// </summary>
        bool Concluido,
        
        /// <summary>
        /// Links HATEOAS para ações relacionadas ao hábito
        /// </summary>
        IEnumerable<object> Links
    );
}
