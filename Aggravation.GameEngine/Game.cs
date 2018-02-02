/*
Copyright (c) 2012 Jason McCoy

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Aggravation.GameEngine.Interfaces;
using Aggravation.GameEngine.UserControls;
using Aggravation.Infrastructure.Constants;
using Aggravation.Infrastructure.Events;
using DeckOfCards;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.GameEngine
{
    public class Game : IGame
    {
        //TODO: Should the game allow moving past your own player during fast track moves?
        //BUG:  Reset game bug: Track or piece is not reset after game finishes
        //BUG:  You can choose fast track location and circle back around the board

        private Boolean resetting = false;
        private Storyboard spinAnimation = null;
        private UserControl mainControl = null;
        private Canvas gameObjects;
        private Panel layoutRoot;
        private Panel playerGamePanel;
        private Grid shellGrid;
        private PlayerPiece activePiece;
        private Queue<BoardMoveLocation> currentTurnPath = new Queue<BoardMoveLocation>();
        private List<Storyboard> highlightStoryboards = new List<Storyboard>();
        private Dictionary<Player, DateTime> playersFinished = new Dictionary<Player, DateTime>();
        private Dictionary<FrameworkElement, BoardMoveRoute> highlightOverlays = new Dictionary<FrameworkElement, BoardMoveRoute>();
        private Dictionary<FrameworkElement, BoardMoveRoute> rankOverlays = new Dictionary<FrameworkElement, BoardMoveRoute>();
        private DoubleAnimation daRoll = null;
        private Boolean showTips = false;
        private Double? cardWidth = null;
        private Double? cardHeight = null;
        private GameAi gameAi = null;
        private IEventAggregator eventAggregator;

        public Dictionary<Int32, Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }

        public event EventHandler<LandedOnPlayerEventArgs> LandedOnPlayer;
        private void RaiseLandedOnPlayer(PlayerPiece newPlayer, PlayerPiece existingPlayer)
        {
            if (LandedOnPlayer != null)
            {
                LandedOnPlayer.Invoke(this, new LandedOnPlayerEventArgs(newPlayer, existingPlayer));
            }
        }

        public event EventHandler<TurnProcessedEventArgs> TurnProcessed;
        private void RaiseTurnProcessed(Player player, PlayerPiece piece)
        {
            if (TurnProcessed!= null)
            {
                TurnProcessed.Invoke(this, new TurnProcessedEventArgs(player, piece));
            }
        }

        public event EventHandler<PlayerFinishedEventArgs> PlayerFinished;
        private void RaisePlayerFinished(Player player)
        {
            if (PlayerFinished != null)
            {
                PlayerFinished.Invoke(this, new PlayerFinishedEventArgs(player));
            }
        }

        public event EventHandler<EventArgs> GameFinished;
        private void RaiseGameFinished()
        {
            if (GameFinished != null)
            {
                GameFinished.Invoke(this, new EventArgs());
            }
        }

        public Game(Grid shellGrid, UserControl main, Panel layoutRoot, Canvas gameObjects, Storyboard spinAnimation, DoubleAnimation rollAnimation, Panel playerGamePanel, IEventAggregator eventAggregator, Double? cardWidth, Double? cardHeight)
        {
            this.shellGrid = shellGrid;
            this.mainControl = main;
            this.gameObjects = gameObjects;
            this.playerGamePanel = playerGamePanel;
            this.eventAggregator = eventAggregator;
            this.layoutRoot = layoutRoot;
            this.cardWidth = cardWidth;
            this.cardHeight = cardHeight;
            this.Players = new Dictionary<Int32, Player>();
            this.daRoll = rollAnimation;
            this.spinAnimation = spinAnimation;
            this.CurrentPlayer = null;

            this.LandedOnPlayer += new EventHandler<LandedOnPlayerEventArgs>(Game_LandedOnPlayer);
            this.TurnProcessed += new EventHandler<TurnProcessedEventArgs>(Game_TurnProcessed);
            this.PlayerFinished += new EventHandler<PlayerFinishedEventArgs>(Game_PlayerFinished);
            this.GameFinished += new EventHandler<EventArgs>(Game_GameFinished);
            this.layoutRoot.KeyDown += new KeyEventHandler(mainControl_KeyDown);

            foreach (var piece in BoardManager.Pieces)
            {
                piece.Value.VisualElement.MouseEnter += new MouseEventHandler(Piece_MouseEnter);
                piece.Value.VisualElement.MouseLeave += new MouseEventHandler(Piece_MouseLeave);
                piece.Value.VisualElement.MouseLeftButtonUp += new MouseButtonEventHandler(Piece_MouseLeftButtonUp);
            }
        }

        public void Start(Players players)
        {
            this.resetting = true;
            this.ResetGameBoard();
            this.resetting = false;

            //Setup players and assign pieces
            foreach (var player in players)
            {
                if (player is ComputerPlayer)
                {
                    if (this.gameAi == null)
                    {
                        this.gameAi = new GameAi(this);
                    }
                }

                player.CurrentDeck = new Deck52Plus(false, true, null, this.cardWidth, this.cardHeight);
                player.UsedDeck = new Deck52Plus(false, true, null, this.cardWidth, this.cardHeight);
                player.IsFinished = false;
                BoardManager.SetPlayerPieces(player);
                this.Players.Add(player.Number, player);

                PlayerGamePanel panel = new PlayerGamePanel();
                panel.Player = player;
                panel.SetToken(BoardManager.GetPlayerPieces(player).First());
                panel.Loaded += new RoutedEventHandler(panel_Loaded);
                panel.PlayerDraws += new EventHandler<PlayerDrawsEventArgs>(GamePanel_PlayerDraws);
                panel.PlayerDrawCompleted += new EventHandler<PlayerDrawCompletedEventArgs>(GamePanel_PlayerDrawCompleted);
                this.playerGamePanel.Children.Add(panel);
                player.GamePanel = panel;
            }
        }

        private void panel_Loaded(object sender, RoutedEventArgs e)
        {
            PlayerGamePanel panel = (PlayerGamePanel)sender;
            panel.Initialize();
            if (panel.Player.StartsFirst)
            {
                this.CurrentPlayer = panel.Player;
                this.ActivateCurrentPlayer();
            }
        }

        protected void ResetGameBoard()
        {
            foreach (var item in this.playerGamePanel.Children)
            {
                if (item is PlayerGamePanel)
                {
                    PlayerGamePanel panel = (PlayerGamePanel)item;
                    panel.Loaded -= panel_Loaded;
                    panel.PlayerDrawCompleted -= GamePanel_PlayerDrawCompleted;
                    panel.PlayerDraws -= GamePanel_PlayerDraws;
                }
            }

            this.CurrentPlayer = null;
            this.playerGamePanel.Children.Clear();
            this.playersFinished.Clear();
            this.Players.Clear();

            this.eventAggregator.GetEvent<GameStatusEvent>().Publish(new GameStatusData(null));
            this.activePiece = null;
            this.currentTurnPath.Clear();
            this.ClearAllHighlights();
            
            foreach (var piece in BoardManager.Pieces)
            {
                this.TakePieceOffFasttrack(piece.Value);
                this.MoveToBase(piece.Value);
            }
        }

        public void MoveToBase(PlayerPiece piece)
        {
            if (piece.CurrentLocation == null || piece.CurrentLocation.LocationType != LocationType.Base)
            {
                foreach (var location in BoardManager.GetPlayerBaseLocations(piece.PlayerNumber))
                {
                    if (!location.Occupied)
                    {
                        this.MoveToLocation(piece, location, false, false);
                        break;
                    }
                }
            }
        }

        public void HighlightRoutes(BoardMoveRoutes routes)
        {
            foreach (var route in routes)
            {
                this.HighlightRoute(route);
            }

            if (this.showTips)
            {
                Dictionary<PlayerPiece, BoardMoveRoutes> allRoutes = this.gameAi.GetRoutesAllPieces(this, this.CurrentPlayer);
                GameAi.RankedRoutes rankedRoutes = this.gameAi.GetRankedRoutes(allRoutes, this.CurrentPlayer);
                foreach (var item in rankedRoutes)
                {
                    foreach (var location in item.Route)
                    {
                        this.ShowRouteRank(location, true, item.Route, item.Rank);
                    }
                }
            }
        }

        public void HighlightRoute(BoardMoveRoute route)
        {
            foreach (var location in route)
            {
                if (route[route.Count - 1] == location)     //Last move in route
                    this.HighlightLocation(location, true, route);
                else
                    this.HighlightLocation(location, false, route);
            }
        }

        public void ShowRouteRank(BoardMoveLocation location, Boolean useOverlay, BoardMoveRoute route, Int32 rank)
        {
            TextBlock rankOverlay = new TextBlock();
            rankOverlay.Text = rank.ToString();
            rankOverlay.FontSize = 9.0;
            SolidColorBrush brush = new SolidColorBrush(Colors.White);
            rankOverlay.Foreground = brush;
            rankOverlay.Width = 16.0;
            rankOverlay.Height = 13.0;

            Point p = Utility.GetSnapPoint(rankOverlay, location.VisualElement);

            this.gameObjects.Children.Add(rankOverlay);
            rankOverlay.SetValue(Canvas.LeftProperty, p.X);
            rankOverlay.SetValue(Canvas.TopProperty, p.Y);
            this.rankOverlays.Add(rankOverlay, route);
        }

        public void HighlightLocation(BoardMoveLocation location, Boolean useOverlay, BoardMoveRoute route)
        {
            DoubleAnimationUsingKeyFrames dauk = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame edk1 = new EasingDoubleKeyFrame();
            edk1.KeyTime = new TimeSpan(0, 0, 0, 0, 0);
            edk1.Value = 0.0;
            EasingDoubleKeyFrame edk2 = new EasingDoubleKeyFrame();
            edk2.KeyTime = new TimeSpan(0, 0, 0, 0, 600);
            edk2.Value = 0.55;
            dauk.KeyFrames.Add(edk1);
            dauk.KeyFrames.Add(edk2);
            if (useOverlay)
            {
                FrameworkElement overlay = this.GetHighlightOverlay();
                Point p = Utility.GetSnapPoint(overlay, location.VisualElement);
                this.gameObjects.Children.Add(overlay);
                overlay.SetValue(Canvas.LeftProperty, p.X);
                overlay.SetValue(Canvas.TopProperty, p.Y);
                this.highlightOverlays.Add(overlay, route);
                overlay.MouseEnter += new MouseEventHandler(overlay_MouseEnter);
                overlay.MouseLeave += new MouseEventHandler(overlay_MouseLeave);
                overlay.MouseLeftButtonUp += new MouseButtonEventHandler(overlay_MouseLeftButtonUp);
                
                Storyboard.SetTarget(dauk, overlay);
            }
            else
            {
                Storyboard.SetTarget(dauk, location.VisualElement);
            }
            Storyboard.SetTargetProperty(dauk, new PropertyPath(UIElement.OpacityProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(dauk);
            sb.RepeatBehavior = RepeatBehavior.Forever;
            sb.AutoReverse = true;
            sb.Begin();
            this.highlightStoryboards.Add(sb);
        }

        private void ClearAllHighlights()
        {
            foreach (var item in this.highlightStoryboards)
            {
                item.Stop();
            }
            this.highlightStoryboards.Clear();

            foreach (var item in this.highlightOverlays)
            {
                item.Key.MouseEnter -= overlay_MouseEnter;
                item.Key.MouseLeave -= overlay_MouseLeave;
                item.Key.MouseLeftButtonUp -= overlay_MouseLeftButtonUp;
                this.gameObjects.Children.Remove(item.Key);
            }
            this.highlightOverlays.Clear();

            foreach (var item in this.rankOverlays)
            {
                this.gameObjects.Children.Remove(item.Key);
            }
            this.rankOverlays.Clear();
        }

        private FrameworkElement GetHighlightOverlay()
        {
            return Utility.CreateHighlightOverlay();
        }

        public void SetLocationStatus(PlayerPiece piece, BoardMoveLocation location)
        {
            if (piece.CurrentLocation != null)
            {
                piece.CurrentLocation.Occupied = false;
                piece.CurrentLocation.OccupiedPlayerPiece = null;
            }

            if (location.Occupied)
            {
                if (location.OccupiedPlayerPiece.PlayerNumber == piece.PlayerNumber)
                {
                    //Raise error: this should never happen
                    throw new Exception("Route should not allow a piece to be moved on a location the same player is on");
                }
                else
                {
                    PlayerPiece oldPiece = location.OccupiedPlayerPiece;
                    this.RaiseLandedOnPlayer(piece, oldPiece);
                }
            }

            piece.CurrentLocation = location;
            piece.PendingLocation = null;
            location.Occupied = true;
            location.OccupiedPlayerPiece = piece;
            
            if (piece.CurrentLocation.LocationType == LocationType.Home)
            {
                Int32 homeCount = 1;
                var pieces = BoardManager.GetPlayerPieces(piece.Player);
                foreach (var item in pieces)
                {
                    if (item != piece)
                    {
                        if (item.CurrentLocation != null && item.CurrentLocation.LocationType == LocationType.Home)
                        {
                            //this.eventAggregator.GetEvent<GameStatusEvent>().Publish(new GameStatusData(String.Format("Player {0} brought all pieces to the home location{1}", piece.Player.Name, Environment.NewLine)));
                            homeCount++;
                        }
                    }
                }

                if (homeCount == 4)
                {
                    this.eventAggregator.GetEvent<GameStatusEvent>().Publish(new GameStatusData(String.Format("Player {0} brought all pieces to the home location{1}", piece.Player.Name, Environment.NewLine)));
                    this.RaisePlayerFinished(piece.Player);
                }
            }
        }

        public void MoveToLocation(PlayerPiece piece, BoardMoveLocation location, Boolean assignEvent, Boolean multiMove)
        {
            piece.PendingLocation = location;
            if (!multiMove) this.SetLocationStatus(piece, piece.PendingLocation);
            Point destinationPoint = Utility.GetSnapPoint(piece.VisualElement, location.VisualElement);

            DoubleAnimation daTop = new DoubleAnimation();
            daTop.To = destinationPoint.Y;
            daTop.Duration = new Duration(new TimeSpan(0, 0, 0, 0, Constants.MoveAnimationDelay));
            Storyboard.SetTarget(daTop, piece.VisualElement);
            Storyboard.SetTargetProperty(daTop, new PropertyPath(Canvas.TopProperty));
            DoubleAnimation daLeft = new DoubleAnimation();
            daLeft.To = destinationPoint.X;
            daLeft.Duration = new Duration(new TimeSpan(0, 0, 0, 0, Constants.MoveAnimationDelay));
            Storyboard.SetTarget(daLeft, piece.VisualElement);
            Storyboard.SetTargetProperty(daLeft, new PropertyPath(Canvas.LeftProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(daTop);
            sb.Children.Add(daLeft);
            if (assignEvent) sb.Completed += new EventHandler(rollToStoryboard_Completed);
            sb.Begin();
        }

        private void SetFastTrackStatus(PlayerPiece piece)
        {
            if (piece.CurrentLocation == null) return;

            foreach (var item in this.CurrentPlayer.Pieces)
            {
                if (item == piece && piece.CurrentLocation.LocationType == LocationType.FastTrack)
                {
                    piece.IsOnFastTrack = true;

                    Ellipse pieceEllipse = item.VisualElement as Ellipse;

                    pieceEllipse.Stroke = Utility.CreateFastTrackBrush();
                    pieceEllipse.StrokeThickness = 3.0;
                    pieceEllipse.UpdateLayout();
                }
                else
                {
                    this.TakePieceOffFasttrack(item);
                }
            }            
        }

        private void TakePieceOffFasttrack(PlayerPiece item)
        {
            item.IsOnFastTrack = false;

            Ellipse pieceEllipse = item.VisualElement as Ellipse;
            pieceEllipse.Stroke = new SolidColorBrush(Colors.Black);
            pieceEllipse.StrokeThickness = 1.0;
            pieceEllipse.UpdateLayout();            
        }

        public void MoveToLocationAll(PlayerPiece piece, BoardMoveLocation location)
        {
            this.spinAnimation.Begin();
            this.MoveToLocation(piece, location, true, true);
        }

        private void rollToStoryboard_Completed(object sender, EventArgs e)
        {
            spinAnimation.Stop();
            if (!this.resetting) this.ProcessTurn();
        }

        public static Int32 GetCardValue(CardRank rank)
        {
            Int32 value = (Int32)rank;
            if (rank == CardRank.None) value = 1;  //Joker
            if (rank == CardRank.Jack) value = 1;
            if (rank == CardRank.King) value = 1;
            if (rank == CardRank.Queen) value = 1;
            return value;
        }

        public void ProcessTurn()
        {
            //Run animations and move piece for each move along the path; then return here
            if (this.currentTurnPath.Count > 0)
            {
                this.MoveToLocationAll(this.activePiece, this.currentTurnPath.Dequeue());
            }
            //All moves along the path are complete, end move
            else
            {
                if (this.activePiece == null || this.activePiece.Player == null) return;
                Player p = this.activePiece.Player;
                PlayerPiece piece = this.activePiece;
                this.SetLocationStatus(this.activePiece, this.activePiece.PendingLocation);
                this.activePiece = null;
                this.RaiseTurnProcessed(p, piece);
            }
        }

        private void Game_LandedOnPlayer(object sender, LandedOnPlayerEventArgs e)
        {
            this.TakePieceOffFasttrack(e.ExistingPlayer);
            this.MoveToBase(e.ExistingPlayer);
        }

        public BoardMoveRoutes GetPossibleRoutes(PlayerPiece piece, CardRank rank)
        {
            BoardMoveRoutes routes = new BoardMoveRoutes();
            Int32 cardValue = Game.GetCardValue(rank);

            //Case when piece is currently at base location
            if (piece.CurrentLocation == null) return null;
            if (piece.CurrentLocation.LocationType == LocationType.Base)
            {
                if (rank == CardRank.None || rank == CardRank.Ace || rank == CardRank.Six)
                {
                    BoardMoveLocation startLocation = BoardManager.GetPlayerStartLocation(piece.Player);
                    if (startLocation.Occupied && startLocation.OccupiedPlayerPiece.PlayerNumber == piece.PlayerNumber)
                        return routes;

                    BoardMoveRoute locations = new BoardMoveRoute();
                    locations.StartingLocation = piece.CurrentLocation;
                    locations.Direction = RouteDirection.Forward;
                    locations.Add(startLocation);
                    routes.Add(locations);
                    return routes;
                }
                else
                {
                    return routes;
                }
            }

            //Case when piece is currently at home location
            if (piece.CurrentLocation.LocationType == LocationType.Home)
            {
                if (rank == CardRank.Four) return routes;

                BoardMoveRoute locations = new BoardMoveRoute();
                locations.StartingLocation = piece.CurrentLocation;
                locations.Direction = RouteDirection.Forward;
                foreach (var location in BoardManager.GetPlayerHomeLocations(piece.Player))
                {
                    if (location.LocationNumber > piece.CurrentLocation.LocationNumber)
                    {
                        if (location.Occupied)
                            break;
                        else
                            locations.Add(location);
                    }

                    if (locations.Count == cardValue) break;
                }

                if ((locations.Count > 0) && (locations.Count == cardValue)) routes.Add(locations);
                return routes;
            }

            //Case when piece is currently on super fast track location
            if (piece.CurrentLocation.LocationType == LocationType.SuperFastTrack)
            {
                if (cardValue != 1) return routes;
                foreach (var location in BoardManager.FastTrackLocationsSorted)
                {
                    BoardMoveRoute locations = new BoardMoveRoute();
                    locations.StartingLocation = piece.CurrentLocation;
                    locations.Direction = RouteDirection.Forward;
                    locations.Add(location);
                    routes.Add(locations);
                }

                return routes;
            }

            //Case when piece is currently on regular game track
            if (piece.IsOnFastTrack)        //If on fast track, calculate all possible fast track routes
            {
                //If move is 1, add route to super fast track
                if (cardValue == 1)
                {
                    if (!BoardManager.SuperFastTrack.Occupied || (BoardManager.SuperFastTrack.Occupied && BoardManager.SuperFastTrack.OccupiedPlayerPiece != piece))
                    {
                        BoardMoveRoute sftRoute = new BoardMoveRoute();
                        sftRoute.StartingLocation = piece.CurrentLocation;
                        sftRoute.Direction = RouteDirection.Forward;
                        sftRoute.Add(BoardManager.SuperFastTrack);
                        routes.Add(sftRoute);
                    }
                }

                //Add moves from current fast track location to regular locations
                BoardMoveRoute initialRoute = BoardManager.GetValidRegularTrackRouteFromLocation(piece.CurrentLocation, piece, cardValue, false);
                if (initialRoute != null) routes.Add(initialRoute);

                //Gets an ordered list of only valid fast track locations starting from the one currently on
                //TODO: Is it allowed for a piece to move backwards fasttrack?
                BoardMoveRoute validFtLocations = BoardManager.GetValidFastTrackLocations(piece);
                if ((cardValue != 4) && (validFtLocations.Count > 0))
                {
                    //Calculate a route off each valid fast track location
                    for (Int32 validFtCount = 1; validFtCount <= validFtLocations.Count; validFtCount++)
                    {
                        //Add fast track locations
                        Boolean reachedEnd = false;
                        Boolean validRoute = true;
                        BoardMoveRoute locations = new BoardMoveRoute();
                        locations.StartingLocation = piece.CurrentLocation;
                        locations.Direction = RouteDirection.Forward;
                        //TODO: Handle backwards 4, and handle invalid back spaces
                        //Can you go backwards on fast track?
                        for (Int32 numFt = 1; numFt <= validFtCount; numFt++)
                        {
                            if (validFtLocations[numFt - 1].Occupied && validFtLocations[numFt - 1].OccupiedPlayerPiece.PlayerNumber == piece.PlayerNumber)
                            {
                                validRoute = false;
                                break;
                            }
                            else
                            {
                                locations.Add(validFtLocations[numFt - 1]);
                                if (numFt == cardValue)
                                {
                                    reachedEnd = true;
                                    break;
                                }
                            }
                        }

                        if (reachedEnd)
                        {
                            if (validRoute) routes.Add(locations);
                            break;
                        }

                        //Add regular locations branching from a fast track location
                        BoardMoveRoute regRoute = null;
                        if (locations.Count > 0)
                        {
                            if (locations.Count < cardValue)
                            {
                                regRoute =
                                    BoardManager.GetValidRegularTrackRouteFromLocation(locations[locations.Count - 1],
                                                                                       piece,
                                                                                       cardValue - locations.Count, true);
                                if (regRoute != null)
                                {
                                    foreach (var item in regRoute)
                                    {
                                        locations.Add(item);
                                    }
                                }

                                if (validRoute && regRoute != null) routes.Add(locations);
                            }
                            else
                            {
                                if (validRoute) routes.Add(locations);
                            }
                        }
                    }
                }
            }
            else        //Not on fast track
            {
                BoardMoveRoute regRoute = BoardManager.GetValidRegularTrackRouteFromLocation(piece.CurrentLocation, piece, cardValue, false);
                if (regRoute != null)
                {
                    routes.Add(regRoute);

                    //If second to last location is a fast track, then add a route to super fast track
                    //TODO: Is this allowed from a backwards move?
                    if ((regRoute.Direction == RouteDirection.Forward) && 
                        ((regRoute.Count > 1) && 
                        (regRoute[regRoute.Count - 2].LocationType == LocationType.FastTrack)))
                    {
                        if (!BoardManager.SuperFastTrack.Occupied || (BoardManager.SuperFastTrack.Occupied && BoardManager.SuperFastTrack.OccupiedPlayerPiece.Player != piece.Player))
                        {
                            BoardMoveRoute sftRoute = new BoardMoveRoute();
                            sftRoute.StartingLocation = piece.CurrentLocation;
                            sftRoute.Direction = RouteDirection.Forward;
                            foreach (var item in regRoute)
                            {
                                sftRoute.Add(item);
                            }
                            sftRoute.RemoveAt(sftRoute.Count - 1);      //Remove location after fast track
                            sftRoute.Add(BoardManager.SuperFastTrack);
                            routes.Add(sftRoute);
                        }
                    }
                    //If on fast track location and one move, then add a route to super fast track
                    else if ((cardValue == 1) && (piece.CurrentLocation.LocationType == LocationType.FastTrack))
                    {
                        if (!BoardManager.SuperFastTrack.Occupied || (BoardManager.SuperFastTrack.Occupied && BoardManager.SuperFastTrack.OccupiedPlayerPiece.Player != piece.Player))
                        {
                            BoardMoveRoute sftRoute = new BoardMoveRoute();
                            sftRoute.StartingLocation = piece.CurrentLocation;
                            sftRoute.Direction = RouteDirection.Forward;
                            sftRoute.Add(BoardManager.SuperFastTrack);
                            routes.Add(sftRoute);
                        }
                    }
                }
            }

            return routes;
        }

        private void ActivateCurrentPlayer()
        {
            this.CurrentPlayer.GamePanel.IsActive = true;
            this.CurrentPlayer.GamePanel.IsDrawEnabled = true;
            if (this.CurrentPlayer is ComputerPlayer) this.gameAi.StartTurn(this, (ComputerPlayer)this.CurrentPlayer);
        }

        private void DeactivateCurrentPlayer()
        {
            this.CurrentPlayer.GamePanel.IsActive = false;
            this.CurrentPlayer.GamePanel.IsDrawEnabled = false;
        }

        public void SwitchToNextPlayer()
        {
            this.DeactivateCurrentPlayer();
            Player findPlayer = this.CurrentPlayer;
            this.CurrentPlayer = null;
            Player nextPlayer = null;
            Boolean found = false;
            List<Player> availablePlayers = new List<Player>();

            foreach (var player in this.Players)
            {
                if (!this.playersFinished.ContainsKey(player.Value)) availablePlayers.Add(player.Value);
            }

            //Game over
            if (availablePlayers.Count == 0) 
            {
                this.RaiseGameFinished();
                return;
            }

            //Find the next available player
            if (availablePlayers.Count == 1)
            {
                nextPlayer = this.Players[availablePlayers.FirstOrDefault().Number];
            }
            else
            {
                foreach (var player in this.Players)
                {
                    if (found && availablePlayers.Contains(player.Value))
                    {
                        nextPlayer = player.Value;
                        break;
                    }

                    if (player.Key == findPlayer.Number) found = true;
                }

                if (!found || nextPlayer == null)
                {
                    foreach (var player in this.Players)
                    {
                        if (availablePlayers.Contains(player.Value))
                        {
                            nextPlayer = player.Value;
                            break;
                        }
                    }
                }
            }

            this.CurrentPlayer = nextPlayer;
            this.ActivateCurrentPlayer();
        }

        private Boolean HasMoves(Player player)
        {
            if (this.playersFinished.ContainsKey(player)) return false;

            List<PlayerPiece> pieces = BoardManager.GetPlayerPieces(player);

            foreach (BoardMoveRoutes routes in pieces.Select(piece => this.GetPossibleRoutes(piece, player.UsedDeck.TopCard.Rank)))
            {
                if (routes == null) return false;
                if (routes.Count > 0) return true;
            }

            return false;
        }

        private void GamePanel_PlayerDraws(object sender, PlayerDrawsEventArgs e)
        {
            //Check for deck replenish
            if (!e.Player.CurrentDeck.HasCards)
            {
                e.Player.UsedDeck.RemoveAllCards();
                e.Player.CurrentDeck.Deal();
                e.Player.CurrentDeck.Shuffle();
                e.Player.GamePanel.SetInitialDecks();
            }

            //Draw card
            PlayerGamePanel panel = (PlayerGamePanel)sender;
            e.Player.CurrentDeck.DrawCard(1, e.Player.UsedDeck);
            e.Player.UsedDeck.TopCard.FaceUp = true;
            e.Player.GamePanel.DrawCard();
            e.Player.GamePanel.IsDrawEnabled = false;
            
            //Check for end of deck
            //if (!e.Player.CurrentDeck.HasCards)
            //{
            //    panel.SetCurrentEmpty();
            //}

            if (e.Player is ComputerPlayer) return;

            //Check for no moves
            if (!this.HasMoves(e.Player))
            {
                if (e.Player.UsedDeck.TopCard.Rank == CardRank.Jack ||
                    e.Player.UsedDeck.TopCard.Rank == CardRank.King ||
                    e.Player.UsedDeck.TopCard.Rank == CardRank.Queen ||
                    e.Player.UsedDeck.TopCard.Rank == CardRank.Ace ||
                    e.Player.UsedDeck.TopCard.Rank == CardRank.Six ||
                    e.Player.UsedDeck.TopCard.CardType == CardType.Joker)
                {
                    e.Player.GamePanel.IsDrawEnabled = true;
                }
                else
                {
                    this.SwitchToNextPlayer();
                }
            }
        }

        protected void GamePanel_PlayerDrawCompleted(object sender, PlayerDrawCompletedEventArgs e)
        {
            if (this.CurrentPlayer is ComputerPlayer) this.gameAi.FinishTurn(this, (ComputerPlayer)this.CurrentPlayer);
        }

        private void Game_PlayerFinished(object sender, PlayerFinishedEventArgs e)
        {
            this.playersFinished.Add(e.Player, DateTime.Now);            
        }

        void Game_GameFinished(object sender, EventArgs e)
        {
            Player champion = this.playersFinished.OrderBy(p => p.Value).FirstOrDefault().Key;
            this.eventAggregator.GetEvent<GameStatusEvent>().Publish(new GameStatusData(String.Format("Game complete.  Player {0} has won.{1}", champion.Name, Environment.NewLine)));
        }

        private void Game_TurnProcessed(object sender, TurnProcessedEventArgs e)
        {
            this.SetFastTrackStatus(e.PlayerPiece);

            //Check for completed player
            if (this.playersFinished.ContainsKey(e.Player))
            {
                this.DeactivateCurrentPlayer();
                this.SwitchToNextPlayer();
                return;
            }

            //Check for additional turns
            if (e.Player.UsedDeck.TopCard.Rank == CardRank.Jack ||
                e.Player.UsedDeck.TopCard.Rank == CardRank.King ||
                e.Player.UsedDeck.TopCard.Rank == CardRank.Queen ||
                e.Player.UsedDeck.TopCard.Rank == CardRank.Ace ||
                e.Player.UsedDeck.TopCard.Rank == CardRank.Six ||
                e.Player.UsedDeck.TopCard.CardType == CardType.Joker)
            {
                e.Player.GamePanel.IsDrawEnabled = true;
                if (this.CurrentPlayer is ComputerPlayer) this.gameAi.StartTurn(this, (ComputerPlayer)this.CurrentPlayer);
            }
            else
            {
                this.SwitchToNextPlayer();
            }            
        }

        //Called when you click a location to move a piece to
        private void overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ClearTurnPath();

            foreach (var item in this.highlightOverlays[sender as FrameworkElement])
            {
                this.EnQueueLocation(item);
            }

            this.BringPieceToTop();

            this.ClearAllHighlights();
            this.ProcessTurn();
        }

        internal void BringPieceToTop()
        {
            if (this.activePiece == null) return;
            
            if (this.gameObjects.Children.Contains(this.activePiece.VisualElement))
            {
                this.gameObjects.Children.Remove(this.activePiece.VisualElement);
                this.gameObjects.Children.Add(this.activePiece.VisualElement);
            }
        }

        public void ClearTurnPath()
        {
            this.currentTurnPath.Clear();
        }

        public void EnQueueLocation(BoardMoveLocation item)
        {
            this.currentTurnPath.Enqueue(item);
        }

        private void overlay_MouseLeave(object sender, MouseEventArgs e)
        {
            this.shellGrid.Cursor = Cursors.Arrow;
            FrameworkElement overlay = sender as FrameworkElement;
            overlay.Width = overlay.Width - 10;
            overlay.Height = overlay.Height - 10;
            overlay.SetValue(Canvas.LeftProperty, (Double)overlay.GetValue(Canvas.LeftProperty) + 5.0);
            overlay.SetValue(Canvas.TopProperty, (Double)overlay.GetValue(Canvas.TopProperty) + 5.0);
        }

        private void overlay_MouseEnter(object sender, MouseEventArgs e)
        {
            this.shellGrid.Cursor = Cursors.Hand;
            FrameworkElement overlay = sender as FrameworkElement;
            overlay.Width = overlay.Width + 10;
            overlay.Height = overlay.Height + 10;
            overlay.SetValue(Canvas.LeftProperty, (Double)overlay.GetValue(Canvas.LeftProperty) - 5.0);
            overlay.SetValue(Canvas.TopProperty, (Double)overlay.GetValue(Canvas.TopProperty) - 5.0);
        }

        private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CurrentPlayer == null) return;

            PlayerPiece piece = BoardManager.Pieces[sender as FrameworkElement];
            if (piece.Player == this.CurrentPlayer && 
                this.activePiece != piece && 
                !this.CurrentPlayer.GamePanel.IsDrawEnabled)
            {
                this.ClearAllHighlights();
                this.SetActivePiece(piece);
                BoardMoveRoutes routes = this.GetPossibleRoutes(this.activePiece, this.CurrentPlayer.UsedDeck.TopCard.Rank);
                this.HighlightRoutes(routes);
            }
        }

        public void SetActivePiece(PlayerPiece piece)
        {
            this.activePiece = piece;
        }

        private void Piece_MouseLeave(object sender, MouseEventArgs e)
        {
            this.shellGrid.Cursor = Cursors.Arrow;
        }

        private void Piece_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.CurrentPlayer == null) return;
            PlayerPiece piece = BoardManager.Pieces[sender as FrameworkElement];
            if (piece.Player == this.CurrentPlayer && this.activePiece != piece && !this.CurrentPlayer.GamePanel.IsDrawEnabled)
                this.shellGrid.Cursor = Cursors.Hand;
        }

        private void mainControl_KeyDown(object sender, KeyEventArgs e)
        {
            //CTRL-SHIFT-H toggles hints
            if (Keyboard.Modifiers == ModifierKeys.Shift & e.Key == Key.H)
            {
                this.showTips = !this.showTips;

                //TextBlock t = new TextBlock();
                //t.FontSize = 10.0;
                //t.Foreground = new SolidColorBrush(Colors.White);
                //t.Text = "Show tips? " + this.showTips.ToString();
                //this.gameObjects.Children.Add(t);
            }
        }
    }
}
