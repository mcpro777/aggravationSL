using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Aggravation.Infrastructure.Strings;

namespace Aggravation.Game.PlayerPanels.Views
{
    public partial class PlayerPanels : UserControl
    {
        private IUnityContainer _container = null;

        public PlayerPanels(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            this.Loaded += new RoutedEventHandler(PlayerPanels_Loaded);
        }

        protected void PlayerPanels_Loaded(object sender, RoutedEventArgs e)
        {
            this._container.RegisterInstance(ContainerItems.PlayerGamePanel, this.LayoutRoot, new ContainerControlledLifetimeManager());
        }
    }
}
