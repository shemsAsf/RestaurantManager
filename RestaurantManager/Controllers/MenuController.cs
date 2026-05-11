using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Models;

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
            return View(categories);
        }

        public async Task<List<MenuCategory>> GetMenuAsync() =>
            await _db.Categories
                .Include(c => c.MenuItems.Where(m => m.IsAvailable).OrderBy(m => m.DisplayOrder))
                .Where(c => c.MenuItems.Any(m => m.IsAvailable))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
    }
}
