using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sales.Application.Commands;
using Sales.Domain.Entities;
using Sales.Domain.Entities.Products;

namespace Sales.Application.Tests.Commands
{
    public class UpdateProductCommandTests
    {
        private readonly UpdateProductCommandValidator _validator;
        private readonly UpdateProductCommandHandler _handler;
        private readonly IValidator<UpdateProductCommand> _mockValidator;
        private readonly ILogger<UpdateProductCommand> _logger;
        private readonly IProductRepository _productRepository;
        private readonly Faker<UpdateProductCommand> _commandFaker;

        public UpdateProductCommandTests()
        {
            _validator = new UpdateProductCommandValidator();
            _mockValidator = Substitute.For<IValidator<UpdateProductCommand>>();
            _logger = Substitute.For<ILogger<UpdateProductCommand>>();
            _productRepository = Substitute.For<IProductRepository>();

            _handler = new UpdateProductCommandHandler(_mockValidator, _logger, _productRepository);

            _commandFaker = new Faker<UpdateProductCommand>()
                .RuleFor(c => c.Id, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.ProductName, f => f.Commerce.ProductName())
                .RuleFor(c => c.UnitPrice, f => f.Random.Decimal(1, 1000));
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldUpdateProductAndReturnProductInResponse()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            _productRepository.UpdateAsync(Arg.Any<Product>())
                              .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(command.Id);
            result.Data.ProductName.Should().Be(command.ProductName);
            result.Data.UnitPrice.Should().Be(command.UnitPrice);
            result.ValidationErrors.Should().BeEmpty();

            await _productRepository.Received(1).UpdateAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task Handle_WhenCommandIsInvalid_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationFailure = new FluentValidation.Results.ValidationFailure("ProductName", "Product name is required.");
            var validationFailures = new FluentValidation.Results.ValidationResult(new[] { validationFailure });

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationFailures));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ValidationErrors.Should().Contain("Product name is required.");
            result.ErrorMessage.Should().BeNull();

            await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _productRepository.UpdateAsync(Arg.Any<Product>())
                              .Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Database error");
            result.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenProductIdIsZero_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 0,
                ProductName = "Test Product",
                UnitPrice = 10
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Product ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenProductNameIsEmpty_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = string.Empty,
                UnitPrice = 10
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                  .WithErrorMessage("Product name is required.");
        }

        [Fact]
        public void Validate_WhenProductNameExceedsMaxLength_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = new string('A', 101),
                UnitPrice = 10
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                  .WithErrorMessage("Product name cannot exceed 100 characters.");
        }

        [Fact]
        public void Validate_WhenProductNameIsValid_ShouldNotReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = "Valid Product",
                UnitPrice = 10
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        }

        [Fact]
        public void Validate_WhenUnitPriceIsZero_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = "Valid Product",
                UnitPrice = 0
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                  .WithErrorMessage("Unit price must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenUnitPriceIsNegative_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = "Valid Product",
                UnitPrice = -10
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                  .WithErrorMessage("Unit price must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenUnitPriceIsPositive_ShouldNotReturnError()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = 1,
                ProductName = "Valid Product",
                UnitPrice = 100
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
        }
    }
}
