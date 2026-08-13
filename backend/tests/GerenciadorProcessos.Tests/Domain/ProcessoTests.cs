using System;
using System.Collections.Generic;
using FluentAssertions;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Exceptions;
using Xunit;

namespace GerenciadorProcessos.Tests.Domain;

public class ProcessoTests
{
    private Parte CreateParte(TipoPolo polo)
    {
        var entidade = new EntidadeLegal("Entidade Teste", TipoEntidade.PessoaFisica, "11122233396");
        return new Parte(polo, entidade);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeCorrectly()
    {
        // Arrange
        var numero = "0001234-56.2023.8.26.0100";
        var assunto = "Ação de Teste";
        var dataCriacao = DateTimeOffset.UtcNow;
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };

        // Act
        var processo = new Processo(numero, TipoProcesso.Judicial, assunto, dataCriacao, partes);

        // Assert
        processo.Id.Should().NotBeEmpty();
        processo.NumeroProcesso.Should().Be(numero);
        processo.TipoProcesso.Should().Be(TipoProcesso.Judicial);
        processo.Assunto.Should().Be(assunto);
        processo.DataCriacao.Should().Be(dataCriacao);
        processo.Status.Should().Be(StatusProcesso.Ativo);
        processo.Partes.Should().HaveCount(2);
        processo.Andamentos.Should().BeEmpty();
        processo.FlagDeleted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithoutMinimumPartes_ShouldThrowDomainException()
    {
        // Arrange
        var numero = "0001234-56.2023.8.26.0100";
        var assunto = "Ação de Teste";
        var dataCriacao = DateTimeOffset.UtcNow;
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo) }; // Missing Passivo

        // Act
        Action act = () => new Processo(numero, TipoProcesso.Judicial, assunto, dataCriacao, partes);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*ao menos uma parte no polo Ativo e uma parte no polo Passivo*");
    }

    [Fact]
    public void UpdateAssunto_WhenProcessoIsAtivo_ShouldUpdateAssunto()
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto Velho", DateTimeOffset.UtcNow, partes);
        var novoAssunto = "Assunto Novo";

        // Act
        processo.UpdateAssunto(novoAssunto);

        // Assert
        processo.Assunto.Should().Be(novoAssunto);
    }

    [Theory]
    [InlineData(StatusProcesso.Arquivado)]
    [InlineData(StatusProcesso.Finalizado)]
    public void UpdateAssunto_WhenProcessoIsNotAtivo_ShouldThrowException(StatusProcesso status)
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);
        processo.ChangeStatus(status);

        // Act
        Action act = () => processo.UpdateAssunto("Novo Assunto");

        // Assert
        act.Should().Throw<ProcessoArquivadoException>();
    }

    [Fact]
    public void AddParte_WhenProcessoIsAtivo_ShouldAddParte()
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);
        var novaParte = CreateParte(TipoPolo.Ativo);

        // Act
        processo.AddParte(novaParte);

        // Assert
        processo.Partes.Should().Contain(novaParte);
    }

    [Fact]
    public void AddParte_WhenProcessoIsArquivado_ShouldThrowException()
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);
        processo.ChangeStatus(StatusProcesso.Arquivado);
        var novaParte = CreateParte(TipoPolo.Ativo);

        // Act
        Action act = () => processo.AddParte(novaParte);

        // Assert
        act.Should().Throw<ProcessoArquivadoException>();
    }

    [Fact]
    public void AddParte_WhenEntidadeLegalIsDuplicate_ShouldThrowDomainException()
    {
        // Arrange
        var entidade = new EntidadeLegal("Entidade", TipoEntidade.PessoaFisica, "11122233396");
        var parte1 = new Parte(TipoPolo.Ativo, entidade);
        var parte2 = new Parte(TipoPolo.Passivo, entidade);

        var partes = new List<Parte> { parte1, CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);

        // Act
        Action act = () => processo.AddParte(parte2);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*já é uma parte ativa*");
    }

    [Fact]
    public void RemoveParte_WhenParteExists_ShouldRemoveParteSoftly()
    {
        // Arrange
        var parteAtivo = CreateParte(TipoPolo.Ativo);
        var partePassivo = CreateParte(TipoPolo.Passivo);
        var partes = new List<Parte> { parteAtivo, partePassivo };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);

        // Act
        processo.RemoveParte(parteAtivo.Id);

        // Assert
        parteAtivo.FlagDeleted.Should().BeTrue();
    }

    [Fact]
    public void RemoveParte_WhenParteDoesNotExist_ShouldThrowParteNotFoundException()
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);

        // Act
        Action act = () => processo.RemoveParte(Guid.NewGuid());

        // Assert
        act.Should().Throw<ParteNotFoundException>();
    }

    [Fact]
    public void AddAndamento_WithValidDate_ShouldAddAndamento()
    {
        // Arrange
        var creationDate = DateTimeOffset.UtcNow.AddDays(-5);
        var systemNow = DateTimeOffset.UtcNow;
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", creationDate, partes);
        var andamento = new Andamento(systemNow.AddDays(-1), "Andamento 1");

        // Act
        processo.AddAndamento(andamento, systemNow);

        // Assert
        processo.Andamentos.Should().Contain(andamento);
    }

    [Fact]
    public void AddAndamento_WhenDateIsFuture_ShouldThrowException()
    {
        // Arrange
        var systemNow = DateTimeOffset.UtcNow;
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", systemNow.AddDays(-1), partes);
        var andamento = new Andamento(systemNow.AddDays(1), "Andamento no Futuro");

        // Act
        Action act = () => processo.AddAndamento(andamento, systemNow);

        // Assert
        act.Should().Throw<InvalidAndamentoDateException>().WithMessage("*futuro*");
    }

    [Fact]
    public void AddAndamento_WhenDateIsBeforeCreation_ShouldThrowException()
    {
        // Arrange
        var creationDate = DateTimeOffset.UtcNow;
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", creationDate, partes);
        var andamento = new Andamento(creationDate.AddDays(-1), "Antes da criação");

        // Act
        Action act = () => processo.AddAndamento(andamento, DateTimeOffset.UtcNow);

        // Assert
        act.Should().Throw<InvalidAndamentoDateException>().WithMessage("*anterior*");
    }

    [Fact]
    public void Delete_WhenArquivado_ShouldSoftDelete()
    {
        // Arrange
        var partes = new List<Parte> { CreateParte(TipoPolo.Ativo), CreateParte(TipoPolo.Passivo) };
        var processo = new Processo("0001234-56.2023.8.26.0100", TipoProcesso.Judicial, "Assunto", DateTimeOffset.UtcNow, partes);
        processo.ChangeStatus(StatusProcesso.Arquivado);

        // Act
        processo.Delete();

        // Assert
        processo.FlagDeleted.Should().BeTrue();
    }
}
