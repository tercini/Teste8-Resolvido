using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.CommandStore.Requests;
using Questao5.Infrastructure.Database.CommandStore.Responses;
using Questao5.Infrastructure.Database.QueryStore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Questao5.Tests.Application.Handlers
{
    public class MovimentacaoHandlerTests
    {
        private readonly IMovimentacaoStore _mockMovimentacaoStore;
        private readonly IContaCorrenteStore _mockContaCorrenteStore;
        private readonly IIdempotenciaStore _mockIdempotenciaStore;
        private readonly MovimentacaoHandler _movimentacaoHandler;

        public MovimentacaoHandlerTests()
        {
            _mockMovimentacaoStore = Substitute.For<IMovimentacaoStore>();
            _mockContaCorrenteStore = Substitute.For<IContaCorrenteStore>();
            _mockIdempotenciaStore = Substitute.For<IIdempotenciaStore>();
            _movimentacaoHandler = new MovimentacaoHandler(_mockMovimentacaoStore, _mockContaCorrenteStore, _mockIdempotenciaStore);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenIdempotenciaExists()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns(new Idempotencia());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _movimentacaoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("DUPLICATE_REQUEST: esta operação já foi realizada anteriormente.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenContaCorrenteDoesNotExist()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id", IdContaCorrente = "invalid-account" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns((ContaCorrente)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _movimentacaoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT: A conta corrente não está cadastrada.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenContaCorrenteIsInactive()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id", IdContaCorrente = "inactive-account" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns(new ContaCorrente { Ativo = 0 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _movimentacaoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INACTIVE_ACCOUNT: A conta corrente não está ativa.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenValorIsNonPositive()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id", IdContaCorrente = "valid-account", Valor = -100 };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns(new ContaCorrente { Ativo = 1 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _movimentacaoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_VALUE: O valor da movimentação deve ser positivo.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTipoMovimentoIsInvalid()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id", IdContaCorrente = "valid-account", Valor = 100, TipoMovimento = "X" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns(new ContaCorrente { Ativo = 1 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _movimentacaoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_TYPE: O tipo de movimentação deve ser 'debito' ou 'credito'.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnMovimentacaoResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdRequisicao = "test-id", IdContaCorrente = "valid-account", Valor = 100, TipoMovimento = "D" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns(new ContaCorrente { Ativo = 1 });
            _mockMovimentacaoStore.StoreMovimentacao(Arg.Any<MovimentacaoStoreRequest>())
                .Returns(new MovimentacaoStoreResponse { Sucesso = true });

            // Act
            var response = await _movimentacaoHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.IdRequisicao, response.IdMovimento);
            await _mockIdempotenciaStore.Received(1).SaveAsync(Arg.Any<Idempotencia>());
        }
    }
}
