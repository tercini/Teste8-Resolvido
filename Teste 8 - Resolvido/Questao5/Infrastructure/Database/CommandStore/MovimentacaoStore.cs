using Dapper;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using System.Data;

namespace Questao5.Infrastructure.Database.CommandStore
{
	public class MovimentacaoStore : IMovimentacaoStore
	{
		private readonly IDbConnection _connection;

		public MovimentacaoStore(IDbConnection connection)
		{
			_connection = connection;
		}

		public async Task<MovimentacaoStoreResponse> StoreMovimentacao(MovimentacaoStoreRequest request)
		{
			var sql = "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
			var parameters = new { request.Movimento.IdMovimento, request.Movimento.IdContaCorrente, request.Movimento.DataMovimento, request.Movimento.TipoMovimento, request.Movimento.Valor };

			var affectedRows = await _connection.ExecuteAsync(sql, parameters);
			return new MovimentacaoStoreResponse { Sucesso = affectedRows > 0, Mensagem = affectedRows > 0 ? "Movimentação realizada com sucesso" : "Falha ao realizar movimentação" };
		}
	}
}
