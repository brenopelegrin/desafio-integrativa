using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Domain.Enums;
using System;

namespace GerenciadorProcessos.Application.Validators;

public class CreateEntidadeLegalDtoValidator : AbstractValidator<CreateEntidadeLegalDto>
{
    private const string CpfRegex = @"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$";
    private const string CnpjRegex = @"^\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2}$";

    public CreateEntidadeLegalDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome não pode exceder 200 caracteres.");

        RuleFor(x => x.TipoEntidade)
            .IsInEnum()
            .WithMessage("Tipo da pessoa inválido. Deve ser PessoaFisica ou PessoaJuridica.");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("O número do documento é obrigatório.");

        RuleFor(x => x.NumeroDocumento)
            .Matches(CpfRegex).WithMessage("Formato de CPF inválido.")
            .When(x => x.TipoEntidade == TipoEntidade.PessoaFisica);

        RuleFor(x => x.NumeroDocumento)
            .Matches(CnpjRegex).WithMessage("Formato de CNPJ inválido.")
            .When(x => x.TipoEntidade == TipoEntidade.PessoaJuridica);
    }
}
