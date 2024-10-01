using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers.Requests;

namespace Sales.API.Controllers.Responses
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SaleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSales([FromQuery] GetSaleRequest request)
        {
            try
            {
                var result = await _mediator.Send(request.ToCommand());
                return Ok(SaleResponse.FromResult(result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
        {
            var response = await _mediator.Send(request.ToCommand());

            if (!response.Success)
            {
                if (response.ValidationErrors.Any())
                    return BadRequest(new { Errors = response.ValidationErrors });

                return BadRequest(new { Error = response.ErrorMessage });
            }

            return CreatedAtAction(nameof(CreateSale),
                                   new { id = response.Data.Id },
                                   SaleResponse.FromResult(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(long id, [FromBody] UpdateSaleRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request.ToCommand());

            if (!response.Success)
            {
                if (response.ValidationErrors.Any())
                    return BadRequest(new { Errors = response.ValidationErrors });

                return BadRequest(new { Error = response.ErrorMessage });
            }

            return Ok(SaleResponse.FromResult(response));
        }
    }
}
