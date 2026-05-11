using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models;

namespace RestaurantManager.Extensions
{
    public static class SessionExtensions
    {
        public static async Task<TableSession?> GetCurrentSessionAsync(
            this HttpContext ctx, AppDbContext db)
        {
            if (!ctx.Request.Cookies.TryGetValue("SessionId", out var raw)) return null;
            if (!int.TryParse(raw, out var id)) return null;

            return await db.TableSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.ClosedAt == null);
        }
    }
}
