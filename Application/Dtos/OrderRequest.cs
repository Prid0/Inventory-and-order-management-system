namespace Pim.Model.Dtos
{
    public class OrderRequest
    {
        public int OrderId { get; set; }
        public List<OrderItemRequest> OrderItem { get; set; }
    }
}
