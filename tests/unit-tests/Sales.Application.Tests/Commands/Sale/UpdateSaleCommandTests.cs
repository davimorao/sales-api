using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sales.Application.Commands;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Application.Messaging;
using Xunit;
using Sales.Domain.Entities;
using Sales.Infra.Persistence.Database.Repositories;

namespace Sales.Application.Tests.Commands
{
    public class UpdateSaleCommandTests
    {
        private readonly UpdateSaleCommandValidator _validator;
        private readonly UpdateSaleCommandHandler _handler;
        private readonly IValidator<UpdateSaleCommand> _mockValidator;
        private readonly ILogger<UpdateSaleCommand> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly Faker<UpdateSaleCommand> _commandFaker;

        public UpdateSaleCommandTests()
        {
            _validator = new UpdateSaleCommandValidator();
            _mockValidator = Substitute.For<IValidator<UpdateSaleCommand>>();
            _logger = Substitute.For<ILogger<UpdateSaleCommand>>();
            _saleRepository = Substitute.For<ISaleRepository>();
            _eventPublisher = Substitute.For<IEventPublisher>();

            _handler = new UpdateSaleCommandHandler(_mockValidator, _logger, _saleRepository, _eventPublisher);

            _commandFaker = new Faker<UpdateSaleCommand>()
                .RuleFor(c => c.Id, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.SaleDate, f => f.Date.Past())
                .RuleFor(c => c.CustomerId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.BranchId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.SaleStatus, f => f.PickRandom<ESaleStatus>())
                .RuleFor(c => c.Items, f => new Faker<UpdateSaleItemDto>()
                    .RuleFor(i => i.ProductId, f => f.Random.Long(1, 1000))
                    .RuleFor(i => i.Quantity, f => f.Random.Int(1, 100))
                    .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100))
                    .RuleFor(i => i.Discount, f => f.Random.Decimal(0, 10))
                    .Generate(3));
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldUpdateSaleAndReturnSaleInResponse()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            var sale = new Sale { Id = command.Id };
            _saleRepository.GetByIdAsync(command.Id)
                           .Returns(sale);

            _saleRepository.UpdateAsync(Arg.Any<Sale>())
                           .Returns(true);

            _eventPublisher.PublishAsync(Arg.Any<SaleUpdatedEvent>())
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.SaleStatus.Should().Be(command.SaleStatus);
            result.Data.CustomerId.Should().Be(command.CustomerId);
            result.Data.BranchId.Should().Be(command.BranchId);
            result.Data.Items.Count.Should().Be(command.Items.Count);

            await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>());
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

            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
            await _eventPublisher.DidNotReceive().PublishAsync(Arg.Any<SaleUpdatedEvent>());
        }

        [Fact]
        public async Task Handle_WhenSaleNotFound_ShouldReturnError()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _saleRepository.GetByIdAsync(command.Id)
                           .Returns(null as Sale);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Sale not found.");
        }

        [Fact]
        public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _saleRepository.GetByIdAsync(command.Id)
                           .Returns(new Sale { Id = command.Id });

            _saleRepository.UpdateAsync(Arg.Any<Sale>())
                           .Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Database error");
            result.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenSaleIdIsZero_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateSaleCommand
            {
                Id = 0,
                SaleDate = DateTime.Now,
                CustomerId = 1,
                BranchId = 1,
                Items = new List<UpdateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Sale ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenSaleDateIsNull_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateSaleCommand
            {
                Id = 1,
                SaleDate = null,
                CustomerId = 1,
                BranchId = 1,
                Items = new List<UpdateSaleItemDto>()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SaleDate)
                  .WithErrorMessage("Sale date is required.");
        }

        [Fact]
        public void Validate_WhenItemsAreEmpty_ShouldReturnError()
        {
            // Arrange
            var command = new UpdateSaleCommand
            {
                Id = 1,
                SaleDate = DateTime.Now,
                CustomerId = 1,
                BranchId = 1,
                Items = new List<UpdateSaleItemDto>()
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
            var item = new UpdateSaleItemDto
            {
                ProductId = 0,
                Quantity = 1,
                UnitPrice = 10,
                Discount = 0
            };

            // Act
            var validator = new UpdateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                  .WithErrorMessage("Product ID must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenQuantityIsZero_ShouldReturnError()
        {
            // Arrange
            var item = new UpdateSaleItemDto
            {
                ProductId = 1,
                Quantity = 0,
                UnitPrice = 10,
                Discount = 0
            };

            // Act
            var validator = new UpdateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage("Quantity must be greater than zero.");
        }

        [Fact]
        public void Validate_WhenUnitPriceIsNegative_ShouldReturnError()
        {
            // Arrange
            var item = new UpdateSaleItemDto
            {
                ProductId = 1,
                Quantity = 1,
                UnitPrice = -10,
                Discount = 0
            };

            // Act
            var validator = new UpdateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                  .WithErrorMessage("Unit price must be greater than or equal to zero.");
        }

        [Fact]
        public void Validate_WhenDiscountIsNegative_ShouldReturnError()
        {
            // Arrange
            var item = new UpdateSaleItemDto
            {
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 10,
                Discount = -1
            };

            // Act
            var validator = new UpdateSaleItemDtoValidator();
            var result = validator.TestValidate(item);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Discount)
                  .WithErrorMessage("Discount must be greater than or equal to zero.");
        }

        [Fact]
        public async Task Handle_WhenUpdateFails_ShouldReturnErrorMessage()
        {
            // Arrange
            var command = _commandFaker.Generate();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            _saleRepository.GetByIdAsync(command.Id)
                           .Returns(new Sale { Id = command.Id });

            _saleRepository.UpdateAsync(Arg.Any<Sale>())
                           .Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error while updating Sale.");
            result.Data.Should().BeNull();
            result.ValidationErrors.Should().BeEmpty();

            await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>());
        }

        [Fact]
        public async Task Handle_WhenSaleIsCancelled_ShouldPublishSaleCancelledEvent()
        {
            // Arrange
            var command = _commandFaker.Generate();
            command.SaleStatus = ESaleStatus.Cancelled;

            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator.ValidateAsync(command, CancellationToken.None)
                          .Returns(Task.FromResult(validationResult));

            var sale = new Sale { Id = command.Id, CustomerId = command.CustomerId!.Value, BranchId = command.BranchId!.Value, SaleStatus = ESaleStatus.Active };

            _saleRepository.GetByIdAsync(command.Id)
                           .Returns(sale);

            _saleRepository.UpdateAsync(Arg.Any<Sale>())
                           .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            await _eventPublisher.Received(1).PublishAsync(Arg.Is<SaleCancelledEvent>(e =>
                e.Id == sale.Id &&
                e.CustomerId == sale.CustomerId &&
                e.BranchId == sale.BranchId &&
                e.SaleStatus == ESaleStatus.Cancelled
            ));
        }
    }
}
