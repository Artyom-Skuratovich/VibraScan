using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibraScan.WinForms.Views.Abstractions
{
    public interface IDataImportView : IView
    {
        event EventHandler CancelRequested;
    }
}