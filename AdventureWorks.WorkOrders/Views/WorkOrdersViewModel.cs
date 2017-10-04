using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AdventureWorks.Model;
using AdventureWorks.WorkOrders.Commands;

namespace AdventureWorks.WorkOrders.Views
{
    public class WorkOrdersViewModel : ViewModelBase
    {
        private const int RecordsToProcess = 1000;
        private AdventureWorksEntities context = new AdventureWorksEntities(Properties.Settings.Default.ConnectionString);
		private Cursor currentCursor = Cursors.Arrow;
        private Stopwatch timer = new Stopwatch();
        private TimeSpan operationTime = TimeSpan.Zero;
        private RelayCommand ordersCommand;
        private RelayCommand<string> filterCommand;
        private int totalOrders;
        private int currentIndex;
        private bool isBusy;
        private WorkOrder highest;
        private WorkOrder scrapped;
        private int totalQty;

        public WorkOrdersViewModel(WorkOrdersView historyView) : base()
        {
            this.View = historyView;
            this.WorkOrders = new ObservableCollection<WorkOrder>();
        }

        public ICommand AllWorkOrdersCommand
        {
            get
            {
                if (null == this.ordersCommand)
                {
                    this.ordersCommand = new RelayCommand(
                        //this.GetWorkOrdersDispatcher,
                        //this.GetWorkOrdersThreadPool,
                        this.GetWorkOrdersBackground,
                        this.CanExecuteCommands);
                }

                return this.ordersCommand;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return this.currentIndex;
            }

            set
            {
                if (this.currentIndex != value)
                {
                    this.currentIndex = value;
                    this.OnPropertyChanged("CurrentIndex");
                }
            }
        }
		
		public Cursor Cursor
		{
			get
			{
				return this.currentCursor;
			}
			
			set
			{
				if (this.currentCursor != value)
				{
					this.currentCursor = value;
					this.OnPropertyChanged("Cursor");
				}
			}
		}

        public TimeSpan ElapsedTime 
        {
            get
            {
                return operationTime;
            }

            set
            {
                if (this.operationTime != value)
                {
                    this.operationTime = value;
                    this.OnPropertyChanged("ElapsedTime");
                }
            }
        }

        public ICommand FilterCommand
        {
            get
            {
                if (null == this.filterCommand)
                {
                    this.filterCommand = new RelayCommand<string>(
                        //this.FilterWorkOrders,
                        this.FilterWorkOrdersParallel,
                        this.CanExecuteCommands);
                }

                return this.filterCommand;
            }
        }

        public WorkOrder HighestQuantity
        {
            get
            {
                return this.highest;
            }

            set
            {
                if (this.highest != value)
                {
                    this.highest = value;
                    this.OnPropertyChanged("HighestQuantity");
                }
            }
        }

        public bool IsQueryActive 
        {
            get 
            { 
                return this.isBusy; 
            }

            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.OnPropertyChanged("IsQueryActive");
                }

                if (true == this.isBusy)
                {
					this.Cursor = Cursors.AppStarting;
                    this.ElapsedTime = TimeSpan.Zero;
                    this.timer.Start();
                }
                else
                {
                    this.timer.Stop();
					this.Cursor = Cursors.Arrow;
                    this.ElapsedTime = this.timer.Elapsed;
                    this.timer.Reset();
                }
            }
        }

        public WorkOrder MostScrapped
        {
            get
            {
                return this.scrapped;
            }

            set
            {
                if (this.scrapped != value)
                {
                    this.scrapped = value;
                    this.OnPropertyChanged("MostScrapped");
                }
            }
        }
        public int TotalQuantity
        {
            get
            {
                return this.totalQty;
            }

            set
            {
                if (this.totalQty != value)
                {
                    this.totalQty = value;
                    this.OnPropertyChanged("TotalQuantity");
                }
            }
        }

        public int TotalWorkOrders
        {
            get
            {
                return this.totalOrders;
            }

            set
            {
                if (this.totalOrders != value)
                {
                    this.totalOrders = value;
                    this.OnPropertyChanged("TotalWorkOrders");
                }
            }
        }

        public ObservableCollection<WorkOrder> WorkOrders { get; private set; }

        public WorkOrdersView View { get; set; }

        protected override void Dispose(bool disposing)
        {
            if ((true == disposing) && (null != this.context))
            {
                this.context.Dispose();
            }
        }

        private bool CanExecuteCommands(object param)
        {
            return !this.IsQueryActive;
        }

        private void Reset()
        {
            this.WorkOrders.Clear();
            this.CurrentIndex = 0;
            this.TotalWorkOrders = 0;
			this.TotalQuantity = 0;
			this.HighestQuantity = null;
			this.MostScrapped = null;
        }

        private List<WorkOrder> GetData()
        {
            var expression = from w in this.context.WorkOrders
                             select w;
            return ((ObjectQuery<WorkOrder>)expression)
                .Execute(MergeOption.AppendOnly)
                .Take(RecordsToProcess)
                .ToList();
        }

        #region Parallel Operations

        private void GetStatistics()
        {
            List<WorkOrder> workOrders = this.WorkOrders.ToList();
            if (workOrders.Count > 0)
            {
                Parallel.Invoke(
                    () => this.HighestQuantity = this.GetHighestQuantity(workOrders),
                    () => this.MostScrapped = this.GetMostScrapped(workOrders),
                    () => this.TotalQuantity = this.GetTotalQuantity(workOrders));
            }
        }

        private WorkOrder GetHighestQuantity(List<WorkOrder> workOrders)
        {
            return (from w in workOrders
                    group w by w.OrderQty into g
                    orderby g.Key descending
                    select g).First().First();
        }

        private WorkOrder GetMostScrapped(List<WorkOrder> workOrders)
        {
            return (from w in workOrders
                    group w by w.ScrappedQty into g
                    orderby g.Key descending
                    select g).First().First();
        }

        private int GetTotalQuantity(List<WorkOrder> workOrders)
        {
            return (from w in workOrders
                    select w).Sum(o => o.OrderQty);
        }

        #endregion Parallel Operations

        private void GetDataPage(int from, int count, List<WorkOrder> workOrders)
        {
            List<WorkOrder> results;
            lock (this.context)
            {
                var expression = (from w in this.context.WorkOrders
                                 select w).OrderBy(w => w.WorkOrderID).Skip(from).Take(count);
                results = ((ObjectQuery<WorkOrder>)expression)
                    .Execute(MergeOption.AppendOnly)
                    .ToList();
            }

            foreach (var item in results)
            {
                lock (workOrders)
                {
                    workOrders.Add(item);
                }
            }
        }

        #region Filter Operations

        private void FilterWorkOrders(string filter)
        {
            this.IsQueryActive = true;

            List<WorkOrder> toBeRemoved = new List<WorkOrder>();
            foreach (var item in this.WorkOrders)
            {
                if (false == item.Product.Name.ToLower().Contains(filter))
                {
                    toBeRemoved.Add(item);
                }
                Thread.SpinWait(1024 * 1000);
            }

            foreach (var item in toBeRemoved)
            {
                this.WorkOrders.Remove(item);
            }

            this.IsQueryActive = false;
        }

        private void FilterWorkOrdersParallel(string filter)
        {
            this.IsQueryActive = true;

            List<WorkOrder> toBeRemoved = new List<WorkOrder>();

            Parallel.ForEach<WorkOrder>(
                this.WorkOrders,
                (workOrder) =>
                {
                    if (false == workOrder.Product.Name.ToLower().Contains(filter))
                    {
                        toBeRemoved.Add(workOrder);
                    }

                    Thread.SpinWait(1024 * 1000);
                });

            foreach (var item in toBeRemoved)
            {
                this.WorkOrders.Remove(item);
            }

            this.GetStatistics();
            this.IsQueryActive = false;
        }

        #endregion

        #region Asynchronous Dispatcher Operations

        private void GetWorkOrdersDispatcher(object param)
        {
            this.Reset();
            this.IsQueryActive = true;

            var results = this.GetData();
            this.TotalWorkOrders = results.Count;
            for (int i = 0; i < results.Count; i++)
            {
                this.View.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action<WorkOrder, int>((workOrder, index) =>
                        {
                            this.WorkOrders.Add(workOrder);
                            this.CurrentIndex = index + 1;
                        }),
                    results[i], i);
            }

            this.View.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => 
                {
                    this.IsQueryActive = false;
                }));
        }

        #endregion Asynchronous Dispatcher Operations

        #region ThreadPool Operations

        private void GetWorkOrdersThreadPool(object param)
        {
            this.Reset();
            this.IsQueryActive = true;

            var dispatcher = Dispatcher.CurrentDispatcher;
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    var results = this.GetData();

                    this.TotalWorkOrders = results.Count;

                    for (int i = 0; i < results.Count; i++)
                    {
                        dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action<WorkOrder, int>((workOrder, index) =>
                            {
                                this.WorkOrders.Add(workOrder);
                                this.CurrentIndex = index + 1;
                            }),
                            results[i], i);
                    }

                    dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            this.IsQueryActive = false;
                        }));
                });
        }

        #endregion ThreadPool Operations

        #region BackgroundWorker Operations

        private void GetWorkOrdersBackground(object param)
        {
            this.Reset();
            this.IsQueryActive = true;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(this.GetWorkOrdersBackground);
            worker.ProgressChanged += new ProgressChangedEventHandler(this.GetWorkOrdersBackgroundProgress);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.GetWorkOrdersBackgroundComplete);
            worker.RunWorkerAsync();
        }

        private void GetWorkOrdersBackground(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker source = (BackgroundWorker)sender;

            var results = this.GetData();
            this.TotalWorkOrders = results.Count;

            int index = 0;
            foreach (var item in results)
            {
                // WPF's version of DoEvents. Makes this thread busy for long enough to free the UI thread to update.
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                source.ReportProgress(++index, item);
            }
        }

        private void GetWorkOrdersBackgroundProgress(object sender, ProgressChangedEventArgs e)
        {
            this.CurrentIndex = e.ProgressPercentage;
            this.WorkOrders.Add((WorkOrder)e.UserState);
        }

        private void GetWorkOrdersBackgroundComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.GetStatistics();
            this.IsQueryActive = false;
        }

        #endregion BackgroundWorkerOperations
    }
}
