using Aggravation.Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;

namespace Aggravation.Infrastructure.Models
{
    public abstract class ViewModelBase : NotificationObject
    {
        protected IUnityContainer Container { get; set; }
        protected IEventAggregator EventAggregator { get; set; }

        protected ViewModelBase()
        {
        }

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
        }

        protected ViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        protected ViewModelBase(IUnityContainer container, IEventAggregator eventAggregator)
        {
            Container = container;
            EventAggregator = eventAggregator;
        }

        protected void BusyMessage(string message)
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<BusyIndicatorEvent>().Publish(new BusyIndicatorData(true, message));
            }
        }

        protected void BusyMessageOff()
        {
            if (EventAggregator != null)
            {
                EventAggregator.GetEvent<BusyIndicatorEvent>().Publish(new BusyIndicatorData(false));
            }
        }
    }
}
