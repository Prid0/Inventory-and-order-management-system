namespace Pim.Model.Dtos

{
    public class OrderDetailsResultSet
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long PhoneNumber { get; set; }

        public int OrderNumber { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double LineTotal { get; set; }

        public int TotalQuantity { get; set; }
        public double OrderTotalValue { get; set; }

        public DateTime PlacedOn { get; set; }
        public DateTime? CancledOn { get; set; }
    }
}

