using System;
using System.Windows.Input;

namespace LibgenDesktop.Infrastructure
{
    public class FuncCommand<TParameter, TResult> : ICommand
    {
        private readonly Func<TParameter, TResult> executeFunction;

        public FuncCommand(Func<TParameter, TResult> executeFunction)
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
            switch (parameter)
            {
                case TParameter typedParameter:
                    ExecuteWithTypedParameter(typedParameter);
                    break;
                default:
                    ExecuteWithTypedParameter(default);
                    break;
            }
        }

        public TResult ExecuteWithTypedParameter(TParameter parameter)
        {
            TResult result = executeFunction != null ? executeFunction(parameter) : default;
            OnCanExecuteChanged();
            return result;
        }

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
