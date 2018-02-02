using System;
using Aggravation.Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Aggravation.Infrastructure.Models
{
    public class ModuleBase : IModule
    {
        internal IEventAggregator _eventAggregator;
        private IUnityContainer _container;

        public ModuleInfo ModuleInfo { get; set; }

        protected IUnityContainer Container
        {
            get { return _container ?? (_container = (IUnityContainer)ServiceLocator.Current.GetService(typeof(IUnityContainer))); }
            private set { _container = value; }
        }

        protected IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = (IEventAggregator)ServiceLocator.Current.GetService(typeof(IEventAggregator))); }
        }

        public ModuleBase()
        {

        }

        public ModuleBase(IUnityContainer container)
        {
            this.Container = container;
        }

        public virtual void Initialize()
        {
            this.ModuleInfo = new ModuleInfo();
            PublishModuleLoadedEvent();
        }

        public virtual void PublishModuleLoadedEvent()
        {
            EventAggregator.GetEvent<ModuleLoadedEvent>().Publish(ModuleInfo);
        }
    }
}
