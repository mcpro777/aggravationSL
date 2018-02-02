using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Practices.Unity;
using Aggravation.Infrastructure.Strings;

namespace Aggravation.GameBoard.SixPlayerModern.Views
{
    public partial class GameBoard : UserControl, Aggravation.GameEngine.IGameBoard
    {
        private IUnityContainer _container = null;

        public GameBoard(IUnityContainer container)
        {
            InitializeComponent();
            this._container = container;
            this.Loaded += new RoutedEventHandler(GameBoard_Loaded);
        }

        protected void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetContainerItemsAfterLoad();
        }

        public void SetContainerItemsAfterLoad()
        {
            var daRoll = this.FindName("dblRoll") as DoubleAnimation;
            var spinAnimation = this.FindName("sbRoll") as Storyboard;

            this._container.RegisterInstance(ContainerItems.GameBoardControl, this, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(ContainerItems.GameBoardLayoutRoot, this.LayoutRoot, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(ContainerItems.GameBoardGameObjects, this.GameObjects, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(ContainerItems.RollAnimation, daRoll, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(ContainerItems.SpinAnimation, spinAnimation, new ContainerControlledLifetimeManager());
        }
    }
}
