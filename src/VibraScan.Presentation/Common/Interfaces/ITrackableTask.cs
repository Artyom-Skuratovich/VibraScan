using System.ComponentModel;

namespace VibraScan.Presentation.Common.Interfaces
{
    public interface ITrackableTask : INotifyPropertyChanged
    {
        bool IsCompleted { get; }
    }
}