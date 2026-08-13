using System;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Responses;

public record EntidadeLegalDto(
    Guid Id,
    string Nome,
    TipoEntidade TipoEntidade,
    string NumeroDocumento
);
