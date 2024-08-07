using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Services;

namespace Questao5.Infrastructure.Services.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SaldoController : ControllerBase
	{
		private readonly IMediator _mediator;

		public SaldoController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{idContaCorrente}")]
		public async Task<IActionResult> Get(string idContaCorrente)
		{

			try
			{
				var request = new SaldoRequest { IdContaCorrente = idContaCorrente };
				var response = await _mediator.Send(request);
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
