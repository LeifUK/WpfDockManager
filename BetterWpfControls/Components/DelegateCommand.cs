using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BetterWpfControls.Components
{
    public class DelegateCommand<T> : ICommand
        where T : class
    {
        #region .ctors

        public DelegateCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion .ctors

        #region Fields

        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        #endregion Fields

        #region Methods

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter as T);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter as T);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        #endregion Methods
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        #region .ctors

        public DelegateCommand(Action execute)
            : base((o) => execute())
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base((o) => execute(), (o) => canExecute())
        {
        }

        #endregion .ctors
    }
}
