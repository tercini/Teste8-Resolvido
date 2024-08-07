using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
	public class SaldoRequest : IRequest<SaldoResponse>
	{
		public string IdContaCorrente { get; set; }
        public string IdRequisicao { get; set; }
    }
}
