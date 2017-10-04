using System;
using System.Collections.ObjectModel;
using AdventureWorks.Model;
using AdventureWorks.WorkOrders.DragAndDrop;

namespace AdventureWorks.WorkOrders.Views
{
    public class ScratchPadViewModel : ViewModelBase
    {
        public ScratchPadViewModel(ScratchPadView scratchPad) : base()
        {
            this.View = scratchPad;
            this.WorkOrders = new ObservableCollection<WorkOrder>();

            DragDropHelper.Instance.Dropped += new Action<object, DragDropEventArgs>(this.OnDropped);
        }

        public ScratchPadView View { get; private set; }

        public ObservableCollection<WorkOrder> WorkOrders { get; private set; }

        private void OnDropped(object sender, DragDropEventArgs e)
        {
            if (sender == this.View)
            {
                this.WorkOrders.Add(e.DragDropData as WorkOrder);
            }
        }
    }
}
