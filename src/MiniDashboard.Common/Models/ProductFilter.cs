namespace MiniDashboard.Common.Models
{
    public class ProductFilter
    {
        public StringFilter? Name { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
    }
}
