using System.Windows;
using AdventureWorks.Model;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        public MainWindowViewModel ViewModel
        {
            get { return this.DataContext as MainWindowViewModel; }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.ViewModel.Dispose();
        }

        private void products_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.products.SelectedItem != null)
            {
                this.ViewModel.SelectedProductItem = this.products.SelectedItem as Product;
            }
        }

        private void workOrders_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.workOrders.SelectedItem != null)
            {
                this.ViewModel.SelectedWorkOrderItem = this.workOrders.SelectedItem as WorkOrder;
            }
        }
    }
}