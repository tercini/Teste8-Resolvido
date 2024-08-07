using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore;

namespace Questao5.Application.Services
{
	public class MovimentacaoService :IMovimentacaoService
	{
		private readonly IMovimentacaoStore _movimentacaoStore;

		public MovimentacaoService(IMovimentacaoStore movimentacaoStore)
		{
			_movimentacaoStore = movimentacaoStore;
		}

		public async Task<MovimentacaoResponse> RealizarMovimentacao(MovimentacaoRequest request)
		{
			var storeRequest = new MovimentacaoStoreRequest { Movimento = new Movimento { IdMovimento = request.IdRequisicao, IdContaCorrente = request.IdContaCorrente, DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"), TipoMovimento = request.TipoMovimento, Valor = request.Valor } };
			var storeResponse = await _movimentacaoStore.StoreMovimentacao(storeRequest);

			if (storeResponse.Sucesso)
			{
				return new MovimentacaoResponse { IdMovimento = request.IdRequisicao };
			}
			else
			{
				throw new Exception(storeResponse.Mensagem);
			}
		}
	}
}
