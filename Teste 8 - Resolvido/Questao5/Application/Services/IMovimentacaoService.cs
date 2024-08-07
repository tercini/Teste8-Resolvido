using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Services
{
	public interface IMovimentacaoService
	{
		Task<MovimentacaoResponse> RealizarMovimentacao(MovimentacaoRequest request);
	}
}
