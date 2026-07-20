using FluentValidation;
using Projekt.DTOs.ProductDtos;

namespace Projekt.Validators.ProductValidators
{
    public class CreateProductValidator : AbstractValidator<AddProductDto>
    {
        public CreateProductValidator() { 
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(50)
                .WithMessage("max 50 znakow imie");

            RuleFor(p => p.Description)
                .MaximumLength(300)
                .WithMessage("max 300 znakow opis");

            RuleFor(p => p.Weight)
                .NotEmpty()
                .NotNull()
                .WithMessage("trzeba podac wage");


            RuleFor(p => p.CurrentPrice)
                .NotEmpty()
                .NotNull()
                .WithMessage("trzeba pdoac cene");


            RuleFor(p => p.CategoryId)
                .NotNull()
                .WithMessage("trzeba podac kategorie");
        }
    }
}
