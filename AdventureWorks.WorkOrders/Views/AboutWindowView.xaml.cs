using System.Windows;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for AboutWindowView.xaml
    /// </summary>
    public partial class AboutWindowView : Window
    {
        public AboutWindowView()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
