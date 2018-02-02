using System;
using Microsoft.Practices.Prism.Commands;

namespace Aggravation.Shell.Interfaces
{
    public interface IShellViewModel
    {
        DelegateCommand<object> ExitCommand { get; }
    }
}
