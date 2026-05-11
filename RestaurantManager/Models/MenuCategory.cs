namespace RestaurantManager.Models
{
    public class MenuCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
