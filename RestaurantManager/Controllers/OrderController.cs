using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers
{
    public class OrderController : CustomerControllerBase
    {
        public OrderController(AppDbContext db) : base(db)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int id)
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return RedirectToAction("Index", "Session");

            var menuItem = await _db.MenuItems
               .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (menuItem == null)
                return RedirectToAction("Index", "Menu");

            var draft = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SessionId == session.Id && o.IsDraft);

            if (draft == null)
            {
                draft = new Order
                {
                    SessionId = session.Id,
                    IsDraft = true,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Orders.Add(draft);
                await _db.SaveChangesAsync();
            }

            var existing = draft.Items.FirstOrDefault(i => i.MenuItemId == id);

            if (existing != null)
                existing.Quantity++;

            else
            {
                draft.Items.Add(new OrderItem
                {
                    MenuItemId = id,
                    Quantity = 1,
                    UnitPrice = 0,
                    Status = OrderStatus.Draft
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Menu");
        }
    }
}
