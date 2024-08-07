using NSubstitute;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Database.QueryStore.Requests;
using Questao5.Infrastructure.Database.QueryStore.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Questao5.Tests.Application.Handlers
{
    public class SaldoHandlerTests
    {
        private readonly ISaldoStore _mockSaldoStore;
        private readonly IContaCorrenteStore _mockContaCorrenteStore;
        private readonly IIdempotenciaStore _mockIdempotenciaStore;
        private readonly SaldoHandler _saldoHandler;

        public SaldoHandlerTests()
        {
            _mockSaldoStore = Substitute.For<ISaldoStore>();
            _mockContaCorrenteStore = Substitute.For<IContaCorrenteStore>();
            _mockIdempotenciaStore = Substitute.For<IIdempotenciaStore>();
            _saldoHandler = new SaldoHandler(_mockSaldoStore, _mockContaCorrenteStore, _mockIdempotenciaStore);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenIdempotenciaExists()
        {
            // Arrange
            var request = new SaldoRequest { IdRequisicao = "test-id" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns(new Idempotencia());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _saldoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("DUPLICATE_REQUEST: esta operação já foi realizada anteriormente.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenContaCorrenteDoesNotExist()
        {
            // Arrange
            var request = new SaldoRequest { IdRequisicao = "test-id", IdContaCorrente = "invalid-account" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns((ContaCorrente)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _saldoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT: A conta corrente não está cadastrada.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenContaCorrenteIsInactive()
        {
            // Arrange
            var request = new SaldoRequest { IdRequisicao = "test-id", IdContaCorrente = "inactive-account" };
            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao)
                .Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente)
                .Returns(new ContaCorrente { Ativo = 0 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _saldoHandler.Handle(request, CancellationToken.None));
            Assert.Equal("INACTIVE_ACCOUNT: A conta corrente não está ativa.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnSaldoResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new SaldoRequest { IdRequisicao = "test-id", IdContaCorrente = "valid-account" };
            var contaCorrente = new ContaCorrente { Ativo = 1, Nome = "Test User", Numero = 12345 };
            var saldoStoreResponse = new SaldoStoreResponse { Sucesso = true, Saldo = 1000 };

            _mockIdempotenciaStore.GetByChaveIdempotenciaAsync(request.IdRequisicao).Returns((Idempotencia)null);
            _mockContaCorrenteStore.GetContaCorrente(request.IdContaCorrente).Returns(contaCorrente);
            _mockSaldoStore.GetSaldo(Arg.Any<SaldoStoreRequest>()).Returns(saldoStoreResponse);

            // Act
            var response = await _saldoHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(saldoStoreResponse.Saldo, response.ValorSaldo);
            Assert.Equal(contaCorrente.Nome, response.NomeTitular);
            Assert.Equal(contaCorrente.Numero, response.NumeroContaCorrente);
            await _mockIdempotenciaStore.Received(1).SaveAsync(Arg.Is<Idempotencia>(i =>
                i.ChaveIdempotencia == request.IdRequisicao &&
                i.Requisicao == request.IdContaCorrente.ToString() &&
                i.Resultado == response.ValorSaldo.ToString()
            ));
        }
    }
}
