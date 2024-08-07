using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;

namespace Questao5.Application.Handlers
{
	public class MovimentacaoHandler : IRequestHandler<MovimentacaoRequest, MovimentacaoResponse>
	{
        private readonly IMovimentacaoStore _movimentacaoStore;
        private readonly IContaCorrenteStore _contaCorrenteStore;
        private readonly IIdempotenciaStore _idempotenciaStore;

        public MovimentacaoHandler(IMovimentacaoStore movimentacaoStore, IContaCorrenteStore contaCorrenteStore, IIdempotenciaStore idempotenciaStore)
        {
            _movimentacaoStore = movimentacaoStore;
            _contaCorrenteStore = contaCorrenteStore;
            _idempotenciaStore = idempotenciaStore;
        }

        public async Task<MovimentacaoResponse> Handle(MovimentacaoRequest request, CancellationToken cancellationToken)
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

            // Validação: Apenas valores positivos podem ser recebidos
            if (request.Valor <= 0)
            {
                throw new Exception("INVALID_VALUE: O valor da movimentação deve ser positivo.");
            }

            // Validação: Apenas os tipos “débito” ou “crédito” podem ser aceitos
            if (request.TipoMovimento != "D" && request.TipoMovimento != "C")
            {
                throw new Exception("INVALID_TYPE: O tipo de movimentação deve ser 'debito' ou 'credito'.");
            }

            // Se todas as validações passarem, prossiga com a movimentação
            var storeRequest = new MovimentacaoStoreRequest
            {
                Movimento = new Movimento
                {
                    IdMovimento = request.IdRequisicao,
                    IdContaCorrente = request.IdContaCorrente,
                    DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
                    TipoMovimento = request.TipoMovimento,
                    Valor = request.Valor,                    
                }
            };
            var storeResponse = await _movimentacaoStore.StoreMovimentacao(storeRequest);

            if (storeResponse.Sucesso)
            {
                // Salvar idempotência
                var novaIdempotencia = new Idempotencia
                {
                    ChaveIdempotencia = request.IdRequisicao,
                    Requisicao = request.ToString(),
                    Resultado = request.IdRequisicao
                };
                await _idempotenciaStore.SaveAsync(novaIdempotencia);

                return new MovimentacaoResponse { IdMovimento = request.IdRequisicao };
            }
            else
            {
                throw new Exception(storeResponse.Mensagem);
            }
        }
    }
}
