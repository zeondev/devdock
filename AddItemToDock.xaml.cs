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
using System.Windows.Shapes;
using System.IO;

namespace DevDock
{
    /// <summary>
    /// Interaction logic for AddItemToDock.xaml
    /// </summary>
    public partial class AddItemToDock : Window
    {
        MainWindow winMain;
        public AddItemToDock()
        {
            InitializeComponent();
            winMain = this.Owner as MainWindow;
            // testing stuff
        }

        private void DragWin(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWin(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            
        }

        private void OKClick(object sender, MouseButtonEventArgs e)
        {
            string itmName = tbName.Text;
            string itmIcon = tbIcon.Text;
            string itmPath = tbPath.Text;
            MainWindow mwin = new MainWindow();
            DockApps itm = new()
            {
                Name = itmName,
                Icon = itmIcon,
                Path = itmPath
            };
            try
            {
                if (!File.Exists(itmPath))
                {
                    MessageBox.Show("Invalid file path.. please pick a valid one.");
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Something went wrong trying to check the path..\ndo you have permissions?\n" + err.Message);
                return;
            }
            this.Hide();
            if (winMain.Dock_AddItem(itm))
            {
                int index = mwin.currentDockInfo.Apps.Count;
                // MessageBox.Show("Index: " + index.ToString() + "\nyou have been warned error may be coming soon!!");
                string tpy = mwin.currentDockInfo.Apps[index - 1].Name;
                
                // MessageBox.Show("SUCCESS!!!!!!!!!1\nLet's get the dock item name: " + tpy);
                return;
            }
            else
            {
                if (winMain.currentError != null)
                {
                    MessageBox.Show("Oops an error occured.. :(\n" + mwin.currentError);
                }
                else
                {
                    MessageBox.Show("Oops an error occured.. :(");
                }
                return;
            }
        }

        private void tbIcon_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                imgIcon.Source = new BitmapImage(new Uri(tbIcon.Text));

            }
            catch (Exception    err)
            {
                imgIcon.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/err.png"));
            }
        }
    }
}
