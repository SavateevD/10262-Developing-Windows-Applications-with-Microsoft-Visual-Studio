using System.Windows;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml.
    /// </summary>
    public partial class OptionsView : Window
    {
        public OptionsView()
        {
            this.ViewModel = new OptionsViewModel();

            InitializeComponent();
        }

        public OptionsViewModel ViewModel
        {
            get { return this.DataContext as OptionsViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            this.ViewModel.Dispose();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Save();
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Reload();
            this.DialogResult = false;
        }
    }
}
