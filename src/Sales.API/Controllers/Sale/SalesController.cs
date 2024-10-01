using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers.Requests;

namespace Sales.API.Controllers.Responses
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SalesController> _logger;

        public SalesController(IMediator mediator, ILogger<SalesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetSaleById")]
        public async Task<IActionResult> Get(long id)
        {
            using (_logger.BeginScope("GetSales Action"))
            {
                _logger.LogInformation("Starting GetSales action");

                try
                {
                    var request = new GetSaleRequest { Id = id };
                    var result = await _mediator.Send(request.ToCommand());
                    _logger.LogInformation("GetSales action completed successfully");
                    if(result.Sales.Any())
                        return Ok(SaleResponse.FromResult(result).First());

                    return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing GetSales action");
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetSaleRequest request)
        {
            using (_logger.BeginScope("GetSales Action"))
            {
                _logger.LogInformation("Starting GetSales action");

                try
                {
                    var result = await _mediator.Send(request.ToCommand());
                    _logger.LogInformation("GetSales action completed successfully");
                    return Ok(SaleResponse.FromResult(result));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing GetSales action");
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateSaleRequest request)
        {
            using (_logger.BeginScope("CreateSale Action"))
            {
                _logger.LogInformation("Starting CreateSale action");

                var response = await _mediator.Send(request.ToCommand());

                if (!response.Success)
                {
                    if (response.ValidationErrors.Any())
                        return BadRequest(new { Errors = response.ValidationErrors });

                    return BadRequest(new { Error = response.ErrorMessage });
                }

                _logger.LogInformation("CreateSale action completed successfully");
                return CreatedAtRoute("GetSaleById", new { id = response.Data.Id }, null);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateSaleRequest request)
        {
            using (_logger.BeginScope("UpdateSale Action"))
            {
                _logger.LogInformation("Starting UpdateSale action with Id: {Id}", id);

                request.Id = id;
                var response = await _mediator.Send(request.ToCommand());

                if (!response.Success)
                {
                    if (response.ValidationErrors.Any())
                        return BadRequest(new { Errors = response.ValidationErrors });

                    return BadRequest(new { Error = response.ErrorMessage });
                }

                _logger.LogInformation("UpdateSale action completed successfully for Id: {Id}", id);
                return Ok();
            }
        }
    }
}
