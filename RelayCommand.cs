// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="MarkJC">
//   Author Sacha Barber
// </copyright>
// <summary>
//   A command whose sole purpose is to relay its functionality to other
//   objects by invoking delegates. The default return value for the
//   CanExecute method is 'true'.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace CeoSeoViewModels
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the
    /// CanExecute method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The can execute.
        /// </summary>
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// The _execute.
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class. 
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">
        /// The execution logic.
        /// </param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class. 
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">
        /// The execution logic.
        /// </param>
        /// <param name="canExecute">
        /// The execution status logic.
        /// </param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return canExecute == null || canExecute(parameters);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Execute(object parameters)
        {
            execute(parameters);
        }
    }
}
