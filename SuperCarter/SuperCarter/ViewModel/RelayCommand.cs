using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SuperCarter.ViewModel
{
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return _canExecute == null ? true : _canExecute(parameters);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameters)
        {
            _execute(parameters);
        }

        #endregion // ICommand Members
    }

    public class RelayCommand<T> : ICommand
    {
        #region Declarations

        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }


        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(Object parameter)
        {
            _execute((T)parameter);
        }

        #endregion ICommand Members
    }

    public class DelegateCommand : ICommand
    {
        Predicate<object> canExecute;
        Action<object> execute;
        public DelegateCommand(Predicate<object> _canexecute, Action<object> _execute)
       : this()
        {
            canExecute = _canexecute;
            execute = _execute;
        }
        public DelegateCommand()
        { }
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
