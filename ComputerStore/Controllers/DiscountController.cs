using Microsoft.AspNetCore.Mvc;
using ComputerStoreWebApi.DTOs;
using ComputerStoreWebApi.Services;
using System.Threading.Tasks;

namespace ComputerStoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly DiscountService _discountService;

        public DiscountController(DiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<decimal>> CalculateDiscount([FromBody] BasketDto basket)
        {
            var (total, error) = await _discountService.CalculateDiscountAsync(basket);
            if (error != null)
            {
                return BadRequest(new { error });
            }

            return Ok(total);
        }
    }
}
