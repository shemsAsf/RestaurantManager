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

            BasketViewModel bvm = new BasketViewModel {
                Items = draft?.Items.Select(i => new BasketItemViewModel()
                {
                    MenuItemId = i.MenuItem.Id,
                    Name = i.MenuItem.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                }).ToList() ?? []
            };

            return View(bvm);
        }
    }
}
