using FluentValidation;
using Projekt.DTOs.OrderDtos;

namespace Projekt.Validators.OrderValidators
{
    public class CreateOrderValidator : AbstractValidator<AddOrderDto>
    {
        public CreateOrderValidator() {

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("ClientId nie moze byc puste");

            RuleFor(x => x.ProductsOrders)
                .NotNull()
                .NotEmpty()
                .WithMessage("Lista produktow nie moze byc pusta");
        
        }
    }
}
