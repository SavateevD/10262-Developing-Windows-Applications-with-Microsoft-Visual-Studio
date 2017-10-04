using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdventureWorks.Model;

namespace AdventureWorks.WorkOrders.Views
{
    /// <summary>
    /// Interaction logic for WorkOrderView.xaml
    /// </summary>
    public partial class WorkOrderView : UserControl
    {
        //private Point startPoint;

        public WorkOrderView()
        {
            InitializeComponent();
        }

        //private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.startPoint = e.GetPosition(null);
        //}

        //private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (null != this.DataContext)
        //    {
        //        Point currentPoint = e.GetPosition(null);
        //        Vector difference = this.startPoint - currentPoint;

        //        // Don't start the DragDrop operation until the user has moved the mouse by a few pixels.
        //        // This value is defined in a system constant.
        //        if ((MouseButtonState.Pressed == e.LeftButton) &&
        //            (Math.Abs(difference.X) > SystemParameters.MinimumHorizontalDragDistance) &&
        //            (Math.Abs(difference.Y) > SystemParameters.MinimumVerticalDragDistance))
        //        {
        //            WorkOrder data = this.DataContext as WorkOrder;
        //            DataObject dragData = new DataObject("WorkOrder", data);
        //            DragDrop.DoDragDrop(this, dragData, DragDropEffects.Copy);
        //        }
        //    }
        //}
    }
}
