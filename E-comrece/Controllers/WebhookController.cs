using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using E_comerce.Services;
using E_comrece.DTO;

namespace E_comrece.Controllers
{
    [Route("api/stripeWebhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IConfiguration _configuration;

        public WebhookController(OrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var endpointSecret = _configuration["Stripe:WebhookSecret"];

            Stripe.Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                if (session != null && session.Metadata != null)
                {
                    var productIdsJson = session.Metadata["productIds"];
                    var storeId = int.Parse(session.Metadata["storeId"]);
                    var phone = int.Parse(session.Metadata["phone"]);

                    var addressLine1 = session.Metadata["line1"];
                    var addressLine2 = session.Metadata["line2"];
                    var city = session.Metadata["city"];
                    var state = session.Metadata["state"];
                    var postalCode = session.Metadata["postalCode"];
                    var country = session.Metadata["country"];

                    var fullAddress = $"{addressLine1}, {addressLine2}, {city}, {state}, {postalCode}, {country}";

                    var productIds = JsonConvert.DeserializeObject<List<int>>(productIdsJson);

                    // Create CartItem list with ID and Quantity (assumes you know the quantities)
                    var cartItems = productIds.Select(id => new CheckoutRequest.CartItem
                    {
                        Id = id,
                        Quantity = 1 // Adjust this based on the frontend/cart data
                    }).ToList();

                    // Pass CartItem list to OrderService
                    await _orderService.CreateOrder(cartItems, fullAddress, phone, storeId);
                }
            }

            return Ok();
        }
    }

}
