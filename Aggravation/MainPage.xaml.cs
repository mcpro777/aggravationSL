using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Aggravation.GameEngine;
using System.Windows.Interactivity;

namespace Aggravation
{
    public partial class MainPage : UserControl
    {
        Game game = null;
        NewGameWindow newGameWindow = null;
        Instructions instructions = null;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ////////this.btnReset.Click += new RoutedEventHandler(btnReset_Click);
            ////////this.btnInstructions.Click += new RoutedEventHandler(btnInstructions_Click);
            ///////////////////////////////////////////////////////////////////this.game = new GameEngine.Game(this, this.LayoutRoot, this.GameObjects, this.gamePanels, this.statusPanel, 60.0, 89.0);
            ////////this.game = new GameEngine.Game(this, this.LayoutRoot, this.GameObjects, this.gamePanels, null, 60.0, 89.0);
            ////////this.btnInstructions.IsEnabled = false;
            ////////this.btnReset.IsEnabled = false;

            this.instructions = new Instructions(this.LayoutRoot);

            this.newGameWindow = new NewGameWindow(this.LayoutRoot);
            this.newGameWindow.Cancel += new EventHandler<EventArgs>(newGameWindow_Cancel);
            this.newGameWindow.Start += new EventHandler<NewGameWindowStartEventArgs>(newGameWindow_Start);
            this.newGameWindow.Show(true);
        }

        private void newGameWindow_Cancel(object sender, EventArgs e)
        {
        }

        private void newGameWindow_Start(object sender, NewGameWindowStartEventArgs e)
        {
            //////////this.btnInstructions.IsEnabled = true;
            //////////this.btnReset.IsEnabled = true;

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
            game.Start(players);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.newGameWindow.Show(false);
        }

        private void btnInstructions_Click(object sender, RoutedEventArgs e)
        {
            this.instructions.Show();    
        }
    }
}
