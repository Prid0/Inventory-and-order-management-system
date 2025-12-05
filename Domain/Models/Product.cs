namespace Pim.Data.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Discription { get; set; }
        public int Quantity { get; set; }
    }
}
