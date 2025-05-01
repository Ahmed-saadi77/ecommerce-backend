using E_comerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_comrece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int storeId)
        {
            // Fetch orders along with their related data in a more optimized manner
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.StoreId == storeId)
                .OrderByDescending(o => o.Created)
                .Select(o => new
                {
                    o.Id,
                    o.Phone,
                    o.Address,
                    o.Created,
                    o.isPaid, // Include the payment status
                    totalPrice = o.OrderItems.Sum(oi => oi.Product.Price * oi.Quantity),
                    Products = o.OrderItems.Select(oi => new
                    {
                        oi.ProductId,
                        oi.Product.Name,
                        oi.Product.Price,
                        oi.Quantity
                    }).ToList(),
                    Store = new
                    {
                        StoreId = o.StoreId,
                        StoreName = o.Store.Name // Directly accessing the store's name
                    }
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
