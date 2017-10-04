using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AdventureWorks.Model;
using AdventureWorks.WorkOrders.Commands;

namespace AdventureWorks.WorkOrders.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Member Data

        private IDataAccessService das;
        private bool isBusy;
        private Product selectedProductItem;
        private WorkOrder selectedWorkOrderItem;
        private IEnumerable<Product> products;
        private IEnumerable<WorkOrder> workOrders;
        private IEnumerable<WorkOrderRouting> workOrderRoutings;
        private MainWindowView view;
        private string skin;

        #endregion

        #region Constructor

        public MainWindowViewModel(MainWindowView mainView, IDataAccessService dataService, string skin)
            : base()
        {
            this.view = mainView;
            this.das = dataService;
            this.ChangeSkin(skin);
        }

        #endregion

        #region Properties

        public string CurrentSkin
        {
            get { return this.skin; }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            private set
            {
                this.isBusy = value;
                this.OnPropertyChanged("IsBusy");
            }
        }

        public Product SelectedProductItem
        {
            get
            {
                return this.selectedProductItem;
            }
            set
            {
                this.selectedProductItem = value;
                this.GetWorkOrders(((Product)selectedProductItem).ProductID);
            }
        }

        public WorkOrder SelectedWorkOrderItem
        {
            get
            {
                return this.selectedWorkOrderItem;
            }
            set
            {
                this.selectedWorkOrderItem = value;
                this.GetWorkOrderRoutings(((WorkOrder)selectedWorkOrderItem).WorkOrderID);
            }
        }

        public IEnumerable<Product> Products
        {
            get
            {
                return this.products;
            }
        }

        public IEnumerable<WorkOrder> WorkOrders
        {
            get
            {
                return this.workOrders;
            }
        }

        public IEnumerable<WorkOrderRouting> WorkOrderRoutings
        {
            get
            {
                return this.workOrderRoutings;
            }
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            das.ServiceDispose();
        }

        #endregion

        #region Commands

        private RelayCommand aboutBoxCommand;
        public ICommand AboutBoxCommand
        {
            get
            {
                if (this.aboutBoxCommand == null)
                {
                    this.aboutBoxCommand = new RelayCommand(param => this.ShowAboutBox(), param => true);
                }
                return this.aboutBoxCommand;
            }
        }

        private RelayCommand allProductsCommand;
        public ICommand AllProductsCommand
        {
            get
            {
                if (this.allProductsCommand == null)
                {
                    this.allProductsCommand = new RelayCommand(param => this.GetAllProducts(), param => true);
                }
                return this.allProductsCommand;
            }
        }

        private RelayCommand optionsCommand;
        public ICommand OptionsCommand
        {
            get
            {
                if (this.optionsCommand == null)
                {
                    this.optionsCommand = new RelayCommand(this.ShowOptions);
                }

                return this.optionsCommand;
            }
        }

        private RelayCommand workOrdersCommand;
        public ICommand WorkOrdersCommand
        {
            get
            {
                if (this.workOrdersCommand == null)
                {
                    this.workOrdersCommand = new RelayCommand(param => this.ShowWorkOrders(param));
                }

                return this.workOrdersCommand;
            }
        }

        private RelayCommand closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (this.closeCommand == null)
                {
                    this.closeCommand = new RelayCommand(param => Application.Current.Shutdown(), param => true);
                }
                return this.closeCommand;
            }
        }

        private RelayCommand changeSkinCommand;
        public ICommand ChangeSkinCommand
        {
            get
            {
                if (this.changeSkinCommand == null)
                {
                    this.changeSkinCommand = new RelayCommand(param => this.ChangeSkin(param), param => true);
                }
                return this.changeSkinCommand;
            }
        }

        private RelayCommand searchProductsCommand;
        public ICommand SearchProductsCommand
        {
            get
            {
                if (this.searchProductsCommand == null)
                {
                    this.searchProductsCommand = new RelayCommand(param => this.SearchProducts(param), param => true);
                }
                return this.searchProductsCommand;
            }
        }

        #endregion

        #region Helper Methods

        private void ChangeSkin(object param)
        {
            ResourceDictionary dict = Application.Current.Resources.MergedDictionaries[0];
            
            if (param.Equals("Shiny"))
            {
                dict.Source = new Uri("pack://application:,,,/AdventureWorks.Resources;component/Themes/ShinyBlue.xaml");
            }
            else
            {
                dict.Source = new Uri("pack://application:,,,/AdventureWorks.Resources;component/Themes/BureauBlue.xaml");
            }

            this.skin = (string)param;
        }

        private void GetAllProducts()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.IsBusy = true;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
                {
                    this.products = das.GetProductList();
                };
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                {
                    this.IsBusy = false;
                    Mouse.OverrideCursor = null;
                    this.OnPropertyChanged("Products");
                };
            worker.RunWorkerAsync();
        }

        private void GetWorkOrders(int id)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.IsBusy = true;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
                {
                    this.workOrders = das.GetWorkOrders(id);
                };
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                {
                    this.IsBusy = false;
                    Mouse.OverrideCursor = null;
                    this.OnPropertyChanged("WorkOrders");
                };
            worker.RunWorkerAsync();
        }

        private void GetWorkOrderRoutings(int id)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.IsBusy = true;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
                {
                    this.workOrderRoutings = das.GetWorkOrderRouting(id);
                };
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                {
                    this.IsBusy = false;
                    Mouse.OverrideCursor = null;
                    this.OnPropertyChanged("WorkOrderRoutings");
                };
            worker.RunWorkerAsync();
        }

        private void SearchProducts(object param)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.IsBusy = true;

            string[] parameters = ((string)param).Split(':');
            string searchField = parameters[2].Trim();
            BackgroundWorker worker = new BackgroundWorker();

            switch (searchField)
            {
                case "Maximum List Price":
                    decimal maxListPrice = Convert.ToDecimal(parameters[0]);
                    worker.DoWork += (object sender, DoWorkEventArgs e) =>
                        {
                            this.products = das.GetProductList(maxListPrice);
                        };
                    break;
                case "Stock Level":
                    int stockLevel = Convert.ToInt32(parameters[0]);
                    worker.DoWork += (object sender, DoWorkEventArgs e) =>
                        {
                            this.products = das.GetProductList(stockLevel);
                        };
                    break;
                case "Name:":
                default:
                    string productName = parameters[0];
                    worker.DoWork += (object sender, DoWorkEventArgs e) =>
                        {
                            this.products = das.GetProductList(productName);
                        };

                    break;
            }

            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                this.IsBusy = false;
                Mouse.OverrideCursor = null;
                this.OnPropertyChanged("Products");
            };
            worker.RunWorkerAsync();
        }

        private void ShowAboutBox()
        {
            AboutWindowView about = new AboutWindowView();
            about.Owner = Application.Current.MainWindow;
            about.ShowDialog();
        }

        private void ShowOptions(object ignore)
        {
            OptionsView view = new OptionsView();
            view.Owner = this.view;
            if (true == view.ShowDialog())
            {
                this.ChangeSkin(view.ViewModel.Skin);
                this.view.Left = view.ViewModel.Left;
                this.view.Top = view.ViewModel.Top;
                this.view.Width = view.ViewModel.Width;
                this.view.Height = view.ViewModel.Height;
            }
        }

        private void ShowWorkOrders(object param)
        {
            WorkOrdersView workOrders = new WorkOrdersView();
            workOrders.Owner = this.view;
            workOrders.ShowDialog();
        }

        #endregion
    }
}
