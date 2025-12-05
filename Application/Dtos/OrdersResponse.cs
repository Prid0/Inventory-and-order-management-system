namespace Pim.Model.Dtos
{
    public class OrdersResponse
    {
        public string UserName { get; set; }
        public long PhoneNumber { get; set; }

        public int OrderNumber { get; set; }

        public double OrderTotalValue { get; set; }

        public string PlacedOn { get; set; }
        public string CancledOn { get; set; }
    }
}
