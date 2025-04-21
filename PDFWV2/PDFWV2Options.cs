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
        EDGE,
        PDFJS
    }
    public class PDFWV2Options
    {
        /// <summary>
        /// Module folder, the location where PDFWV2 stores data, shared across applications by default, but you can set to another folder if you really need fully isolation.
        /// </summary>
        public string ModuleFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PDFWV2";
        /// <summary>
        /// Target PDF Engine, currently 'pdfjs' (default) and 'edge' are the only acceptable, more engines in future updates
        /// </summary>
        public Engines Engine { get; set; } = Engines.PDFJS;
        /// <summary>
        /// Enable F12 DebugTool, disabled by default
        /// </summary>
        public bool DebugTool { get; set; } = false;
        /// <summary>
        /// Update mode, background update by default
        /// </summary>
        public UpdateMode EnableUpdate { get; set; } = UpdateMode.Background;
        /// <summary>
        /// Whether to fallback to Edge PDF engine when another render cannot be initialized, enabled by default.
        /// Has no impact if Edge is already the selected engine
        /// </summary>
        public bool FallbackToEdge { get; set; } = true;
    }
}
