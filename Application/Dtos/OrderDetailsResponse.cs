namespace Pim.Model.Dtos
{
    public class OrderDetailsResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long PhoneNumber { get; set; }

        public int OrderNumber { get; set; }

        public int TotalQuantity { get; set; }
        public double OrderTotalValue { get; set; }

        public string PlacedOn { get; set; }
        public string? CancledOn { get; set; }

        public List<OrderItemDetailResponse> OrderItems { get; set; }
    }
}
