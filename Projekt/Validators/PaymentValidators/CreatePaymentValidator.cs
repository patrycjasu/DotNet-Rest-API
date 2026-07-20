using FluentValidation;
using Projekt.DTOs.PaymentDtos;

namespace Projekt.Validators.PaymentValidators
{
    public class CreatePaymentValidator : AbstractValidator<AddPaymentDto>
    {
        public CreatePaymentValidator() {

            RuleFor(x => x.Amount)
                .NotEmpty()
                .WithMessage("Trzeba podac kwote");


            RuleFor(x => x.Method)
                .NotEmpty()
                .WithMessage("Trzeba podac metode");

        }
    }
}
