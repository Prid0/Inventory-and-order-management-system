namespace Pim.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime PlacedOn { get; set; }
        public DateTime? CancledOn { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
