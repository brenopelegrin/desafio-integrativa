using System;
using System.Collections.Generic;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Responses;

public record ProcessoDto(
    Guid Id,
    string NumeroProcesso,
    TipoProcesso TipoProcesso,
    string Assunto,
    DateTimeOffset DataCriacao,
    StatusProcesso Status,
    IEnumerable<ParteDto> Partes,
    IEnumerable<AndamentoDto> Andamentos
);
