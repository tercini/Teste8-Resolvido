using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public interface IIdempotenciaStore
    {
        Task<Idempotencia> GetByChaveIdempotenciaAsync(string chaveIdempotencia);
        Task SaveAsync(Idempotencia idempotencia);
    }
}
