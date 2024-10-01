using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sales.Application.Commands;
using Sales.Domain.Repositories;
using Xunit;

namespace Sales.Application.Tests.Commands
{
    public class DeleteProductCommandTests
    {
        private readonly DeleteProductCommandValidator _validator;
        private readonly DeleteProductCommandHandler _handler;
        private readonly IValidator<DeleteProductCommand> _mockValidator;
        private readonly ILogger<DeleteProductCommand> _logger;
        private readonly IProductRepository _productRepository;
        private readonly Faker<DeleteProductCommand> _commandFaker;

        public DeleteProductCommandTests()
        {
            _validator = new DeleteProductCommandValidator();
            _mockValidator = Substitute.For<IValidator<DeleteProductCommand>>();
            _logger = Substitute.For<ILogger<DeleteProductCommand>>();
            _productRepository = Substitute.For<IProductRepository>();

            _handler = new DeleteProductCommandHandler(_mockValidator, _logger, _productRepository);

            _commandFaker = new Faker<DeleteProductCommand>()
                .CustomInstantiator(f => new DeleteProductCommand(f.Random.Long(1, 1000)));
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldDeleteProductAndReturnTrue()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            _productRepository.DeleteByIdAsync(command.Id)
                              .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.ValidationErrors.Should().BeEmpty();

            await _productRepository.Received(1).DeleteByIdAsync(command.Id);
        }

        [Fact]
        public async Task Handle_WhenCommandIsInvalid_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationFailure = new FluentValidation.Results.ValidationFailure("Id", "Product ID must be greater than zero.");
            var validationFailures = new FluentValidation.Results.ValidationResult(new[] { validationFailure });

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationFailures));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ValidationErrors.Should().Contain("Product ID must be greater than zero.");
            result.ErrorMessage.Should().BeNull();

            await _productRepository.DidNotReceive().DeleteByIdAsync(command.Id);
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _productRepository.DeleteByIdAsync(command.Id)
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
            var command = new DeleteProductCommand(0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Product ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenProductIdIsNegative_ShouldReturnError()
        {
            // Arrange
            var command = new DeleteProductCommand(-10);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Product ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenProductIdIsValid_ShouldNotReturnError()
        {
            // Arrange
            var command = new DeleteProductCommand(100);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
