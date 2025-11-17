using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TimeWise.API.Middleware
{
	/// <summary>
	/// Middleware para tratamento global de exceções
	/// </summary>
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError;
			var message = "Ocorreu um erro ao processar a requisição";
			var details = exception.GetType().Name;

			// Tratamento específico para erros de banco de dados
			if (exception is DbUpdateException dbEx)
			{
				code = HttpStatusCode.BadRequest;
				message = "Erro ao acessar o banco de dados";
				
				// Verificar se é erro de tabela não encontrada
				if (dbEx.InnerException?.Message?.Contains("ORA-00942") == true ||
					dbEx.InnerException?.Message?.Contains("table or view does not exist") == true)
				{
					message = "Tabela não encontrada no banco de dados. Verifique se as migrations foram aplicadas.";
					details = "Migration não aplicada";
				}
			}

			var result = JsonSerializer.Serialize(new
			{
				error = message,
				message = exception.Message,
				details = details,
				innerException = exception.InnerException?.Message
			}, new JsonSerializerOptions
			{
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
		}
	}
}

