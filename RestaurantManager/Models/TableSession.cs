namespace RestaurantManager.Models
{
    public class TableSession
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
