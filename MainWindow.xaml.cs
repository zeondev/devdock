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

namespace DevDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            double left = desktopWorkingArea.Right - this.Width;
            double top = desktopWorkingArea.Bottom - this.Height;
            MessageBox.Show("First Messagebox\ndesktopWorkingArea.Right = " + desktopWorkingArea.Right.ToString() + "\nthis.Width = " + this.Width + "\n" + desktopWorkingArea.Right.ToString() + "-" + this.Width + "=" + left.ToString() + "!!");
            MessageBox.Show("Second Messagebox\ndesktopWorkingArea.Bottom = " + desktopWorkingArea.Bottom.ToString() + "\nthis.Height = " + this.Height + "\n" + desktopWorkingArea.Bottom.ToString() + "-" + this.Height + "=" + top.ToString() + "!!");
            this.Left = 1212;
            this.Top = top;

            // CENTERED TOP: 673
            // CENTERED LEFT: 1212
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
