using FluentValidation;
using Projekt.DTOs.ClientDtos;

namespace Projekt.Validators.ClientValidators
{
    public class CreateClientCompanyValidator : AbstractValidator<AddClientCompanyDto>
    {
        public CreateClientCompanyValidator()
        { 
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(50)
                .WithMessage("Nazwa max 50 znakow");

            RuleFor(x => x.NIP)
                .NotNull()
                .NotEmpty()
                .Length(10)
                .Matches("^[0-9]+$")
                .WithMessage("NIP 10 cyfr");

            RuleFor(x => x.Address)
                .NotNull()
                .NotEmpty()
                .MaximumLength(300)
                .WithMessage("Adres max 300 znakow");

            RuleFor(x => x.Phone)
                .NotNull()
                .NotEmpty()
                .Length(9)
                .Matches("^[0-9]+$")
                .WithMessage("Telefon 9 cyfr");

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50)
                .WithMessage("Musi byc poprawny i 50 znakow max");
        }
    }
}
