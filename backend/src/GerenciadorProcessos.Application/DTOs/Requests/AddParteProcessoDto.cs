using System;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

/// <summary>
/// DTO para adicionar uma Parte a um Processo.
///
/// Aceita informar o ID de uma EntidadeLegal já existente, ou criar uma nova.
///
/// **Regras de validação:**
/// - É obrigatório informar um **entidadeLegalId** válido (caso a entidade já exista no sistema) OU os dados completos em **novaEntidadeLegal** para cadastrar uma nova entidade.
/// </summary>
public record AddParteProcessoDto(
    TipoPolo TipoPolo,
    Guid? EntidadeLegalId,
    CreateEntidadeLegalDto? NovaEntidadeLegal
);
