using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Database.QueryStore
{
	public interface IContaCorrenteStore
	{
		Task<ContaCorrente> GetContaCorrente(string idContaCorrente);
	}

}
