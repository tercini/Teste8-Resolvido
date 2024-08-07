using Dapper;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using System.Data;

namespace Questao5.Infrastructure.Database.QueryStore
{
	public class SaldoStore : ISaldoStore
	{
		private readonly IDbConnection _connection;

		public SaldoStore(IDbConnection connection)
		{
			_connection = connection;
		}

		public async Task<SaldoStoreResponse> GetSaldo(SaldoStoreRequest request)
		{
			var sql = "SELECT SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END) AS Saldo FROM movimento WHERE idcontacorrente = @IdContaCorrente";
			var parameters = new { request.IdContaCorrente };

			var saldo = await _connection.QuerySingleOrDefaultAsync<double?>(sql, parameters);

			if (saldo.HasValue)
			{
				return new SaldoStoreResponse { Saldo = saldo.Value, Sucesso = true, Mensagem = "Saldo recuperado com sucesso" };
			}
			else
			{
				return new SaldoStoreResponse { Saldo = 0.0, Sucesso = false, Mensagem = "Falha ao recuperar o saldo" };
			}

		}
		
	}
}
