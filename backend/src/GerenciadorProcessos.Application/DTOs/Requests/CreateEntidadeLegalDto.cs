using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

/// <summary>
/// DTO para criar uma nova Entidade Legal.
///
/// **Regras de validação:**
/// - Quando o **TipoEntidade** for **PessoaFisica**, o campo **numeroDocumento** deverá ser um CPF válido de 11 dígitos.
/// - Quando o **TipoEntidade** for **PessoaJuridica**, o campo **numeroDocumento** deverá ser um CNPJ válido de 14 dígitos.
/// </summary>
public record CreateEntidadeLegalDto(
    string Nome,
    TipoEntidade TipoEntidade,
    string NumeroDocumento
);
