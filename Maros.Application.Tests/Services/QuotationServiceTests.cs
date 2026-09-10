using Maros.Application.Common;
using Maros.Application.DTOs.Quotations;
using Maros.Application.Interfaces;
using Maros.Application.Services;
using Maros.Domain.Entities;
using Maros.Domain.Interfaces;
using Moq;
using Xunit;

namespace Maros.Application.Tests.Services;

public class QuotationServiceTests
{
    private readonly Mock<IQuotationRepository> _quotationRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IPaginationService> _paginationService = new();
    private readonly QuotationService _sut;

    public QuotationServiceTests()
    {
        _sut = new QuotationService(_quotationRepository.Object, _customerRepository.Object, _paginationService.Object);
    }

    private static QuotationCreateDto BuildRequest(string phone) => new(
        "María López", phone, null, "Valledupar",
        new List<QuotationItemInputDto> { new(Guid.NewGuid(), "M", 1, new List<Guid>(), null) },
        new List<string>(), "Notas"
    );

    [Fact]
    public async Task CreateAsync_ClienteExistentePorTelefono_ReutilizaElMismoCliente_NoCreaUnoNuevo()
    {
        var clienteExistente = new Customer { Id = Guid.NewGuid(), Name = "María López", Phone = "3001234567" };
        _customerRepository.Setup(r => r.GetByPhoneAsync("3001234567")).ReturnsAsync(clienteExistente);
        _quotationRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Quotation { Customer = clienteExistente });

        await _sut.CreateAsync(BuildRequest("3001234567"));

        _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_TelefonoNuevo_CreaUnClienteNuevo()
    {
        _customerRepository.Setup(r => r.GetByPhoneAsync("3009999999")).ReturnsAsync((Customer?)null);
        _quotationRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Quotation { Customer = new Customer { Name = "Nuevo" } });

        await _sut.CreateAsync(BuildRequest("3009999999"));

        _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SinItems_LanzaAppException400()
    {
        var request = new QuotationCreateDto(
            "María", "3001234567", null, "Valledupar",
            new List<QuotationItemInputDto>(), // vacío
            new List<string>(), ""
        );

        var ex = await Assert.ThrowsAsync<AppException>(() => _sut.CreateAsync(request));
        Assert.Equal(400, ex.StatusCode);
    }
}