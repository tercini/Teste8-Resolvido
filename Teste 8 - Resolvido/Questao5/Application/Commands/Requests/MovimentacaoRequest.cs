using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
	public class MovimentacaoRequest: IRequest<MovimentacaoResponse>
	{
		public string IdRequisicao { get; set; }
		public string IdContaCorrente { get; set; }
		public double Valor { get; set; }
		public string TipoMovimento { get; set; }
	}
}
