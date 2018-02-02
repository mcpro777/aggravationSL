using System;
using System.Collections.Generic;
using Aggravation.Infrastructure.Events;
using Aggravation.Infrastructure.Models;
using Aggravation.Infrastructure.Strings;
using Aggravation.Shell.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Aggravation.Shell.ViewModels
{
    public class ShellViewModel : ViewModelBase, IShellViewModel
    {
        readonly IDictionary<String, String> _initParams;

        public DelegateCommand<object> ExitCommand { get; private set; }

        public ShellViewModel(IUnityContainer container, IEventAggregator eventAggregator)
            : base(container, eventAggregator)
        {
            this._initParams = Container.Resolve<IDictionary<String, String>>(ContainerItems.InitParams);

            EventAggregator.GetEvent<ModuleChangedEvent>().Subscribe(ModuleChangedEventHandler, ThreadOption.UIThread, true);
            EventAggregator.GetEvent<BusyIndicatorEvent>().Subscribe(BusyIndicatorEventHandler, ThreadOption.UIThread, true);

            this.ExitCommand = new DelegateCommand<object>(ExecuteExit, CanExit);
        }

        private Boolean _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        private String _busyDisplayMessage;
        public String BusyDisplayMessage
        {
            get { return _busyDisplayMessage; }
            set
            {
                _busyDisplayMessage = value;
                RaisePropertyChanged(() => BusyDisplayMessage);
            }
        }

        public void ModuleChangedEventHandler(ModuleInfo e)
        {

        }

        public void BusyIndicatorEventHandler(BusyIndicatorData e)
        {
            BusyDisplayMessage = e.IsBusy ? e.Message : String.Empty;
            this.IsBusy = e.IsBusy;
        }

        private Boolean CanExit(object item)
        {
            return true;
        }

        private void ExecuteExit(object item)
        {

        }
    }
}
