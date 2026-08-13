using System;
using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;

namespace GerenciadorProcessos.Application.Validators;

public class AddAndamentoDtoValidator : AbstractValidator<AddAndamentoDto>
{
    public AddAndamentoDtoValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do andamento é obrigatória.")
            .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres.");

        RuleFor(x => x.DataAndamento)
            .NotEmpty().WithMessage("A data do andamento é obrigatória.")

            .Must((dto, data) => data <= timeProvider.GetUtcNow())
            .WithMessage("A data do andamento não pode ser no futuro.");
    }
}
