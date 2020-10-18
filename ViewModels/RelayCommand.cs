using System;
using System.Windows.Input;

namespace ImageTagger.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action     _executeAction;
        private readonly Func<bool> _canExecuteFunction;

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public RelayCommand(Action executeAction, Func<bool> canExecuteFunction = null)
        {
            _executeAction      = executeAction;
            _canExecuteFunction = canExecuteFunction;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunction?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _executeAction();
        }
    }
}
