using Dapper;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class IdempotenciaStore : IIdempotenciaStore
    {
        private readonly IDbConnection _connection;

        public IdempotenciaStore(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Idempotencia> GetByChaveIdempotenciaAsync(string chaveIdempotencia)
        {
            var sql = "SELECT * FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia";
            var parameters = new { ChaveIdempotencia = chaveIdempotencia };

            return await _connection.QuerySingleOrDefaultAsync<Idempotencia>(sql, parameters);
        }

        public async Task SaveAsync(Idempotencia idempotencia)
        {
            var sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            var parameters = new
            {
                idempotencia.ChaveIdempotencia,
                idempotencia.Requisicao,
                idempotencia.Resultado
            };

            await _connection.ExecuteAsync(sql, parameters);
        }
    }
}
