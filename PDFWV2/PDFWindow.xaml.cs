using System.Windows;

namespace PDFWV2
{
    /// <summary>
    /// Interaction logic for PDFWindow.xaml
    /// </summary>
    public partial class PDFWindow : Window
    {
        public PDFWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFWV2.AddWindow(this);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PDFWV2.RemoveWindow(this);
        }
    }
}
