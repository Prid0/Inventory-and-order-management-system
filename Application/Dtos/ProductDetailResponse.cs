namespace Pim.Model.Dtos
{
    public class ProductDetailResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Discription { get; set; }
        public string Category { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
