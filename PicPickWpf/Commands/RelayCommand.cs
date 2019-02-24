using System;
using System.Windows.Input;

namespace PicPick.Commands
{
	public delegate bool PredicateDelegate();

	public class RelayCommand : ICommand // a la Josh Smith
	{
		#region Fields

		readonly Action _execute;
		readonly Predicate<object> _canExecute;

		#endregion // Fields

		#region Ctors

		public RelayCommand(Action execute)
			: this(execute, (Predicate<object>)null)
		{
		}

		public RelayCommand(Action execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action execute, PredicateDelegate canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = (object obj) => { return canExecute(); };
		}

		#endregion // Constructors

		public void Execute()
		{
			_execute();
		}

		#region ICommand Members

		void ICommand.Execute(object parameter)
		{
			_execute();
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
			{
				return _canExecute(parameter);
			}
			return true;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		#endregion // ICommand Members
	}

	public class RelayCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;

		#endregion

		#region Ctors

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		public void Execute(T parameter)
		{
			_execute(parameter);
		}

		#region ICommand Members

		void ICommand.Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
				return _canExecute((T)parameter);
			return true;
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		#endregion
	}
}
