using FluentValidation;
using Projekt.DTOs.ClientDtos;

namespace Projekt.Validators.ClientValidators
{
    public class CreateClientPersonValidator : AbstractValidator<AddClientPersonDto>
    {
        public CreateClientPersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Imie max 50 znakow");

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Imie max 50 znakow");

            RuleFor(x => x.PESEL)
                .NotNull()
                .NotEmpty()
                .Length(11)
                .Matches("^[0-9]+$")
                .WithMessage("Pesel 11 cyfr");

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
