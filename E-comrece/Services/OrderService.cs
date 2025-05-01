using System.Linq;
using E_comerce.Data;
using E_comerce.Models;
using E_commerce.Models;
using E_comrece.DTO;
using E_comrece.Models;

namespace E_comerce.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(List<CheckoutRequest.CartItem> items, string address, int phone, int storeId)
        {
            var productIds = items.Select(i => i.Id).ToList();
            var products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();

            if (products.Count != productIds.Count)
            {
                throw new ArgumentException("One or more products do not exist.");
            }

            var order = new Order
            {
                Address = address,
                Phone = phone,
                isPaid = true,
                StoreId = storeId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.Id,
                    Quantity = item.Quantity
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            return order;
        }


    }
}
