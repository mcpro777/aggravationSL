using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Aggravation.Infrastructure.Strings;
using Aggravation.Infrastructure.Events;

namespace Aggravation.Game.Status.Views
{
    public partial class StatusPanel : UserControl
    {
        private IUnityContainer _container = null;

        public StatusPanel(IUnityContainer container, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this._container = container;
            this.Loaded += new RoutedEventHandler(StatusPanel_Loaded);
            eventAggregator.GetEvent<GameStatusEvent>().Subscribe(GameStatusEventHandler, ThreadOption.UIThread, true);
        }

        protected void StatusPanel_Loaded(object sender, RoutedEventArgs e)
        {
            this._container.RegisterInstance(ContainerItems.StatusPanel, this.LayoutRoot, new ContainerControlledLifetimeManager());
        }

        protected void GameStatusEventHandler(GameStatusData e)
        {
            if (e.Message == null)
            {
                this.tbStatus.Text = "";
                return;
            }

            this.tbStatus.Text = e.Message + this.tbStatus.Text;
        }
    }
}
