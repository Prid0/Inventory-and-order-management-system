namespace Pim.Model.Dtos
{
    public class OrdersResutSet
    {
        public string UserName { get; set; }
        public long PhoneNumber { get; set; }

        public int OrderNumber { get; set; }

        public double OrderTotalValue { get; set; }

        public DateTime PlacedOn { get; set; }
        public DateTime? CancledOn { get; set; }
    }
}
