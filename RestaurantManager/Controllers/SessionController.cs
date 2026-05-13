using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers
{
    public class SessionController : Controller
    {
        private readonly AppDbContext _db;

        public SessionController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session != null)
            {
                ViewData["TableNumber"] = session.TableNumber;
                ViewData["HasSession"] = true;
                return RedirectToAction("Index", "Menu");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Start(int tableNumber)
        {
            if (tableNumber < 1 || tableNumber > 99)
            {
                ModelState.AddModelError("", "Please enter a valid table number (1–99)");
                return View("Index");
            }
            
            var session = await _db.TableSessions
                .FirstOrDefaultAsync(s => s.TableNumber == tableNumber
                                       && s.ClosedAt == null);

            if (session == null)
            {
                session = new TableSession
                {
                    TableNumber = tableNumber,
                    OpenedAt = DateTime.UtcNow,
                };
                _db.TableSessions.Add(session);
                await _db.SaveChangesAsync();
            }

            Response.Cookies.Append("SessionId", session.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            });

            return RedirectToAction("Index", "Menu");
        }

        public IActionResult Ended()
        {
            Response.Cookies.Delete("SessionId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeTable()
        {
            Response.Cookies.Delete("SessionId");
            return RedirectToAction("Index");
        }
    }
}
