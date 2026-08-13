using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace GerenciadorProcessos.Tests.Integration;

public class ProcessosControllerTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProcessosControllerTests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        var dbName = "IntegrationTestDb_" + Guid.NewGuid();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
    }

    public void Dispose() => _factory.Dispose();

    private CreateProcessoDto GenerateValidCreateProcessoDto(string numeroProcesso)
    {
        var partes = new List<AddParteProcessoDto>
        {
            new AddParteProcessoDto(TipoPolo.Ativo, null, new CreateEntidadeLegalDto("Autor", TipoEntidade.PessoaFisica, "11122233396")),
            new AddParteProcessoDto(TipoPolo.Passivo, null, new CreateEntidadeLegalDto("Reu", TipoEntidade.PessoaFisica, "44455566619"))
        };

        return new CreateProcessoDto(numeroProcesso, TipoProcesso.Judicial, "Ação Trabalhista", partes);
    }

    [Fact]
    public async Task CreateProcesso_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = GenerateValidCreateProcessoDto("0001234-56.2023.8.26.0100");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/processos", request, _jsonOptions);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorStr = await response.Content.ReadAsStringAsync();
            throw new Exception("Validation Error: " + errorStr);
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var processo = await response.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);
        processo.Should().NotBeNull();
        processo!.NumeroProcesso.Should().Be("0001234-56.2023.8.26.0100");
        processo.Assunto.Should().Be("Ação Trabalhista");
        processo.Status.Should().Be(StatusProcesso.Ativo);
        processo.Partes.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateProcesso_WithInvalidCNJ_ShouldReturnBadRequest_DueToFluentValidation()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = GenerateValidCreateProcessoDto("123");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/processos", request, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WhenProcessoDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/v1/processos/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddAndamento_ToArquivadoProcesso_ShouldReturnUnprocessableEntity_DueToDomainException()
    {
        // Arrange
        var client = _factory.CreateClient();

        var createRequest = GenerateValidCreateProcessoDto("0009999-99.2023.8.26.0100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/processos", createRequest, _jsonOptions);
        var processo = await createResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        var updateRequest = new UpdateProcessoDto("Ação Teste", StatusProcesso.Arquivado);
        await client.PatchAsJsonAsync($"/api/v1/processos/{processo!.Id}", updateRequest);

        var andamentoRequest = new AddAndamentoDto(DateTimeOffset.UtcNow, "Tentativa Invalida");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/processos/{processo.Id}/andamentos", andamentoRequest, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("arquivado");
    }

    [Fact]
    public async Task AddAndamento_WithFutureDate_ShouldReturnBadRequest_DueToFluentValidation()
    {
        // Arrange
        var client = _factory.CreateClient();

        var createRequest = GenerateValidCreateProcessoDto("0008888-88.2023.8.26.0100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/processos", createRequest, _jsonOptions);
        var processo = await createResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        var futureDate = DateTimeOffset.UtcNow.AddDays(5);
        var andamentoRequest = new AddAndamentoDto(futureDate, "Tentativa Invalida");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/processos/{processo!.Id}/andamentos", andamentoRequest, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddParte_WhenDuplicate_ShouldReturnUnprocessableEntity_DueToDomainException()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = GenerateValidCreateProcessoDto("0007777-77.2023.8.26.0100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/processos", createRequest, _jsonOptions);
        var processo = await createResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        // We try to add a part using the exact same EntidadeLegal ID as the first party created
        var firstParte = processo!.Partes.First();
        var parteRequest = new AddParteDto(TipoPolo.Passivo, firstParte.EntidadeLegal.Id, null);

        // Act
        var duplicateResponse = await client.PostAsJsonAsync($"/api/v1/processos/{processo.Id}/partes", parteRequest, _jsonOptions);

        // Assert
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var error = await duplicateResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var errorMessage = error.GetProperty("error").GetString();
        errorMessage.Should().Contain("já é uma parte ativa");
    }

    [Fact]
    public async Task RemoveParte_WhenExists_ShouldReturnOk_AndRemoveParte()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = GenerateValidCreateProcessoDto("0006666-66.2023.8.26.0100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/processos", createRequest, _jsonOptions);
        createResponse.EnsureSuccessStatusCode();
        var processo = await createResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        var parteRequest = new AddParteDto(TipoPolo.Passivo, null, new CreateEntidadeLegalDto("Para Remover", TipoEntidade.PessoaFisica, "77788899941"));
        var addParteResponse = await client.PostAsJsonAsync($"/api/v1/processos/{processo!.Id}/partes", parteRequest, _jsonOptions);
        addParteResponse.EnsureSuccessStatusCode();
        var processoUpdated = await addParteResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        // Find the newly added party
        var parteToRemove = processoUpdated!.Partes.First(p => p.EntidadeLegal.Nome == "Para Remover");

        // Act
        var removeResponse = await client.DeleteAsync($"/api/v1/processos/{processo.Id}/partes/{parteToRemove.Id}");

        // Assert
        removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var processoFinal = await removeResponse.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        // We originally had 2 partes, added 1 (total 3), and deleted 1 (back to 2)
        processoFinal!.Partes.Should().HaveCount(2);
        processoFinal.Partes.Should().NotContain(p => p.Id == parteToRemove.Id);
    }

    [Fact]
    public async Task GetAll_WithFilters_ShouldReturnFilteredProcessos()
    {
        // Arrange
        var client = _factory.CreateClient();

        // 1. Create Processo 1 (Ativo, with specific CPFs)
        var partes1 = new List<AddParteProcessoDto>
        {
            new AddParteProcessoDto(TipoPolo.Ativo, null, new CreateEntidadeLegalDto("Autor1", TipoEntidade.PessoaFisica, "53967006379")),
            new AddParteProcessoDto(TipoPolo.Passivo, null, new CreateEntidadeLegalDto("Reu1", TipoEntidade.PessoaFisica, "61702437914"))
        };
        var p1Request = new CreateProcessoDto("0001000-01.2023.8.26.0100", TipoProcesso.Judicial, "Ação", partes1);
        var response1 = await client.PostAsJsonAsync("/api/v1/processos", p1Request, _jsonOptions);
        if (!response1.IsSuccessStatusCode) { var err = await response1.Content.ReadAsStringAsync(); throw new Exception("P1 Error: " + err); }
        var p1 = await response1.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);

        // 2. Create Processo 2 (Arquivado, with different CPFs)
        var partes2 = new List<AddParteProcessoDto>
        {
            new AddParteProcessoDto(TipoPolo.Ativo, null, new CreateEntidadeLegalDto("Autor2", TipoEntidade.PessoaFisica, "52235509940")),
            new AddParteProcessoDto(TipoPolo.Passivo, null, new CreateEntidadeLegalDto("Reu2", TipoEntidade.PessoaFisica, "22347190466"))
        };
        var p2Request = new CreateProcessoDto("0002000-02.2023.8.26.0100", TipoProcesso.Judicial, "Ação 2", partes2);
        var response2 = await client.PostAsJsonAsync("/api/v1/processos", p2Request, _jsonOptions);
        response2.EnsureSuccessStatusCode();
        var p2 = await response2.Content.ReadFromJsonAsync<ProcessoDto>(_jsonOptions);
        
        var updateRequest = new UpdateProcessoDto("Assunto Atualizado", StatusProcesso.Arquivado);
        await client.PatchAsJsonAsync($"/api/v1/processos/{p2!.Id}", updateRequest);

        // Act (Filter 1)
        // --- Test Filter 1: StatusProcesso = Arquivado ---
        var filterStatusResponse = await client.GetAsync("/api/v1/processos?statusProcesso=Arquivado");
        
        // Assert (Filter 1)
        filterStatusResponse.EnsureSuccessStatusCode();
        var filterStatusResult = await filterStatusResponse.Content.ReadFromJsonAsync<PaginatedListDto<ProcessoDto>>(_jsonOptions);
        
        filterStatusResult!.Items.Should().HaveCount(1);
        filterStatusResult.Items.First().NumeroProcesso.Should().Be("0002000-02.2023.8.26.0100");

        // Act (Filter 2)
        // --- Test Filter 2: NumeroDocumentoParte = 539.670.063-79 ---
        // Notice we test that stripping punctuation works
        var filterDocResponse = await client.GetAsync("/api/v1/processos?numeroDocumentoParte=539.670.063-79");
        
        // Assert (Filter 2)
        filterDocResponse.EnsureSuccessStatusCode();
        var filterDocResult = await filterDocResponse.Content.ReadFromJsonAsync<PaginatedListDto<ProcessoDto>>(_jsonOptions);
        
        filterDocResult!.Items.Should().HaveCount(1);
        filterDocResult.Items.First().NumeroProcesso.Should().Be("0001000-01.2023.8.26.0100");
    }
}
