using System.Windows.Input;

namespace MiniDashboard.App.Infrastructure
{
    public class RelayCommand<T> : ICommand 
    { 
        private readonly Action<T> m_execute; 

        private readonly Func<T, bool>? m_canExecute; 

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null) 
        { 
            m_execute = execute; 
            m_canExecute = canExecute; 
        } 
        
        public bool CanExecute(object? parameter) => m_canExecute?.Invoke((T)parameter!) ?? true; 
        
        public void Execute(object? parameter) => m_execute((T)parameter!); 
        
        public event EventHandler? CanExecuteChanged 
        {
            add => CommandManager.RequerySuggested += value; 
            remove => CommandManager.RequerySuggested -= value; 
        } 
    }

    public class RelayCommand : RelayCommand<object?>
    {
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : base(
                _ => execute(),
                _ => canExecute?.Invoke() ?? true
            )
        {
        }
    }
}
