# Engines
## Edge PDF
Value: `edge`  
Microsoft Edge PDF engine in WebView2, best compatibility and good performance in most circumstances. Very basic features.  
Updates through Microsoft Edge Update so should always on latest version ready to use.
Does not require any additional download size, which is the default engine if fallback is enabled and primary engine fails to load.  

> [!WARNING]  
> Disabling `NetworkRequestIsolation` WILL allow non-PDF file to be displayed. DO NOT disable it unless you DO need it for hacky tricks.  

## PDF.js
Value: `pdfjs`  
Mozilla [PDF.js](https://mozilla.github.io/pdf.js/), good compatibility and moderate performance. Supports various enhanced features like highlight and draw.  
Updates regularly.  
Requires approx 6MB download size.  
## Adobe PDF
Value: `adobe`
Adobe PDF engine, close sourced but with good compatibility and moderate performance. Supports various enhanced features like highlight and draw.  
Dynamically loaded from Adobe servers, so no additional download size is required.  

> [!WARNING]  
> Adobe collectes usage data. By default the built-in key will send usage data to Qinlili Tech. Use your own key to collect by yourself.  
> Get key at [Adobe Embedded PDF Service](https://developer.adobe.com/document-services/apis/pdf-embed/).  
> This engine may not work in some regions due to network restrictions and Adobe policy.  

## MuPDF.js (Removed from plan)
Value: `mupdf`
Written in C and compiled to WebAssembly, [MuPDF](http://www.mupdf.com/) is extremely fast with good compatibility. Memory usage may be a bit high.  
Irregular updates as currently in experiment.  
Requires approx 4MB download size.  
Due to I cannot find a way to  make this project in GPLv3 compatible while allow using in close source projects, currently I have dropped the plan. May add in future if I can find a solution for license issue.  