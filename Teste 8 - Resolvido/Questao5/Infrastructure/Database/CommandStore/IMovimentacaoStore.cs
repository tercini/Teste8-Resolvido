using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;

namespace Questao5.Infrastructure.Database.CommandStore
{
	public interface IMovimentacaoStore
	{
		Task<MovimentacaoStoreResponse> StoreMovimentacao(MovimentacaoStoreRequest request);
	}
}
