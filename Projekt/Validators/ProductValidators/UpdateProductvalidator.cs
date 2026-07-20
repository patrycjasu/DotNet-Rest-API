using FluentValidation;
using Projekt.DTOs.ProductDtos;

namespace Projekt.Validators.ProductValidators
{
    public class UpdateProductvalidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductvalidator() {
            RuleFor(x => x.Description)
                .MaximumLength(300)
                .WithMessage("max 300 znakow opis");
        }
    }
}
