using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Discount>), 200)]
        public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscounts()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }

        [HttpGet("by-segment/{segment}")]
        [ProducesResponseType(typeof(IEnumerable<Discount>), 200)]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscountsBySegment(CustomerSegment segment)
        {
            var discounts = await _discountService.GetDiscountsBySegmentAsync(segment);
            return Ok(discounts);
        }
    }
}
