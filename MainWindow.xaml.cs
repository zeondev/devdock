using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace DevDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class DockInfo
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("apps")]
        public List<DockApps> Apps { get; set; }
    }
    public class DockApps
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public partial class MainWindow : Window
    {
        public string currentver = "0.4b";
        public string currentError;
        public DockInfo currentDockInfo;
        public string pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DevDock\";
        public string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DevDock\settings.json";
        public MainWindow cwin;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
            cwin = this;

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {

                TimeContent.Text = DateTime.Now.ToString("hh:mm tt");
            }, this.Dispatcher);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string settingsFile = File.ReadAllText(path);
            NewItemModal.Visibility = Visibility.Collapsed;
            if (!File.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DevDock\settings.json") || string.IsNullOrEmpty(settingsFile))
            {
                string text = "{\"version\":\"" + currentver + "\",\"date\":\"" + DateTime.Today.ToShortDateString() + "\",\"apps\": []}";

                await File.WriteAllTextAsync(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DevDock\settings.json", text);
            }
            else
            {
                DockInfo deserialized = JsonConvert.DeserializeObject<DockInfo>(settingsFile);
                currentDockInfo = deserialized;
                string newstr = JsonConvert.SerializeObject(deserialized);
                Dock_Refresh();
            }
            // double left = desktopWorkingArea.Right - this.Width;
            // double bottom = desktopWorkingArea.Bottom - 40;
            // 0.3b: removed all this

        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NewItemModal.Visibility = Visibility.Visible;
        }

        public void ClosingWindow()
        {

        }

        public bool Dock_AddItem(DockApps app)
        {
            try
            {
                string settingsFile = File.ReadAllText(path);
                DockInfo deserialized = JsonConvert.DeserializeObject<DockInfo>(settingsFile);
                deserialized.Apps.Add(app);
                currentDockInfo = deserialized;
                string newstr = JsonConvert.SerializeObject(deserialized);
                MessageBox.Show(newstr);
                if (char.GetNumericValue(deserialized.Version[2]) > 3)
                {
                    currentError = "Version " + deserialized.Version + " is higher than current version " + currentver + ", aborting";
                    return false;
                }
                MessageBox.Show("beginning for loop (before)");
                foreach (DockApps thisapp in currentDockInfo.Apps.ToArray())
                {
                    MessageBox.Show(thisapp.Name + "\n" + thisapp.Icon + "\n" + thisapp.Path);
                }
                Dock_Refresh();
                MessageBox.Show("beginning for loop (after)");
                foreach (DockApps thisapp in currentDockInfo.Apps.ToArray())
                {
                    MessageBox.Show(thisapp.Name + "\n" + thisapp.Icon + "\n" + thisapp.Path);
                }
                File.WriteAllText(path, newstr);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Oops! An unexpected error occured while reading from the settings file: " + err.Message);
                return false;
            }
        }

        public void LaunchDockApp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(this.Width.ToString());
            Uri uriResult;
            Border bdr = sender as Border;
            bool result = Uri.TryCreate(bdr.Tag.ToString(), UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                // Valid url, OPENING IN BROWSER
#if DEBUG
                MessageBox.Show("This is a url " + bdr.Tag.ToString());
#endif
                var psi = new ProcessStartInfo
                {
                    FileName = bdr.Tag.ToString(),
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show("This is a file");
                Process.Start(bdr.Tag.ToString());
                // Invalid path, lets check if its a file
            }
            return;
        }

        public bool Dock_Refresh(int dockScale = 0)
        {
            if (dockScale > 0)
            {
                MessageBox.Show("Dock Scale is not ready in this current build.. Sorry about that.");
                // tell the user that dock scaling is not supported yet
            }
            else
            {
                dockScale = 3;
            }
            // remove all items from dock
            TheDock.Children.Clear();
            // create 'new item' button
            //MessageBox.Show("Creating bc");
            BrushConverter bc = new();
            //MessageBox.Show("Creating newItmBtn");
            Border newItmBtn = new()
            {
                Style = Resources["DockBtn"] as Style,
                Background = bc.ConvertFromString("#303030") as Brush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 75,
                BorderBrush = null
            };
            // create image
            // MessageBox.Show("Creating newItmBtn_img");
            Image newItmBtn_img = new()
            {
                Height = dockScale * 25,
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/new.png"))
            };
            // add handlers
            // MessageBox.Show("Creating newItmBtn's handlers\nNewItmBtn_img's Source: " + newItmBtn_img.Source.ToString() + " SHOULD NOT BE NULL!!");
            newItmBtn.PreviewMouseUp += new MouseButtonEventHandler(Border_PreviewMouseUp);
            newItmBtn.Child = newItmBtn_img;


            foreach (DockApps app in currentDockInfo.Apps)
            {
                // MessageBox.Show("Creating tempItm");
                // create dock item  
                Border tempItm = new()
                {
                    Style = Resources["DockBtn"] as Style,
                    Background = bc.ConvertFromString("#303030") as Brush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 75,
                    BorderBrush = null,
                    ToolTip = app.Name,
                    Tag = app.Path
                };
                // create image
                // MessageBox.Show("Creating tempItm_Img");
                Image tempItm_Img = new()
                {
                    Height = dockScale * 25,
                    Source = new BitmapImage(new Uri(app.Icon))
                };

                // MessageBox.Show("Creating tempItm's handlers\ntempItm_Img's Source: " + tempItm_Img.Source.ToString() + " SHOULD NOT BE NULL!!");
                tempItm.PreviewMouseUp += new MouseButtonEventHandler(LaunchDockApp);
                tempItm.Child = tempItm_Img;
                //  MessageBox.Show("Adding tempItm");
                TheDock.Children.Add(tempItm);
            }
            // MessageBox.Show("Adding newItmBtn");
            TheDock.Children.Add(newItmBtn);
            return true;
        }

        private void DragWin(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWin(object sender, MouseButtonEventArgs e)
        {
            NewItemModal.Visibility = Visibility.Collapsed;

        }

        private void OKClick(object sender, MouseButtonEventArgs e)
        {
            bool isUri = false;
            string itmName = tbName.Text;
            string itmIcon = tbIcon.Text;
            string itmPath = tbPath.Text;

            DockApps itm = new()
            {
                Name = itmName,
                Icon = itmIcon,
                Path = itmPath
            };
            try
            {
                if (Uri.IsWellFormedUriString(itmPath, UriKind.Absolute))
                {
                    isUri = true;
                }
                else if (!File.Exists(itmPath))
                {
                    // file doesnt exist
                    MessageBox.Show("Invalid file path.. please pick a valid one.");
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Something went wrong trying to check the path..\ndo you have permissions?\n" + err.Message);
                return;
            }
            NewItemModal.Visibility = Visibility.Collapsed;
            if (this.Dock_AddItem(itm))
            {
                int index = currentDockInfo.Apps.Count;
                string tpy = currentDockInfo.Apps[index - 1].Name;
                MessageBox.Show(tpy);
                return;
            }
            else
            {
                if (this.currentError != null)
                {
                    MessageBox.Show("Oops an error occured.. :(\n" + currentError);
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
            catch (Exception err)
            {
                imgIcon.Source = new BitmapImage(new Uri(@"pack://application:,,,/Assets/err.png"));
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Dock_Refresh();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            double calculated = (desktopWorkingArea.Width / 2) - (this.Width / 2);
            // debugging only
            // MessageBox.Show((string.Format("({0} / 2) - ({1} / 2) = {2} ({3})", desktopWorkingArea.Width, this.Width, calculated.ToString(), Math.Round(calculated).ToString())));
            this.Left = Math.Round((desktopWorkingArea.Width / 2) - (this.Width / 2));
            this.Top = (desktopWorkingArea.Height / 2) - (this.Height / 2);
        }
    }
}
