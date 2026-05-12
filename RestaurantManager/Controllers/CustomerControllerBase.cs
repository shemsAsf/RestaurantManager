using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Extensions;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers
{
    public class CustomerControllerBase : Controller
    {
        protected readonly AppDbContext _db;

        public CustomerControllerBase(AppDbContext db) => _db = db;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = await HttpContext.GetCurrentSessionAsync(_db);
            if (session != null)
            {
                ViewData["HasSession"] = true;
                ViewData["TableNumber"] = session.TableNumber;

                var basketCount = await _db.OrderItems
                    .Where(o => o.Order.SessionId == session.Id && o.Status == OrderStatus.Draft)
                    .SumAsync(i => i.Quantity);

                ViewData["BasketCount"] = basketCount;
            }
            await next();
        }
    }
}