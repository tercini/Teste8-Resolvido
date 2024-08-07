using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Services
{
	public interface ISaldoService
	{
		Task<SaldoResponse> ConsultarSaldo(string idContaCorrente);
	}
}
