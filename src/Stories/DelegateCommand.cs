namespace Stories
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class DelegateCommand<T> : ICommand
        where T : class
    {
        private readonly Predicate<T> canExecute;

        private readonly Action<T> execute;

        public DelegateCommand(Action<T> execute) => this.execute = execute;

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => this.canExecute?.Invoke(parameter as T) ?? true;

        public void Execute(object parameter) => this.execute(parameter as T);
    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;

        private readonly Action execute;

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
            this.canExecute = () => true;
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => this.canExecute();

        public void Execute(object parameter) => this.execute();
    }

    public class AsyncCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> execute) : this(execute, () => true)
        {
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }


        public async void Execute(object parameter)
        {
            this._isExecuting = true;
            try
            {
                await this._execute();
            }
            finally
            {
                this._isExecuting = false;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => !this._isExecuting && (this._canExecute?.Invoke() ?? false);
    }
}