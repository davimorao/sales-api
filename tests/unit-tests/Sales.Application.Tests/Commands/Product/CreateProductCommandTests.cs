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
    public class CreateProductCommandTests
    {
        private readonly CreateProductCommandValidator _validator;
        private readonly CreateProductCommandHandler _handler;
        private readonly IValidator<CreateProductCommand> _mockValidator;
        private readonly ILogger<CreateProductCommand> _logger;
        private readonly IProductRepository _productRepository;
        private readonly Faker<CreateProductCommand> _commandFaker;

        public CreateProductCommandTests()
        {
            _validator = new CreateProductCommandValidator();
            _mockValidator = Substitute.For<IValidator<CreateProductCommand>>();
            _logger = Substitute.For<ILogger<CreateProductCommand>>();
            _productRepository = Substitute.For<IProductRepository>();

            _handler = new CreateProductCommandHandler(_mockValidator, _logger, _productRepository);

            _commandFaker = new Faker<CreateProductCommand>()
                .RuleFor(c => c.ProductName, f => f.Commerce.ProductName())
                .RuleFor(c => c.UnitPrice, f => f.Random.Decimal(1, 1000));
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldInsertProductAndReturnProductInResponse()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            var createdProductId = 1L;
            _productRepository.InsertAsync(Arg.Any<Product>())
                              .Returns(createdProductId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ProductName.Should().Be(command.ProductName);
            result.Data.UnitPrice.Should().Be(command.UnitPrice);
            result.ValidationErrors.Should().BeEmpty();

            await _productRepository.Received(1).InsertAsync(Arg.Any<Product>());
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

            await _productRepository.DidNotReceive().InsertAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _productRepository.InsertAsync(Arg.Any<Product>())
                              .Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Database error");
            result.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenProductNameIsEmpty_ShouldReturnError()
        {
            // Arrange
            var command = new CreateProductCommand
            {
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
            var command = new CreateProductCommand
            {
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
            var command = new CreateProductCommand
            {
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
            var command = new CreateProductCommand
            {
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
            var command = new CreateProductCommand
            {
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
            var command = new CreateProductCommand
            {
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
