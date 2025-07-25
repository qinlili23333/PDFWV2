﻿using Microsoft.Web.WebView2.Core;

namespace PDFWV2
{
    /// <summary>
    /// Internal instance manager stores all static variables need in same process.
    /// One process can only initialize one PDFWV2 instance.
    /// </summary>
    internal static class PDFWV2InstanceManager
    {
        /// <summary>
        /// PDFWV2 instance in process.
        /// </summary>
        internal static PDFWV2Instance? Instance;
        /// <summary>
        /// Options for instance.
        /// </summary>
        internal static PDFWV2Options Options = new();
        /// <summary>
        /// WebView2 environment for whole process.
        /// </summary>
        internal static CoreWebView2Environment? WebView2Environment;
        /// <summary>
        /// The hidden WebView2 controller to keep WebView2 environment always ready.
        /// Fucking Microsoft doesn't provide keep alive feature and just destroys environment after all pages closed.
        /// </summary>
        internal static CoreWebView2Controller? AliveController;
        /// <summary>
        /// Stores created engines
        /// </summary>
        internal static Dictionary<Engines, PDFEngine> ActiveEngines = [];
        /// <summary>
        /// A list of active documents.
        /// Instance can only be destroyed with no active document.
        /// </summary>
        internal static List<PDFWindow> ActiveDocuments = [];
    }
}
