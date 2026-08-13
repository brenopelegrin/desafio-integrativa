using FluentValidation;
using GerenciadorProcessos.Application.DTOs.Requests;

namespace GerenciadorProcessos.Application.Validators;

public class UpdateEntidadeLegalDtoValidator : AbstractValidator<UpdateEntidadeLegalDto>
{
    public UpdateEntidadeLegalDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome não pode exceder 200 caracteres.");
    }
}
