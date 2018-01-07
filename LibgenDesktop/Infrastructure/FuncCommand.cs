using System;
using System.Windows.Input;

namespace LibgenDesktop.Infrastructure
{
    public class FuncCommand<TResult> : ICommand
    {
        private readonly Func<TResult> executeFunction;

        public FuncCommand(Func<TResult> executeFunction)
        {
            this.executeFunction = executeFunction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Execute();
        }

        public TResult Execute()
        {
            TResult result = executeFunction != null ? executeFunction() : default(TResult);
            OnCanExecuteChanged();
            return result;
        }

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
