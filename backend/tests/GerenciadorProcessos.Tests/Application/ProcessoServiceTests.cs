using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.Services;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Interfaces;
using GerenciadorProcessos.Shared.Time;
using Moq;
using Xunit;

namespace GerenciadorProcessos.Tests.Application;

public class ProcessoServiceTests
{
    private readonly Mock<IProcessoRepository> _mockRepo;
    private readonly Mock<IEntidadeLegalRepository> _mockEntidadeRepo;
    private readonly BrazilianTimeProvider _timeProvider;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProcessoService _service;

    public ProcessoServiceTests()
    {
        _mockRepo = new Mock<IProcessoRepository>();
        _mockEntidadeRepo = new Mock<IEntidadeLegalRepository>();
        _timeProvider = new BrazilianTimeProvider();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new ProcessoService(
            _mockRepo.Object, 
            _mockEntidadeRepo.Object, 
            _timeProvider, 
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnMappedDto()
    {
        // Arrange
        var partesRequest = new List<AddParteProcessoDto>
        {
            new AddParteProcessoDto(TipoPolo.Ativo, null, new CreateEntidadeLegalDto("Ativo", TipoEntidade.PessoaFisica, "11122233396")),
            new AddParteProcessoDto(TipoPolo.Passivo, null, new CreateEntidadeLegalDto("Passivo", TipoEntidade.PessoaFisica, "44455566619"))
        };
        var dto = new CreateProcessoDto("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Teste", partesRequest);

        _mockEntidadeRepo.Setup(r => r.GetByDocumentoAsync(It.IsAny<string>())).ReturnsAsync((EntidadeLegal?)null);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Processo>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.NumeroProcesso.Should().Be(dto.NumeroProcesso);
        result.Assunto.Should().Be(dto.Assunto);
        result.Status.Should().Be(StatusProcesso.Ativo);
        result.Partes.Should().HaveCount(2);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Processo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProcessoExists_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entidadeAtiva = new EntidadeLegal("Entidade Ativa", TipoEntidade.PessoaFisica, "77788899941");
        var entidadePassiva = new EntidadeLegal("Entidade Passiva", TipoEntidade.PessoaFisica, "44455566619");
        var partes = new List<Parte> { new Parte(TipoPolo.Ativo, entidadeAtiva), new Parte(TipoPolo.Passivo, entidadePassiva) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Velho", DateTimeOffset.UtcNow, partes);

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(processo);
        var updateDto = new UpdateProcessoDto("Novo Assunto", StatusProcesso.Ativo);

        // Act
        var result = await _service.UpdateAsync(id, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Assunto.Should().Be("Novo Assunto");
        result.Status.Should().Be(StatusProcesso.Ativo);
        _mockRepo.Verify(r => r.UpdateAsync(processo), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProcessoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Processo?)null);
        var dto = new UpdateProcessoDto("Assunto", StatusProcesso.Ativo);

        // Act
        var result = await _service.UpdateAsync(Guid.NewGuid(), dto);

        // Assert
        result.Should().BeNull();
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Processo>()), Times.Never);
    }

    [Fact]
    public async Task AddParteAsync_WhenProcessoExists_ShouldAddParte()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entidadeAtiva = new EntidadeLegal("Entidade Ativa", TipoEntidade.PessoaFisica, "77788899941");
        var entidadePassiva = new EntidadeLegal("Entidade Passiva", TipoEntidade.PessoaFisica, "44455566619");
        var partes = new List<Parte> { new Parte(TipoPolo.Ativo, entidadeAtiva), new Parte(TipoPolo.Passivo, entidadePassiva) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(processo);

        var novaEntidade = new EntidadeLegal("Nova Entidade", TipoEntidade.PessoaFisica, "11122233396");
        _mockEntidadeRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(novaEntidade);

        var dto = new AddParteDto(TipoPolo.Ativo, Guid.NewGuid(), null);

        // Act
        var result = await _service.AddParteAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Partes.Should().HaveCount(3);
        result.Partes.Should().Contain(p => p.EntidadeLegal.Nome == "Nova Entidade");
        _mockRepo.Verify(r => r.UpdateAsync(processo), Times.Once);
    }
}
