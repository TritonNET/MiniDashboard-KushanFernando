using MiniDashboard.Common.Models;

namespace MiniDashboard.App.EntityModels
{
    public class EmProduct
    {
        private Product m_product;

        public Guid? ID 
        {
            get => m_product.ID;
            set => m_product.ID = value;
        }

        public string? Name 
        {
            get => m_product.Name; 
            set => m_product.Name = value; 
        }

        public string Description 
        {
            get => m_product.Description;
            set => m_product.Description = value;
        }

        public decimal Price
        {
            get => m_product.Price;
            set => m_product.Price = value;
        }

        public EmProduct(Product product)
        {
            m_product = product;
        }

        public EmProduct()
        {
            m_product = new Product
            {
                ID = Guid.NewGuid()
            };
        }

        public void Reset()
        {
            ID = Guid.NewGuid();
            Name=string.Empty;
            Description = string.Empty;
            Price = 0;
        }

        public static explicit operator Product(EmProduct em)
        {
            return em.m_product;
        }
    }
}
