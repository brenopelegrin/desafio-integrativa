using System;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Responses;

public record ParteDto(
    Guid Id,
    TipoPolo TipoPolo,
    EntidadeLegalDto EntidadeLegal
);
