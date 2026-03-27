using MiniDashboard.App.ViewModels;
using System.Windows;

namespace MiniDashboard.App.Views
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        public ProductWindow(VmProducts vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ((VmProducts)DataContext).Initialize();
        }
    }
}
