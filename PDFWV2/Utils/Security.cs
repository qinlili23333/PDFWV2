using Microsoft.Win32;

namespace PDFWV2.Utils
{
    /// <summary>
    /// A static util class for some security harden methods
    /// </summary>
    internal static class Security
    {
        /// <summary>
        /// Call all enhanced security harden launch time checks
        /// </summary>
        internal static void EnhancedHarden()
        {
            RegistryCheck();
            VariableCheck();
        }

        /// <summary>
        /// Check Registry value about WebView2 debugging
        /// </summary>
        internal static void RegistryCheck()
        {
            try
            {
                if ((string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge\\WebView2\\AdditionalBrowserArguments", System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, string.Empty) != string.Empty)
                {
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge\\WebView2\\AdditionalBrowserArguments", System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, string.Empty);
                }
            }
            catch (Exception)
            {

            }
            try
            {
                if ((string?)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge\\WebView2\\AdditionalBrowserArguments", "*", string.Empty) != string.Empty)
                {
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge\\WebView2\\AdditionalBrowserArguments", "*", string.Empty);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Check environment variable about WebView2 debugging
        /// </summary>
        internal static void VariableCheck()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", null);
            Environment.SetEnvironmentVariable("WEBVIEW2_PIPE_FOR_SCRIPT_DEBUGGER", null);
        }
    }
}
