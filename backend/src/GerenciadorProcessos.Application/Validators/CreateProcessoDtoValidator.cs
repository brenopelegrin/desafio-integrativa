using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Domain.Enums;
using System;
using System.Linq;
using GerenciadorProcessos.Domain.Constants;

namespace GerenciadorProcessos.Application.Validators;

public class CreateProcessoDtoValidator : AbstractValidator<CreateProcessoDto>
{
    public CreateProcessoDtoValidator()
    {
        RuleFor(x => x.NumeroProcesso)
            .NotEmpty().WithMessage("O número do processo é obrigatório.");

        RuleFor(x => x.NumeroProcesso)
            .Matches(DomainConstants.CnjRegex).WithMessage("O número do processo judicial deve seguir o padrão CNJ (NNNNNNN-DD.AAAA.J.TR.OOOO).")
            .When(x => x.TipoProcesso == TipoProcesso.Judicial);

        RuleFor(x => x.TipoProcesso)
            .IsInEnum()
            .WithMessage("Tipo de processo inválido. O valor deve ser um tipo de processo válido (ex: Judicial ou Administrativo).");

        RuleFor(x => x.Assunto)
            .NotEmpty().WithMessage("O assunto é obrigatório.")
            .MaximumLength(500).WithMessage("O assunto não pode exceder 500 caracteres.");

        RuleFor(x => x.Partes)
            .NotEmpty().WithMessage("Um processo deve ter partes.")
            .Must(p => p != null && p.Count() >= 2).WithMessage("Um processo deve ter ao menos 2 partes (1 ativo e 1 passivo).");

        RuleForEach(x => x.Partes)
            .SetValidator(new AddParteProcessoDtoValidator());
    }
}
