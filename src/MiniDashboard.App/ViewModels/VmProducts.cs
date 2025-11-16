using MiniDashboard.App.EntityModels;
using MiniDashboard.App.Infrastructure;
using MiniDashboard.Common;
using MiniDashboard.Common.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MiniDashboard.App.ViewModels
{
    public class VmProducts : VmBase
    {
        private readonly IProductService m_productService;

        public ObservableCollection<EmProduct> Products { get; }

        public ICommand DeleteProductCommand { get; }

        public ICommand FilterVisibilityCommand { get; }

        public ICommand FilterProductsCommand { get; }

        public ICommand AddProductCommand { get; }

        public ICommand CancelNewProductCommand { get; }

        public ICommand SaveNewProductCommand { get; }

        public EmProductFilter Filter { get; }

        public EmProduct NewProduct { get; set; }

        public VmProducts(IProductService productService)
        {
            NewProduct = new EmProduct();
            Filter = new EmProductFilter();
            Products = new ObservableCollection<EmProduct>();
            
            Products.Add(new EmProduct(new Product
            {
                ID = Guid.NewGuid(),
                Name = "Product One",
                Description = "Product One Description",
                Price = 10
            }));

            Products.Add(new EmProduct(new Product
            {
                ID = Guid.NewGuid(),
                Name = "Product Two",
                Description = "Product Two Description",
                Price = 20
            }));
            Products.Add(new EmProduct(new Product
            {
                ID = Guid.NewGuid(),
                Name = "Product Three",
                Description = "Product Three Description",
                Price = 30
            }));
            Products.Add(new EmProduct(new Product
            {
                ID = Guid.NewGuid(),
                Name = "Product Four",
                Description = "Product Four Description",
                Price = 40
            }));
            Products.Add(new EmProduct(new Product
            {
                ID = Guid.NewGuid(),
                Name = "Product Five",
                Description = "Product Five Description",
                Price = 50
            }));

            m_productService = productService;

            DeleteProductCommand = new AsyncRelayCommand<EmProduct>(OnDeleteProductAsync);

            FilterVisibilityCommand = new RelayCommand(OnFilterVisibility);

            FilterProductsCommand = new AsyncRelayCommand(OnFilterProductAsync);

            AddProductCommand = new RelayCommand(OnAddProduct);

            CancelNewProductCommand = new RelayCommand(OnCancelNewProduct);
            SaveNewProductCommand = new AsyncRelayCommand(OnSaveNewProductAsync);
        }

        public override Task InitializeAsync()
        {
            try
            {
                return OnFilterProductAsync();
            }
            catch (Exception ex)
            {
                ShowStatusMessage(StatusMessageType.Error, ex.Message);
            }

            return base.InitializeAsync();
        }

        private bool m_isFilterVisible = false;
        public bool IsFilterVisible
        {
            get => m_isFilterVisible;
            set => SetProperty(ref m_isFilterVisible, value);
        }

        private bool m_addingProduct = false;
        public bool AddingProduct
        {
            get => m_addingProduct;
            set => SetProperty(ref m_addingProduct, value);
        }

        private async Task OnDeleteProductAsync(EmProduct product)
        {
            if (product == null || product.ID == null)
                return;

            IsBusy = true;

            try
            {
                bool deleted = await m_productService.DeleteProductAsync(product.ID.Value, CancellationToken.None);

                if (deleted)
                {
                    Products.Remove(product);
                }
                else
                {

                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnFilterProductAsync()
        {
            IsBusy = true;

            var products = await m_productService.GetProductsAsync((ProductFilter)Filter, CancellationToken.None);

            Products.Clear();

            foreach (var product in products)
                Products.Add(new EmProduct(product));

            IsBusy = false;
        }

        private void OnFilterVisibility()
        {
            IsFilterVisible = !IsFilterVisible;
            OnPropertyChanged(nameof(Filter));
        }

        private void OnAddProduct()
        {
            AddingProduct = true;
        }

        private void OnCancelNewProduct()
        {
            NewProduct.Reset();
            AddingProduct = false;
        }

        private async Task OnSaveNewProductAsync()
        {
            try
            {
                var newID = await m_productService.AddProductAsync((Product)NewProduct, CancellationToken.None);

                var newProduct = await m_productService.GetProductAsync(newID, CancellationToken.None);

                Products.Add(new EmProduct(newProduct));

                NewProduct.Reset();
                AddingProduct  = false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
