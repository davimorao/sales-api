using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers.Requests;
using Sales.API.Controllers.Sale.Requests;
using Sales.Application.Commands;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> Get(long id)
        {
            using (_logger.BeginScope("GetProducts Action"))
            {
                _logger.LogInformation("Starting GetProducts action");

                try
                {
                    var request = new GetProductRequest { Id = id };
                    var result = await _mediator.Send(request.ToCommand());
                    _logger.LogInformation("GetProducts action completed successfully");
                    if (result.Products.Any())
                        return Ok(ProductResponse.FromResult(result).First());

                    return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing GetProducts action");
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetProductRequest request)
        {
            using (_logger.BeginScope("GetProducts Action"))
            {
                _logger.LogInformation("Starting GetProducts action");

                try
                {
                    var result = await _mediator.Send(request.ToCommand());
                    _logger.LogInformation("GetProducts action completed successfully");
                    return Ok(ProductResponse.FromResult(result));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing GetProducts action");
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
        {
            using (_logger.BeginScope("CreateProduct Action"))
            {
                _logger.LogInformation("Starting CreateProduct action");

                var response = await _mediator.Send(request.ToCommand());

                if (!response.Success)
                {
                    if (response.ValidationErrors.Any())
                        return BadRequest(new { Errors = response.ValidationErrors });

                    return BadRequest(new { Error = response.ErrorMessage });
                }

                _logger.LogInformation("CreateProduct action completed successfully");
                return CreatedAtRoute("GetProductById", new { id = response.Data.Id }, null);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateProductRequest request)
        {
            using (_logger.BeginScope("UpdateProduct Action"))
            {
                _logger.LogInformation("Starting UpdateProduct action with Id: {Id}", id);

                request.Id = id;
                var response = await _mediator.Send(request.ToCommand());

                if (!response.Success)
                {
                    if (response.ValidationErrors.Any())
                        return BadRequest(new { Errors = response.ValidationErrors });

                    return BadRequest(new { Error = response.ErrorMessage });
                }

                _logger.LogInformation("UpdateProduct action completed successfully for Id: {Id}", id);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (_logger.BeginScope("DeleteProduct Action"))
            {
                _logger.LogInformation("Starting DeleteProduct action for Id: {Id}", id);

                var response = await _mediator.Send(new DeleteProductCommand(id));

                if (!response.Success)
                {
                    if (response.ValidationErrors.Any())
                        return BadRequest(new { Errors = response.ValidationErrors });

                    return BadRequest(new { Error = response.ErrorMessage });
                }

                _logger.LogInformation("DeleteProduct action completed successfully for Id: {Id}", id);
                return NoContent();
            }
        }
    }
}
