using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFWV2.PDFEngines
{
    internal class EdgeController : PDFEngineController
    {
        internal EdgeController(Stream Stream) 
        { 

        }
        internal EdgeController(string Path)
        {

        }

        internal override void OnWebViewReady()
        {
            throw new NotImplementedException();
        }
    }
}
