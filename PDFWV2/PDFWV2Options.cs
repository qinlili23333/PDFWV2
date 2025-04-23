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
    public enum SecurityLevel
    {
        None,
        Basic,
        Enhanced
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
        /// <summary>
        /// Whether to enable network request isolation.
        /// If enabled, all network requests will be done in main process and convert to memory stream for PDF engine. All network request from PDF engine will be blocked.
        /// This enhances security to prevent remote execution from WebView2, but may impact performance.
        /// Enabled by default and not recommend to disable.
        /// If FallbackToEdge is enabled and fallback happens, this option will automatically enable to prevent remote execution.
        /// </summary>
        public bool NetworkRequestIsolation { get; set; } = true;
        /// <summary>
        /// Security harden level, which indicates how many harden techniques are used.
        /// Higher level will apply more mitigation techniques, but may degrade performance.
        /// Default level is Basic.
        /// </summary>
        public SecurityLevel SecurityHardenLevel { get; set; } = SecurityLevel.Basic;
    }
}
