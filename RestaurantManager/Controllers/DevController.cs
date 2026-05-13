using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;

namespace RestaurantManager.Controllers
{
#if DEBUG
    public class DevController : Controller
    {
        private readonly AppDbContext _db;
        public DevController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Reset()
        {
            await _db.Database.ExecuteSqlRawAsync(@"
            TRUNCATE ""OrderItems"", ""Orders"", ""TableSessions"" RESTART IDENTITY CASCADE;
        ");
            return Ok("Reset done");
        }
    }
#endif
}
