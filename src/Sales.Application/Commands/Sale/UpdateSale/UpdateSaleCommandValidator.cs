using FluentValidation;

namespace Sales.Application.Commands
{
    public sealed class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Sale ID must be greater than zero.");

            RuleFor(x => x.SaleDate)
                .NotNull().WithMessage("Sale date is required.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID must be greater than zero.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch ID must be greater than zero.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one sale item is required.");

            RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemDtoValidator());
        }
    }

    public sealed class UpdateSaleItemDtoValidator : AbstractValidator<UpdateSaleItemDto>
    {
        public UpdateSaleItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to zero.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to zero.");
        }
    }
}
