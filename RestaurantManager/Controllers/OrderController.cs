using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Hubs;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers
{
    public class OrderController : CustomerControllerBase
    {
        private readonly IHubContext<TableHub> _hub;
        public OrderController(AppDbContext db, IHubContext<TableHub> hub) : base(db)
        {
            _hub = hub;
        }

        private async Task BroadcastBasketUpdate(int sessionId, string tableNumber, List<OrderItem> items)
        {
            var quantities = items.ToDictionary(i => i.MenuItemId, i => i.Quantity);
            var basketCount = items.Sum(i => i.Quantity);

            await _hub.Clients
                .Group($"table-{tableNumber}")
                .SendAsync("BasketUpdated", new { quantities, basketCount });
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
            await BroadcastBasketUpdate(session.Id, session.TableNumber.ToString(), draft.Items.ToList());

            return Json(new { menuItemId = id, quantity = existing?.Quantity ?? 1, basketCount = draft.Items.Sum(i => i.Quantity) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return Json(new { error = "no_session" });

            var menuItem = await _db.MenuItems
               .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (menuItem == null)
                return Json(new { error = "item_not_found" });

            var draft = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SessionId == session.Id && o.IsDraft);

            if (draft == null)
                return Json(new { error = "draft_not_found" });

            var existing = draft.Items.FirstOrDefault(i => i.MenuItemId == id);

            if (existing == null || existing.Quantity <= 0)
                return Json(new { error = "item_not_in_basket" });

            existing.Quantity--;

            if (existing.Quantity == 0)
                draft.Items.Remove(existing);

            await _db.SaveChangesAsync();
            await BroadcastBasketUpdate(session.Id, session.TableNumber.ToString(), draft.Items.ToList());

            return Json(new { menuItemId = id, quantity = existing?.Quantity ?? 1, basketCount = draft.Items.Sum(i => i.Quantity) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAllItem(int id)
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return Json(new { error = "no_session" });

            var menuItem = await _db.MenuItems
               .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (menuItem == null)
                return Json(new { error = "item_not_found" });

            var draft = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SessionId == session.Id && o.IsDraft);

            if (draft == null)
                return Json(new { error = "draft_not_found" });

            var existing = draft.Items.FirstOrDefault(i => i.MenuItemId == id);

            if (existing == null)
                return Json(new { error = "item_not_in_basket" });

            draft.Items.Remove(existing);

            await _db.SaveChangesAsync();
            await BroadcastBasketUpdate(session.Id, session.TableNumber.ToString(), draft.Items.ToList());

            return Json(new { menuItemId = id, quantity = 0, basketCount = draft.Items.Sum(i => i.Quantity) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return RedirectToAction("Index", "Session");

            var draft = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.SessionId == session.Id && o.IsDraft);

            if (draft == null || !draft.Items.Any())
                return RedirectToAction("Index");

            foreach (var item in draft.Items)
                item.UnitPrice = item.MenuItem.Price;

            draft.IsDraft = false;
            draft.PlacedAt = DateTime.UtcNow;

            foreach (var item in draft.Items)
                item.Status = OrderStatus.Placed;

            await _db.SaveChangesAsync();
            await _hub.Clients
                .Group($"table-{session.TableNumber}")
                .SendAsync("OrderPlaced");

            return RedirectToAction("Index", "Basket");
        }
    }
}
