using MiniDashboard.App.EntityModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniDashboard.App.ViewModels
{
    public abstract class VmBase: INotifyPropertyChanged
    {
        public EmStatusMessage StatusMessage { get; }

        protected VmBase()
        {
            StatusMessage = new EmStatusMessage
            {
                Type = StatusMessageType.None,
                Message = string.Empty
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;

            OnPropertyChanged(propertyName);
            
            return true;
        }

        private bool m_isBusy = false;
        public bool IsBusy
        {
            get => m_isBusy;
            set => SetProperty(ref m_isBusy, value);
        }

        public void Initialize()
        {
            _ = InitializeAsync().ContinueWith(t =>
            {
                ShowStatusMessage(StatusMessageType.Error, "Initialization failed.");
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        protected void ShowStatusMessage(StatusMessageType type, string message)
        {
            StatusMessage.Type = type;
            StatusMessage.Message = message;

            OnPropertyChanged(nameof(StatusMessage));
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
