using System;
using System.Windows.Input;

namespace LibgenDesktop.Infrastructure
{
    internal class Command : ICommand
    {
        private readonly Predicate<object> canExecute = null;
        private readonly Action<Object> executeAction = null;

        public Command(Action executeAction)
            : this(param => true, param => executeAction())
        {
        }

        public Command(Action<object> executeAction)
            : this(param => true, executeAction)
        {
        }

        public Command(Predicate<object> canExecute, Action<object> executeAction)
        {
            this.canExecute = canExecute;
            this.executeAction = executeAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            executeAction?.Invoke(parameter);
            UpdateCanExecuteState();
        }

        public void UpdateCanExecuteState()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
