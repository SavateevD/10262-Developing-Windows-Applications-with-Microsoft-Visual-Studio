using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdventureWorks.WorkOrders.DragAndDrop
{
    public class DragDropHelper
    {
        public const string DataFormat = "WorkOrder";

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached(
                "IsDragSource", 
                typeof(bool), 
                typeof(DragDropHelper), 
                new UIPropertyMetadata(false, IsDragSourceChanged));
        
        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached(
                "IsDropTarget",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        public event Action<object, DragDropEventArgs> Dropped = delegate { };

        private static readonly DragDropHelper instance = new DragDropHelper();
        private FrameworkElement dragSource;
        private object dragData;
        private Point dragStart;
        private Window dragWindow;

        static DragDropHelper()
        {
        }

        private DragDropHelper()
        {
        }

        public static DragDropHelper Instance
        {
            get { return instance; }
        }

        public static bool GetIsDragSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }

        public static bool GetIsDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDragSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragSourceProperty, value);
        }

        public static void SetIsDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropTargetProperty, value);
        }

        private static void IsDragSourceChanged(
            DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            var source = d as FrameworkElement;
            if (null != source)
            {
                if (true == object.Equals(e.NewValue, true))
                {
                    // Add event handlers to initiate dragging.
                    source.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                    source.PreviewMouseRightButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                    source.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
                }
                else
                {
                    // Remove event handlers for initiating dragging.
                    source.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                    source.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                    source.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
                }
            }
        }

        private static void IsDropTargetChanged(
            DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (null != target)
            {
                if (true == object.Equals(e.NewValue, true))
                {
                    // Add event handlers to handle dropping.
                    target.PreviewDrop += Instance.DropTarget_PreviewDrop;
                    target.PreviewDragEnter += Instance.DropTarget_PreviewDragEnter;
                    target.PreviewDragOver += Instance.DropTarget_PreviewDragOver;

                    target.AllowDrop = true;
                }
                else
                {
                    //  Remove event handlers that handle dropping.
                    target.PreviewDrop -= Instance.DropTarget_PreviewDrop;
                    target.PreviewDragEnter -= Instance.DropTarget_PreviewDragEnter;
                    target.PreviewDragOver -= Instance.DropTarget_PreviewDragOver;

                    target.AllowDrop = false;
                }
            }
        }

        protected virtual void OnDropped(object sender, object data)
        {
            this.Dropped(sender, new DragDropEventArgs(data));
        }

        #region Drag Handling

        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.dragStart = e.GetPosition(null);
            this.dragSource = sender as FrameworkElement;
            if (null != this.dragSource)
            {
                this.dragData = this.dragSource.DataContext;
            }
        }

        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dragData = null;
        }

        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (null != this.dragData)
            {
                Point currentPosition = e.GetPosition(null);
                Vector difference = this.dragStart - currentPosition;

                // Don't start the DragDrop operation until the user has moved the mouse by a few pixels.
                // This value is defined in a system constant.
                if ((MouseButtonState.Pressed == e.LeftButton) &&
                    ((Math.Abs(difference.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(difference.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    this.dragSource.QueryContinueDrag += new QueryContinueDragEventHandler(this.DragSource_QueryContinueDrag);
                    this.CreateDragWindow(this.dragSource);
                    this.dragWindow.Show();

                    DataObject data = new DataObject(DataFormat, this.dragData);
                    DragDrop.DoDragDrop(this.dragSource, data, DragDropEffects.Copy);

                    this.DestroyDragWindow();
                    this.dragSource.QueryContinueDrag -= this.DragSource_QueryContinueDrag;

                    this.dragData = null;
                }
            }
        }

        private void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            this.UpdateDragWindowLocation();
        }

        private void CreateDragWindow(Visual dragElement)
        {
            this.dragWindow = new Window();
            this.dragWindow.WindowStyle = WindowStyle.None;
            this.dragWindow.AllowsTransparency = true;
            this.dragWindow.AllowDrop = false;
            this.dragWindow.Background = null;
            this.dragWindow.IsHitTestVisible = false;
            this.dragWindow.SizeToContent = SizeToContent.WidthAndHeight;
            this.dragWindow.Topmost = true;
            this.dragWindow.ShowInTaskbar = false;

            this.dragWindow.SourceInitialized += (object sender, EventArgs e) =>
                {
                    PresentationSource windowSource = PresentationSource.FromVisual(this.dragWindow);
                    IntPtr handle = ((HwndSource)windowSource).Handle;

                    Int32 styles = Win32.GetWindowLong(handle, Win32.GWL_EXSTYLE);
                    Win32.SetWindowLong(handle, Win32.GWL_EXSTYLE, styles | Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT);
                };

            Rectangle rect = new Rectangle();
            rect.Width = ((FrameworkElement)dragElement).ActualWidth;
            rect.Height = ((FrameworkElement)dragElement).ActualHeight;
            rect.Fill = new VisualBrush(dragElement);
            rect.Opacity = 1;
            LinearGradientBrush opacityBrush = new LinearGradientBrush();
            opacityBrush.EndPoint = new Point(0.3, 1.0);
            opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, 0));
            opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
            rect.OpacityMask = opacityBrush;
            this.dragWindow.Content = rect;

            this.UpdateDragWindowLocation();
        }

        private void DestroyDragWindow()
        {
            if (null != this.dragWindow)
            {
                this.dragWindow.Close();
                this.dragWindow = null;
            }
        }

        private void UpdateDragWindowLocation()
        {
            if (null != this.dragWindow)
            {
                Win32.POINT currentPosition;
                if (false == Win32.GetCursorPos(out currentPosition))
                {
                    return;
                }

                this.dragWindow.Left = currentPosition.X;
                this.dragWindow.Top = currentPosition.Y;
            }
        }

        private void Root_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Root_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        #endregion Drag Handling

        #region  Drop Handling

        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (false == e.Data.GetDataPresent(DataFormat))
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (false == e.Data.GetDataPresent(DataFormat))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var target = sender as FrameworkElement;
            if (null != target)
            {
                if (true == e.Data.GetDataPresent(DataFormat))
                {
                    this.OnDropped(target, e.Data.GetData(DataFormat));
                }
            }

            e.Handled = true;
        }

        #endregion Drop  Handling
    }
}
