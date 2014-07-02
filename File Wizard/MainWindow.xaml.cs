using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace File_Wizard
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
        

        /// <summary>
        /// Code behind for open folder dialog box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFolderBrowserDialog(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fd = new FolderBrowserDialog())
            {
                DialogResult result = fd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.folderPath.Text = fd.SelectedPath.ToString();
                }
            }
        }

        /// <summary>
        /// Passing multiple selected rows to view model, either this or 
        /// giving up on virtualized panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList files = this.myGrid.SelectedItems;
            ((MainWindowViewModel)this.DataContext).SelectedFiles = files;
        }
    }
}
