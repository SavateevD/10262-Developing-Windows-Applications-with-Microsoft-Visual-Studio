using System.Globalization;
using System.Threading;
using System.Windows;
using AdventureWorks.Model;
using AdventureWorks.WorkOrders.Views;

namespace AdventureWorks.WorkOrders
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindowView window;

        public App() : base()
        {
            string culture = "fr-FR";
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.window = new MainWindowView();

            // Read the position settings and position the window accordingly.
            OptionsViewModel options = new OptionsViewModel();
            this.window.Left = options.Left;
            this.window.Top = options.Top;
            this.window.Width = options.Width;
            this.window.Height = options.Height;

            // Show the main window.
            var dataService = new DataAccessService(options.ConnectionString);
            window.DataContext = new MainWindowViewModel(window, dataService, options.Skin);
            options.Dispose();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Save the position settings.
            OptionsViewModel options = new OptionsViewModel();
            options.Left = this.window.Left;
            options.Top = this.window.Top;
            options.Width = this.window.ActualWidth;
            options.Height = this.window.ActualHeight;
            options.Skin = this.window.ViewModel.CurrentSkin;
            options.Save();
            options.Dispose();
        }
    }
}
