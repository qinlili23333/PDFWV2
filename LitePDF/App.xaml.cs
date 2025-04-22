using System.Windows;

namespace LitePDF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent(params string[] Args)
        {
            if (Args.Length > 1)
            {
                //Directly open file
                //TODO
            }
            else
            {
                this.StartupUri = new System.Uri("OpenFile.xaml", System.UriKind.Relative);
            }
        }
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        static void Main(params string[] Args)
        {
            App app = new App();
            app.InitializeComponent(Args);
            app.Run();
        }
    }

}
