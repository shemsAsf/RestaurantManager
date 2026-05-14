using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers
{
    public class BasketController : CustomerControllerBase
    {
        public BasketController(AppDbContext db) : base(db)
        {
        }

        public async Task<IActionResult> Index()
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return RedirectToAction("Index", "Session");

            ViewData["ActiveTab"] = "basket";

            var draft = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.SessionId == session.Id && o.IsDraft);

            var previousOrders = await _db.Orders
                .Where(o => o.SessionId == session.Id && !o.IsDraft)
                .SelectMany(o => o.Items)
                .Include(i => i.MenuItem)
                .GroupBy(i => i.MenuItemId)
                .Select(i => new ItemViewModel()
                {
                    MenuItemId = i.Key,
                    Name = i.First().MenuItem.Name,
                    UnitPrice = i.First().MenuItem.Price,
                    Quantity = i.Sum(it => it.Quantity),
                })
                .ToListAsync();

            BasketViewModel bvm = new BasketViewModel
            {
                BasketItems = draft?.Items.Select(i => new ItemViewModel()
                {
                    MenuItemId = i.MenuItem.Id,
                    Name = i.MenuItem.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                }).ToList() ?? [],

                PreviousOrderItems = previousOrders,
                Total = previousOrders.Sum(i => i.Quantity * i.UnitPrice),
            };

            return View(bvm);
        }
    }
}
