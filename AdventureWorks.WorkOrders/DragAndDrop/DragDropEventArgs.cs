using System;

namespace AdventureWorks.WorkOrders.DragAndDrop
{
    public class DragDropEventArgs : EventArgs
    {
        public DragDropEventArgs() : base()
        {
        }

        public DragDropEventArgs(object data)
            : this()
        {
            this.DragDropData = data;
        }

        public object DragDropData { get; set; }
    }
}
