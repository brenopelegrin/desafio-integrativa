using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Domain.Enums;
using System;

namespace GerenciadorProcessos.Application.Validators;

public class AddParteDtoValidator : AbstractValidator<AddParteDto>
{
    public AddParteDtoValidator()
    {
        RuleFor(x => x.TipoPolo)
            .IsInEnum()
            .WithMessage("Polo tipo inválido. Deve ser Ativo ou Passivo.");

        RuleFor(x => x)
            .Must(x => (x.EntidadeLegalId.HasValue && x.EntidadeLegalId.Value != Guid.Empty) || x.NovaEntidadeLegal != null)
            .WithMessage("Deve ser informado um EntidadeLegalId válido ou os dados de NovaEntidadeLegal.");

        RuleFor(x => x.NovaEntidadeLegal)
            .SetValidator(new CreateEntidadeLegalDtoValidator()!)
            .When(x => x.NovaEntidadeLegal != null);
    }
}
