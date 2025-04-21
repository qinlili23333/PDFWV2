using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFWV2
{
    public enum UpdateMode
    {
        Never,
        Background,
        Foreground
    }
    public class PDFWV2Options
    {
        //Module folder
        public string ModuleFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PDFWV2";
        //Target PDF Engine, currently 'pdfjs' is the only acceptable, more engines in future updates
        public string Engine { get; set; } = "pdfjs";
        //Enable F12 DebugTool
        public bool DebugTool { get; set; } = false;
        //Update mode
        public UpdateMode EnableUpdate { get; set; } = UpdateMode.Background;
    }
}
