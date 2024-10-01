using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sales.Application.Commands;
using Sales.Application.Messaging;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Enums;

namespace Sales.Application.Tests.Commands
{
    public class CreateSaleCommandTests
    {
        private readonly CreateSaleCommandValidator _validator;
        private readonly CreateSaleCommandHandler _handler;
        private readonly IValidator<CreateSaleCommand> _mockValidator;
        private readonly ILogger<CreateSaleCommand> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly Faker<CreateSaleCommand> _commandFaker;

        public CreateSaleCommandTests()
        {
            _validator = new CreateSaleCommandValidator();
            _mockValidator = Substitute.For<IValidator<CreateSaleCommand>>();
            _logger = Substitute.For<ILogger<CreateSaleCommand>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _eventPublisher = Substitute.For<IEventPublisher>();

            _handler = new CreateSaleCommandHandler(_mockValidator, _logger, _saleRepository, _eventPublisher);

            _commandFaker = new Faker<CreateSaleCommand>()
                .RuleFor(c => c.SaleDate, f => f.Date.Past())
                .RuleFor(c => c.CustomerId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.BranchId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.Items, f => new Faker<CreateSaleItemDto>()
                    .RuleFor(i => i.ProductId, f => f.Random.Long(1, 1000))
                    .RuleFor(i => i.Quantity, f => f.Random.Int(1, 100))
                    .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100))
                    .RuleFor(i => i.Discount, f => f.Random.Decimal(0, 10))
                    .Generate(3));
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldCreateSaleAndReturnSaleInResponse()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            _saleRepository.InsertAsync(Arg.Any<Sale>())
                           .Returns(Task.FromResult(1L));

            _eventPublisher.PublishAsync(Arg.Any<SaleCreatedEvent>())
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.SaleStatus.Should().Be(ESaleStatus.Active);
            result.Data.CustomerId.Should().Be(command.CustomerId);
            result.Data.BranchId.Should().Be(command.BranchId);
            result.Data.Items.Count.Should().Be(command.Items.Count);

            await _saleRepository.Received(1).InsertAsync(Arg.Any<Sale>());
            await _eventPublisher.Received(1).PublishAsync(Arg.Any<SaleCreatedEvent>());
        }

        [Fact]
        public async Task Handle_WhenCommandIsInvalid_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationFailure = new FluentValidation.Results.ValidationFailure("CustomerId", "Customer ID must be greater than zero.");
            var validationFailures = new FluentValidation.Results.ValidationResult(new[] { validationFailure });

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationFailures));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ValidationErrors.Should().Contain("Customer ID must be greater than zero.");
            result.ErrorMessage.Should().BeNull();

            await _saleRepository.DidNotReceive().InsertAsync(Arg.Any<Sale>());
            await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<SaleCreatedEvent>());
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _saleRepository.InsertAsync(Arg.Any<Sale>())
                           .Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Database error");
            result.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenSaleDateIsEmpty_ShouldReturnError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleDate = default(DateTime),
                CustomerId = 1,
                BranchId = 1,
                Items = new List<CreateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SaleDate)
                  .WithErrorMessage("Sale date is required.");
        }

        [Fact]
        public void Validate_WhenCustomerIdIsZero_ShouldReturnError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                CustomerId = 0,
                BranchId = 1,
                Items = new List<CreateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId)
                  .WithErrorMessage("Customer ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenBranchIdIsZero_ShouldReturnError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                CustomerId = 1,
                BranchId = 0,
                Items = new List<CreateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BranchId)
                  .WithErrorMessage("Branch ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenItemsAreEmpty_ShouldReturnError()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                CustomerId = 1,
                BranchId = 1,
                Items = new List<CreateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Items);
        }

        [Fact]
        public void Validate_WhenProductIdIsZero_ShouldReturnError()
        {
            // Arrange
            var item = new CreateSaleItemDto
            {
                ProductId = 0,
                Quantity = 1,
                UnitPrice = 10,
                Discount = 0
            };

            // Act
            var validator = new CreateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                  .WithErrorMessage("Product ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenQuantityIsZero_ShouldReturnError()
        {
            // Arrange
            var item = new CreateSaleItemDto
            {
                ProductId = 1,
                Quantity = 0,
                UnitPrice = 10,
                Discount = 0
            };

            // Act
            var validator = new CreateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenUnitPriceIsNegative_ShouldReturnError()
        {
            // Arrange
            var item = new CreateSaleItemDto
            {
                ProductId = 1,
                Quantity = 1,
                UnitPrice = -10,
                Discount = 0
            };

            // Act
            var validator = new CreateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                  .WithErrorMessage("Unit price must be greater than or equal to zero.");
        }

        [Fact]
        public void Validate_WhenDiscountIsNegative_ShouldReturnError()
        {
            // Arrange
            var item = new CreateSaleItemDto
            {
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 10,
                Discount = -1
            };

            // Act
            var validator = new CreateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Discount)
                  .WithErrorMessage("Discount must be greater than or equal to zero.");
        }
    }
}
