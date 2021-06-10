#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Edimsha.Core.Logging.Implementation;
using Edimsha.WPF.Annotations;

namespace Edimsha.WPF.ViewModels
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Dispose()
        {
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            // Faster image procesing
            if (!propertyName.Equals("StatusBar2") || !propertyName.Equals("StatusBar"))
                Logger.Log($"PropertyName: {propertyName}");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}