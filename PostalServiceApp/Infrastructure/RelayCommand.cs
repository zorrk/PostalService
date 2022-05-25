using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PostalServiceApp.Infrastructure;

public class RelayCommand : ICommand
{

	private readonly Action<object> _executionAction;
	private readonly Predicate<object> _canExecutePredicate;

	public RelayCommand(Action<object> execute)
		: this(execute, null)
	{
	}

	public RelayCommand(Action<object> execute, Predicate<object> canExecute)
	{
		_executionAction = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecutePredicate = canExecute;
	}

	public event EventHandler CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	public bool CanExecute(object parameter) =>
		_canExecutePredicate == null || _canExecutePredicate(parameter);

	public void Execute(object parameter) => _executionAction(parameter);
}