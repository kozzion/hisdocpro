using HisDocProUI.Model;
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

namespace HisDocProUI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMain : Window
    {
        public ViewMain()
        {

            this.DataContext = new ModelMain();
            InitializeComponent();
        }

        private void DropCall(object sender, DragEventArgs e)
        {
            ModelMain modelMain = (ModelMain)DataContext;
            // Note that you can have more than one file.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length == 1)
            {
                if (files[0].Substring(files[0].Length - 3, 3).ToLower().Equals("pdf"))
                {
                    modelMain.LoadPDF(files[0]);
                }
            }
        }
    }
}
