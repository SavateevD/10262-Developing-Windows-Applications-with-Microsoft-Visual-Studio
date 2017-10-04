using System.Collections.Generic;
using AdventureWorks.WorkOrders.Properties;

namespace AdventureWorks.WorkOrders.Views
{
    public class OptionsViewModel : ViewModelBase
    {
        private List<string> skinNames = new List<string>(2) { "Bureau", "Shiny" };

        public OptionsViewModel() : base()
        {
        }

        public double Left
        {
            get { return Settings.Default.Left; }
            set { Settings.Default.Left = value; }
        }

        public double Top
        {
            get { return Settings.Default.Top; }
            set { Settings.Default.Top = value; }
        }

        public double Width
        {
            get { return Settings.Default.Width; }
            set { Settings.Default.Width = value; }
        }

        public double Height
        {
            get { return Settings.Default.Height; }
            set { Settings.Default.Height = value; }
        }

        public string Skin
        {
            get { return Settings.Default.Skin; }
            set { Settings.Default.Skin = value; }
        }

        public List<string> Skins
        {
            get { return this.skinNames; }
        }

        public string ConnectionString
        {
            get { return Settings.Default.ConnectionString; }
        }

        public void Save()
        {
            Settings.Default.Save();
        }

        public void Reload()
        {
            Settings.Default.Reload();
        }
    }
}
