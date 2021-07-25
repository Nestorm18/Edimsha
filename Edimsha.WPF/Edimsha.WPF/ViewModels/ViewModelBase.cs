#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Edimsha.WPF.Annotations;

namespace Edimsha.WPF.ViewModels
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;

    public class ViewModelBase : INotifyPropertyChanged
    {
        // Log
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion

        public static void Dispose()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Faster image procesing
            if (!propertyName.Contains("StatusBar") || !propertyName.Contains("PbPosition")) return;

            Logger.Info($"PropertyName: {propertyName}");
        }
    }
}