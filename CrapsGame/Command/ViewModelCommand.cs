using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrapsGame.Command
{
    public class ViewModelCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;

        public ViewModelCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return true;
            else
                return canExecute(parameter);
        }
        
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
        
    }
}
