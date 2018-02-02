using System.Windows;
using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Aggravation.Infrastructure.Strings;
using Aggravation.Infrastructure.Events;
using Aggravation.Shell.Interfaces;
using Aggravation.GameEngine;
using Aggravation.Game.Ui;
using Aggravation.Infrastructure.Constants;

namespace Aggravation.Shell.Views
{
    public partial class Shell : IShell
    {
        readonly IShellViewModel _vm;
        private IEventAggregator _ea;
        private IUnityContainer _container = null;
        private Aggravation.GameEngine.Game _game = null;

        public Shell(IShellViewModel vm, IUnityContainer container, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this._vm = vm;
            this._ea = eventAggregator;
            this._container = container;
            this.DataContext = this._vm;
            this.Loaded += new RoutedEventHandler(Shell_Loaded);
            eventAggregator.GetEvent<ShowInstructionsEvent>().Subscribe(ShowInstructionsEventHandler, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<ResetEvent>().Subscribe(ResetEventHandler, ThreadOption.UIThread, true);
        }

        protected void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            this.inst.Cancel += new EventHandler<EventArgs>(inst_Cancel);

            this.newGame.Cancel += new EventHandler<EventArgs>(newGameWindow_Cancel);
            this.newGame.Start += new EventHandler<Aggravation.Game.Ui.NewGameWindowStartEventArgs>(newGameWindow_Start);
            this.newGame.HideCancel();
            this.DisableGameControls();
            this.ShowNewGameWindow();
        }

        public void Show()
        {
            Application.Current.RootVisual = this;
        }

        public void ShowInstructionsEventHandler(ShowInstructionsData e)
        {
            this.inst.Visibility = System.Windows.Visibility.Visible;
        }

        public void ResetEventHandler(ResetData e)
        {
            this.newGame.ShowCancel();
            this.DisableGameControls();
            this.ShowNewGameWindow();
        }

        protected void DisableGameControls()
        {
            this._ea.GetEvent<DisableShowInstructionsEvent>().Publish(new DisableShowInstructionsData());
            this._ea.GetEvent<DisableResetEvent>().Publish(new DisableResetData());
        }

        protected void EnableGameControls()
        {
            this._ea.GetEvent<EnableShowInstructionsEvent>().Publish(new EnableShowInstructionsData());
            this._ea.GetEvent<EnableResetEvent>().Publish(new EnableResetData());
        }

        protected void ShowNewGameWindow()
        {
            this.newGame.Visibility = System.Windows.Visibility.Visible;
        }

        protected void HideNewGameWindow()
        {
            this.newGame.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected void inst_Cancel(object sender, EventArgs e)
        {
            this.inst.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void newGameWindow_Cancel(object sender, EventArgs e)
        {
            this.EnableGameControls();
            this.HideNewGameWindow();
        }

        private void newGameWindow_Start(object sender, Aggravation.Game.Ui.NewGameWindowStartEventArgs e)
        {
            this._ea.GetEvent<BusyIndicatorEvent>().Publish(new BusyIndicatorData(true, "Loading Game..."));

            this.HideNewGameWindow();
            this.EnableGameControls();

            UserControl gameBoard = this._container.Resolve<UserControl>(ContainerItems.GameBoardControl);
            Canvas layoutRoot = this._container.Resolve<Canvas>(ContainerItems.GameBoardLayoutRoot);
            Canvas gameObjects = this._container.Resolve<Canvas>(ContainerItems.GameBoardGameObjects);
            StackPanel playerPanel = this._container.Resolve<StackPanel>(ContainerItems.PlayerGamePanel);
            Grid statusPanel = this._container.Resolve<Grid>(ContainerItems.StatusPanel);
            DoubleAnimation rollAnimation = this._container.Resolve<DoubleAnimation>(ContainerItems.RollAnimation);
            Storyboard spinAnimation = this._container.Resolve<Storyboard>(ContainerItems.SpinAnimation);

            this._game = new GameEngine.Game(this.grdLayoutRoot, gameBoard, layoutRoot, gameObjects, spinAnimation, rollAnimation, playerPanel, this._ea, Constants.CardWidth, Constants.CardHeight);

            Boolean foundStart = false;
            Players players = new Players();
            foreach (var item in e.PlayerPrompts)
            {
                if (item.IsActivated)
                {
                    Player player = null;
                    if (item.PlayerType == PlayerType.Human)
                    {
                        player = new Player(item.PlayerName, item.PlayerNumber);
                        player.Gender = Gender.Male;
                    }
                    else
                    {
                        GameDifficulty difficulty = GameDifficulty.Easy;
                        if (item.ComputerDificulty == ComputerDificulty.Easy) difficulty = GameDifficulty.Easy;
                        if (item.ComputerDificulty == ComputerDificulty.Medium) difficulty = GameDifficulty.Medium;
                        if (item.ComputerDificulty == ComputerDificulty.Hard) difficulty = GameDifficulty.Hard;
                        player = new ComputerPlayer(item.PlayerName, item.PlayerNumber, difficulty);
                        player.Gender = Gender.NoSex;
                    }

                    if (e.StartPlayerNum == player.Number)
                    {
                        player.StartsFirst = true;
                        foundStart = true;
                    }
                    else
                    {
                        player.StartsFirst = false;
                    }

                    players.Add(player);
                }
            }

            if (!foundStart) players[0].StartsFirst = true;
            players.Sort(new PlayerComparer());
            this._game.Start(players);

            this._ea.GetEvent<BusyIndicatorEvent>().Publish(new BusyIndicatorData(false));
        }
    }
}
