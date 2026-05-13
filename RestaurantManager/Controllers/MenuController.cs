using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Models;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers
{
    public class MenuController : CustomerControllerBase
    {
        public MenuController(AppDbContext db) : base(db)
        {
        }

        public async Task<IActionResult> Index()
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session == null)
                return RedirectToAction("Index", "Session");

            ViewData["ActiveTab"] = "menu";

            var categories = await GetMenuAsync();
            var quantities = await GetItemQuantitiesAsync(session.Id);

            return View(new MenuViewModel() { Categories = categories, Quantities = quantities });
        }

        public async Task<List<MenuCategory>> GetMenuAsync() =>
            await _db.Categories
                .Include(c => c.MenuItems.Where(m => m.IsAvailable).OrderBy(m => m.DisplayOrder))
                .Where(c => c.MenuItems.Any(m => m.IsAvailable))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

        public async Task<Dictionary<int, int>> GetItemQuantitiesAsync(int sessionId) =>
            await _db.Orders
                .Where(o => o.SessionId == sessionId && o.IsDraft)
                .SelectMany(o => o.Items)
                .ToDictionaryAsync(i => i.MenuItemId, i => i.Quantity);
    }
}
