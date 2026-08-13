using System.Collections.Generic;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

/// <summary>
/// DTO para criar um novo Processo.
///
/// **Regras de validação:**
/// - Quando o **TipoProcesso** for **Judicial**, o campo **numeroProcesso** deverá seguir o padrão CNJ (NNNNNNN-DD.AAAA.J.TR.OOOO).
/// - Para processos Administrativos, qualquer String é aceita como número do processo.
/// </summary>
public record CreateProcessoDto(
    string NumeroProcesso,
    TipoProcesso TipoProcesso,
    string Assunto,
    IEnumerable<AddParteProcessoDto> Partes
);
