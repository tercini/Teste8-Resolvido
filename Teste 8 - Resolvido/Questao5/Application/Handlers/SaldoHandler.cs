using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore;

namespace Questao5.Application.Handlers
{
	public class SaldoHandler : IRequestHandler<SaldoRequest, SaldoResponse>
	{
        private readonly ISaldoStore _saldoStore;
        private readonly IContaCorrenteStore _contaCorrenteStore;
        private readonly IIdempotenciaStore _idempotenciaStore;

        public SaldoHandler(ISaldoStore saldoStore, IContaCorrenteStore contaCorrenteStore, IIdempotenciaStore idempotenciaStore)
        {
            _saldoStore = saldoStore;
            _contaCorrenteStore = contaCorrenteStore;
            _idempotenciaStore = idempotenciaStore;
        }

        public async Task<SaldoResponse> Handle(SaldoRequest request, CancellationToken cancellationToken)
        {
            // Verificar idempotência
            var idempotencia = await _idempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao);
            if (idempotencia != null)
            {
                throw new Exception("DUPLICATE_REQUEST: esta operação já foi realizada anteriormente.");
            }

            // Validação: Apenas contas correntes cadastradas podem receber movimentação
            var contaCorrente = await _contaCorrenteStore.GetContaCorrente(request.IdContaCorrente);
            if (contaCorrente == null)
            {
                throw new Exception("INVALID_ACCOUNT: A conta corrente não está cadastrada.");
            }

            // Validação: Apenas contas correntes ativas podem receber movimentação
            if (contaCorrente.Ativo != 1)
            {
                throw new Exception("INACTIVE_ACCOUNT: A conta corrente não está ativa.");
            }

            var storeRequest = new SaldoStoreRequest { IdContaCorrente = request.IdContaCorrente };
            var storeResponse = await _saldoStore.GetSaldo(storeRequest);

            if (storeResponse.Sucesso)
            {
                var response = new SaldoResponse
                {
                    ValorSaldo = storeResponse.Saldo,
                    NomeTitular = contaCorrente.Nome,
                    NumeroContaCorrente = contaCorrente.Numero,
                    DataHoraResposta = DateTime.Now.ToString()
                };

                // Salvar idempotência
                var novaIdempotencia = new Idempotencia
                {
                    ChaveIdempotencia = request.IdRequisicao,
                    Requisicao = request.IdContaCorrente.ToString(),
                    Resultado = response.ValorSaldo.ToString()
                };
                await _idempotenciaStore.SaveAsync(novaIdempotencia);
                return response;
            }
            else
            {
                throw new Exception(storeResponse.Mensagem);
            }
        }
    }
}
