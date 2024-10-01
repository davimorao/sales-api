using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers.Requests;
using Sales.API.Controllers.Sale.Requests;
using Sales.Application.Commands;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductRequest request)
        {
            try
            {
                var result = await _mediator.Send(request.ToCommand());
                return Ok(ProductResponse.FromResult(result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var response = await _mediator.Send(request.ToCommand());

            if (!response.Success)
            {
                if (response.ValidationErrors.Any())
                    return BadRequest(new { Errors = response.ValidationErrors });

                return BadRequest(new { Error = response.ErrorMessage });
            }

            return CreatedAtAction(nameof(CreateProduct),
                                   new { id = response.Data.Id },
                                   ProductResponse.FromResult(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request.ToCommand());

            if (!response.Success)
            {
                if (response.ValidationErrors.Any())
                    return BadRequest(new { Errors = response.ValidationErrors });

                return BadRequest(new { Error = response.ErrorMessage });
            }

            return Ok(ProductResponse.FromResult(response));
        }

        // TODO: Implement soft delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var response = await _mediator.Send(new DeleteProductCommand(id));

            if (!response.Success)
            {
                if (response.ValidationErrors.Any())
                    return BadRequest(new { Errors = response.ValidationErrors });

                return BadRequest(new { Error = response.ErrorMessage });
            }

            return NoContent();
        }
    }
}
