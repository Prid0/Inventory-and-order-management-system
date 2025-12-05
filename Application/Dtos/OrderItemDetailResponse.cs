namespace Pim.Model.Dtos
{
    public class OrderItemDetailResponse
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double LineTotal { get; set; }
    }
}
