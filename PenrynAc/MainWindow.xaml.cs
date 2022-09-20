using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jbh;

namespace PenrynAc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private List<TextBlock> plabels = new List<TextBlock>();
        //private List<Button> pOpenButtons = new List<Button>();
        //private List<Button> pEditButtons = new List<Button>();

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var stephensonImage = System.IO.Path.Combine(AppManager.DataPath, "John Cecil Stephenson_03.jpg");
            var imageUri = new Uri(stephensonImage);
            imageIllustration.Source = imageUri.GetBitmapImage(BitmapCacheOption.OnLoad);
            imageIllustration.Stretch = Stretch.Uniform;
        }

        //private void CreatePropertyControls(int Top)
        //{
        //    gridPropertyList.RowDefinitions.Clear();
        //    // make 4 rows for pretty spacing - could be more with resized window
        //    plabels.Clear();
        //    pOpenButtons.Clear();
        //    pEditButtons.Clear();
        //    for (int x = gridPropertyList.RowDefinitions.Count(); x < 4; x++)
        //    { gridPropertyList.RowDefinitions.Add(new RowDefinition()); }

        //    for (int x = 0; x < Top; x++)
        //    {
        //        TextBlock tbk;

        //        Button btnOpen;
        //        Button btnEdit;

        //        if (x == 0)
        //        {
        //            tbk = textblockFirstProperty;
        //            tbk.Name = "PropertyLabel0";
        //            btnOpen = buttonFirstOpen;
        //            btnEdit = buttonFirstEdit;
        //        }
        //        else
        //        {
        //            tbk = new TextBlock();
        //            tbk.Name = "PropertyLabel" + x.ToString();
        //            gridPropertyList.Children.Add(tbk);
        //            tbk.Background = textblockFirstProperty.Background;
        //            tbk.Foreground = textblockFirstProperty.Foreground;
        //            tbk.VerticalAlignment = textblockFirstProperty.VerticalAlignment;
        //            tbk.HorizontalAlignment = textblockFirstProperty.HorizontalAlignment;
        //            tbk.Width = textblockFirstProperty.Width;
        //            tbk.Height = textblockFirstProperty.Height;
        //            tbk.Margin = textblockFirstProperty.Margin;
        //            tbk.Padding = textblockFirstProperty.Padding;
        //            tbk.FontFamily = textblockFirstProperty.FontFamily;
        //            tbk.FontSize = textblockFirstProperty.FontSize;
        //            tbk.FontWeight = textblockFirstProperty.FontWeight;

        //            btnOpen = new Button();
        //            gridPropertyList.Children.Add(btnOpen);
        //            btnOpen.VerticalAlignment = buttonFirstOpen.VerticalAlignment;
        //            btnOpen.HorizontalAlignment = buttonFirstOpen.HorizontalAlignment;
        //            btnOpen.Width = buttonFirstOpen.Width;
        //            btnOpen.Height = buttonFirstOpen.Height;
        //            btnOpen.Margin = buttonFirstOpen.Margin;
        //            btnOpen.Padding = buttonFirstOpen.Padding;
        //            btnOpen.Content = "Open";

        //            btnEdit = new Button();
        //            gridPropertyList.Children.Add(btnEdit);
        //            btnEdit.VerticalAlignment = buttonFirstEdit.VerticalAlignment;
        //            btnEdit.HorizontalAlignment = buttonFirstEdit.HorizontalAlignment;
        //            btnEdit.Width = buttonFirstEdit.Width;
        //            btnEdit.Height = buttonFirstEdit.Height;
        //            btnEdit.Margin = buttonFirstEdit.Margin;
        //            btnEdit.Padding = buttonFirstEdit.Padding;
        //            btnEdit.Content = "Edit";
        //        }
        //        plabels.Add(tbk);
        //        pOpenButtons.Add(btnOpen);
        //        pEditButtons.Add(btnEdit);
        //        Grid.SetColumn(tbk, 0);
        //        Grid.SetRow(tbk, x);
        //        Grid.SetColumn(btnOpen, 1);
        //        Grid.SetColumn(btnEdit, 2);
        //        Grid.SetRow(btnOpen, x);
        //        Grid.SetRow(btnEdit, x);
        //        tbk.Text = "Prop" + x.ToString();
        //    }
        //}

        //private void RefreshPropertyList()
        //{
        //    string dpath = Jbh.AppManager.DataPath;
        //    string[] u;
        //    string p;

        //    u = System.IO.Directory.GetFiles(dpath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);

        //    int x = 0;
        //    foreach (string s in u)
        //    {
        //        if (!s.EndsWith("_runtime.txt"))
        //        {
        //            x++;
        //        }
        //    }

        //    CreatePropertyControls(x);

        //    x = 0;
        //    foreach (string s in u)
        //    {
        //        if (!s.EndsWith("_runtime.txt"))
        //        {
        //            p = s.Substring(dpath.Length + 1);
        //            p = p.Substring(0, p.Length - 4);
        //            plabels[x].Text = p;
        //            pOpenButtons[x].Tag = p;
        //            pEditButtons[x].Tag = p;
        //            pOpenButtons[x].Click += new RoutedEventHandler(buttonPropertyOpen_Click);
        //            pEditButtons[x].Click += new RoutedEventHandler(buttonPropertyEdit_Click);
        //            x++;
        //        }
        //    }
        //}
        // TODO Review need for backups window (or will AppManager cope?)
        //private void ButtonBackups_Click(object sender, RoutedEventArgs e)
        //{
        //    BackupsWindow bw = new BackupsWindow();
        //    bw.Owner = this;
        //    bw.ShowDialog();
        //}

        private void buttonPropertyOpen_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = (Button)sender;
            //string SelectedProperty = (string)btn.Tag;
            Core.Accounts = new PropertyAccounts();
            PropertyAccountsWindow wdw = new PropertyAccountsWindow();
            wdw.Owner = this;
            wdw.ShowDialog();
            Core.Accounts.SaveData();
        }

        private void buttonPropertyEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string SelectedProperty = (string)btn.Tag;
            Core.Accounts = new PropertyAccounts();
            PropertyDetailsWindow wdw = new PropertyDetailsWindow
            {
                Owner = this
            };
            if (wdw.ShowDialog() == (bool)true)
            {
                PropertyAccounts ac = Core.Accounts;
                ac.PropertyAddress = wdw.PropertyAddress;
                //ac.PropertyName = wdw.PropertyTitle;
                ac.PropertyPurchaseCost = wdw.PropertyPurchaseCost;
                ac.LandlordShares.Specification = wdw.PropertySharingSpecification;
                ac.PropertyPurchaseDate = wdw.PropertyPurchaseDate;
                ac.SaveData();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Jbh.AppManager.DataPath)) { MessageBox.Show("Path to data is not found.\n\nLettings will close.", "Databank is not accessible", MessageBoxButton.OK, MessageBoxImage.Error); Close(); }
            textblockAppRev.Text = Core.AppRevDateString();
            //RefreshPropertyList();
        }
    }
}