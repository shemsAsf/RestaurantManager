using RestaurantManager.Models;

namespace RestaurantManager.ViewModels
{
    public class MenuViewModel
    {
        public List<MenuCategory> Categories { get; set; }
        public Dictionary<int, int> Quantities { get; set; } = new();
    }
}
