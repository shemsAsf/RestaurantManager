namespace RestaurantManager.ViewModels
{
    public class BasketViewModel
    {
        public List<ItemViewModel> BasketItems { get; set; }
        public List<ItemViewModel> PreviousOrderItems { get; set; }
        public decimal Total { get; set; } = 0;
    }

    public class ItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
