# Engines
## Edge PDF
Value: `edge`
Microsoft Edge PDF engine in WebView2, best compatibility and good performance in most circumstances. Very basic features.  
Updates through Microsoft Edge Update so should always on latest version ready to use.
Does not require any additional download size, which is the default engine if fallback is enabled and primary engine fails to load.  
> [!WARN]  
> Disabling `NetworkRequestIsolation` WILL allow non-PDF file to be displayed. DO NOT disable it unless you DO need it for hacky tricks.  
## PDF.js
Value: `pdfjs`  
Mozilla [PDF.js](https://mozilla.github.io/pdf.js/), good compatibility and moderate performance. Supports various enhanced features like highlight and draw.  
Updates regularly.  
Requires approx 6MB download size.  
## MuPDF.js (Planned in future)
Value: `mupdf`
Written in C and compiled to WebAssembly, [MuPDF](http://www.mupdf.com/) is extremely fast with good compatibility. Memory usage may be a bit high.  
Irregular updates as currently in experiment.  
Requires approx 4MB download size.