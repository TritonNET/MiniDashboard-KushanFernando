namespace MiniDashboard.DataAccess
{
    public class tbl_product
    {
        public required Guid id { get; set; }

        public required string name { get; set; }

        public string? description { get; set; }

        public decimal price { get; set; }
    }
}
