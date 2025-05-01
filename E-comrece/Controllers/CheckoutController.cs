using E_comerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using E_comrece.Settings;
using E_comerce.Data;
using E_comerce.Services;
using E_comrece.DTO;
using Microsoft.EntityFrameworkCore;

namespace E_comrece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;
        private readonly AppDbContext _context;

        public CheckoutController(IOptions<StripeSettings> stripeOptions, AppDbContext context)
        {
            _stripeSettings = stripeOptions.Value;
            _context = context;

            // Initialize Stripe API key from environment variables
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey ?? throw new InvalidOperationException("Stripe API key is not set.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest("Cart is empty.");

            var groupedByStore = request.Items
                .GroupBy(i => i.StoreId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var orderService = new OrderService(_context);
            var results = new List<object>();
            var orderIds = new List<int>();

            var sessionService = new SessionService();
            var allLineItems = new List<SessionLineItemOptions>(); // Combine all line items here

            foreach (var (storeId, storeItems) in groupedByStore)
            {
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
                if (store == null)
                    return BadRequest($"Store with ID {storeId} does not exist.");

                var order = await orderService.CreateOrder(storeItems, "Address", 1234567890, storeId);
                orderIds.Add(order.Id);

                var productIds = storeItems.Select(i => i.Id).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var item in storeItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.Id);
                    if (product == null) continue;

                    allLineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(product.Price * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = product.Name
                            },
                        },
                        Quantity = item.Quantity,
                    });
                }
            }

            // Create a single session with all line items
            var session = await sessionService.CreateAsync(new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = allLineItems,
                Mode = "payment",
                SuccessUrl = $"http://localhost:3001/cart?success=true",
                CancelUrl = $"http://localhost:3001/cart?canceled=true",
            });

            return Ok(new
            {
                sessionUrl = session.Url, // Return a single session URL for all items
                orderIds
            });
        }
    }
}
