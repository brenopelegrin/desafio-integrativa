using System;

namespace GerenciadorProcessos.Application.DTOs.Requests;

/// <summary>
/// DTO para adicionar um novo Andamento a um Processo.
///
/// **Regras de validação:**
/// - A data do andamento não pode estar no futuro em relação à data atual do sistema.
/// - A data do andamento não pode ser anterior à data de criação do Processo.
/// </summary>
/// <param name="DataAndamento">Data do andamento no formato ISO 8601. Exemplo: 2026-10-25T14:30:00Z</param>
/// <param name="Descricao">Descrição do andamento. Exemplo: Audiência de conciliação realizada com sucesso.</param>
public record AddAndamentoDto(
    DateTimeOffset DataAndamento,
    string Descricao
);
