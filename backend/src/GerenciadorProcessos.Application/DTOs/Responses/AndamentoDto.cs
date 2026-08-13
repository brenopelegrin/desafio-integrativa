using System;

namespace GerenciadorProcessos.Application.DTOs.Responses;

public record AndamentoDto(Guid Id, DateTimeOffset Data, string Descricao);
