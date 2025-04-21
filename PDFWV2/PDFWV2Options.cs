namespace PDFWV2
{
    public enum UpdateMode
    {
        Never,
        Background,
        Foreground
    }
    public enum Engines
    {
        PDFJS
    }
    public class PDFWV2Options
    {
        //Module folder
        public string ModuleFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PDFWV2";
        //Target PDF Engine, currently 'pdfjs' is the only acceptable, more engines in future updates
        public Engines Engine { get; set; } = Engines.PDFJS;
        //Enable F12 DebugTool
        public bool DebugTool { get; set; } = false;
        //Update mode
        public UpdateMode EnableUpdate { get; set; } = UpdateMode.Background;
    }
}
