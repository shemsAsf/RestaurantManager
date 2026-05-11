using Microsoft.AspNetCore.Identity;
using RestaurantManager.Models;

namespace RestaurantManager.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, UserManager<IdentityUser> users, RoleManager<IdentityRole> roles)
        {
            foreach (var role in new[] { "Admin", "Manager", "Cook", "Waiter" })
                if (!await roles.RoleExistsAsync(role))
                    await roles.CreateAsync(new IdentityRole(role));

            #region Seed Admin Account
            if (await users.FindByNameAsync("admin") == null)
            {
                var admin = new IdentityUser { UserName = "admin" };
                await users.CreateAsync(admin, "Admin123!");
                await users.AddToRoleAsync(admin, "Admin");
            }
            #endregion

            #region Seed Menu
            if (!db.Categories.Any())
            {
                var starters = new MenuCategory { Name = "Starters", DisplayOrder = 1 };
                var mains = new MenuCategory { Name = "Mains", DisplayOrder = 2 };
                var drinks = new MenuCategory { Name = "Drinks", DisplayOrder = 3 };
                db.Categories.AddRange(starters, mains, drinks);

                db.MenuItems.AddRange(
                    new MenuItem { Category = starters, Name = "Bruschetta", Description = "Grilled bread with tomatoes", Price = 6.50m, ImageUrl = "/images/bruschetta.jpg", DisplayOrder = 1 },
                    new MenuItem { Category = mains, Name = "Margherita", Description = "Classic tomato and mozzarella", Price = 12.00m, ImageUrl = "/images/margherita.jpg", DisplayOrder = 1 },
                    new MenuItem { Category = drinks, Name = "Sparkling Water", Description = "75cl bottle", Price = 3.00m, ImageUrl = "/images/water.jpg", DisplayOrder = 1 }
                );
                await db.SaveChangesAsync();
            }
            #endregion
        }
    }
}
