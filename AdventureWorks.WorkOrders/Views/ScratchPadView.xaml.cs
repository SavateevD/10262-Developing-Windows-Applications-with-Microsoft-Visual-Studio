using System.Windows;
using System.Windows.Controls;
using AdventureWorks.Model;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for ScratchPadView.xaml
    /// </summary>
    public partial class ScratchPadView : UserControl
    {
        public ScratchPadView()
        {
            this.ViewModel = new ScratchPadViewModel(this);
            InitializeComponent();
        }

        public ScratchPadViewModel ViewModel
        {
            get { return this.DataContext as ScratchPadViewModel; }
            set { this.DataContext = value; }
        }

        //private void OnDragEnter(object sender, DragEventArgs e)
        //{
        //    if (false == e.Data.GetDataPresent("WorkOrder"))
        //    {
        //        e.Effects = DragDropEffects.None;
        //    }
        //}

        //private void OnDrop(object sender, DragEventArgs e)
        //{
        //    if (true == e.Data.GetDataPresent("WorkOrder"))
        //    {
        //        WorkOrder data = e.Data.GetData("WorkOrder") as WorkOrder;
        //        this.ViewModel.OnDrop(data);
        //    }
        //}
    }
}
