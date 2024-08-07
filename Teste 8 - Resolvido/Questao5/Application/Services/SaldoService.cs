using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore;

namespace Questao5.Application.Services
{
	public class SaldoService : ISaldoService
	{
		private readonly ISaldoStore _saldoStore;

		public SaldoService(ISaldoStore saldoStore)
		{
			_saldoStore = saldoStore;
		}

		public async Task<SaldoResponse> ConsultarSaldo(string idContaCorrente)
		{
			var storeRequest = new SaldoStoreRequest { IdContaCorrente = idContaCorrente };
			var storeResponse = await _saldoStore.GetSaldo(storeRequest);

			return new SaldoResponse { ValorSaldo = storeResponse.Saldo };
		}
	}
}
