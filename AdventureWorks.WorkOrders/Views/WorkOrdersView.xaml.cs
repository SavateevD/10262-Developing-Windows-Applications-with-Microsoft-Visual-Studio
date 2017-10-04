using System.ComponentModel;
using System.Windows;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for WorkOrdersView.xaml
    /// </summary>
    public partial class WorkOrdersView : Window
    {
        public WorkOrdersView()
        {
            this.ViewModel = new WorkOrdersViewModel(this);

            InitializeComponent();
        }

        public WorkOrdersViewModel ViewModel 
        {
            get { return this.DataContext as WorkOrdersViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.ViewModel.Dispose();
            this._scratchPad.ViewModel.Dispose();
        }
    }
}
