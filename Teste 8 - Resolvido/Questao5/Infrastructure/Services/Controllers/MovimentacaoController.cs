using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Services;

namespace Questao5.Infrastructure.Services.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MovimentacaoController : ControllerBase
	{
		private readonly IMediator _mediator;

		public MovimentacaoController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<IActionResult> Post(MovimentacaoRequest request)
		{
			try
			{
				var response = await _mediator.Send(request);

				// Se a movimentação for bem-sucedida, retorne HTTP 200 e o ID do movimento no corpo da resposta
				return Ok(response);
			}
			catch (Exception ex)
			{
				// Se houver uma falha, retorne HTTP 400 e a mensagem de erro no corpo da resposta
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
