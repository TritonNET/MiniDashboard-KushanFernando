using Microsoft.Extensions.DependencyInjection;
using MiniDashboard.App.ViewModels;
using MiniDashboard.App.Views;
using MiniDashboard.Client;
using MiniDashboard.Common;
using System.Windows;

namespace MiniDashboard.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddHttpClient<ProductClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:60812/product");
            });

            services.AddSingleton(provider =>
            {
                var backend = provider.GetRequiredService<ProductClient>();
                return new ProductCacheService(backend);
            });

            services.AddSingleton<IProductService>(provider => provider.GetRequiredService<ProductCacheService>());

            services.AddTransient<VmProducts>();
            services.AddTransient<ProductWindow>();

            Services = services.BuildServiceProvider();

            var window = Services.GetRequiredService<ProductWindow>();
            window.Show();
        }
    }

}
