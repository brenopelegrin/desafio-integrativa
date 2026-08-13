using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Domain.Enums;
using System;

namespace GerenciadorProcessos.Application.Validators;

public class UpdateProcessoDtoValidator : AbstractValidator<UpdateProcessoDto>
{
    public UpdateProcessoDtoValidator()
    {
        RuleFor(x => x.Assunto)
            .NotEmpty().WithMessage("O assunto é obrigatório.")
            .MaximumLength(500).WithMessage("O assunto não pode exceder 500 caracteres.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status do processo inválido.");
    }
}
