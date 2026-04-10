using System.Windows;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.Services
{
    public class ClipboardService : IClipboardService
    {
        public bool SetText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    Clipboard.SetText(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}