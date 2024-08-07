using Dapper;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Database.QueryStore
{
	public class ContaCorrenteStore : IContaCorrenteStore
	{
		private readonly IDbConnection _connection;

		public ContaCorrenteStore(IDbConnection connection)
		{
			_connection = connection;
		}

		public async Task<ContaCorrente> GetContaCorrente(string idContaCorrente)
		{
			var sql = "SELECT * FROM ContaCorrente WHERE IdContaCorrente = @IdContaCorrente";
			var parameters = new { IdContaCorrente = idContaCorrente };

			var contaCorrente = await _connection.QuerySingleOrDefaultAsync<ContaCorrente>(sql, parameters);
			return contaCorrente;
		}
	}
}
