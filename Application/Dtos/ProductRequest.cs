namespace Pim.Model.Dtos
{
    public class ProductRequest
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Discription { get; set; }
        public int Quantity { get; set; }
    }
}
