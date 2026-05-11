namespace RestaurantManager.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public TableSession Session { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PlacedAt { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }

    public enum OrderStatus { Draft, Placed, Done }
}
