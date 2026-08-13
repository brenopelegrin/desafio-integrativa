using System;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

public record AddParteDto(
    TipoPolo TipoPolo,
    Guid? EntidadeLegalId,
    CreateEntidadeLegalDto? NovaEntidadeLegal
);
