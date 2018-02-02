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
using Aggravation.GameEngine.Interfaces;

namespace Aggravation.GameEngine
{
    public class GameAi : IGame
    {
        protected IGame _game;

        public GameAi(IGame game)
        {
            this._game = game;
        }

        public void StartTurn(Game game, ComputerPlayer player)
        {
            player.GamePanel.DrawComputer();
        }

        public void FinishTurn(Game game, ComputerPlayer player)
        {
            Dictionary<PlayerPiece, BoardMoveRoutes> allRoutes = this.GetRoutesAllPieces(game, player);
            GameAi.RankedRoutes rankedRoutes = this.GetRankedRoutes(allRoutes, player);
            RankedRoute rankedRoute = this.SelectRoute(rankedRoutes, player);

            if (rankedRoute == null)
            {
                game.SwitchToNextPlayer();
                return;
            }

            game.ClearTurnPath();
            foreach (var location in rankedRoute.Route)
            {
                game.EnQueueLocation(location);
            }

            game.SetActivePiece(rankedRoute.Piece);
            game.BringPieceToTop();
            game.ProcessTurn();
        }

        public Dictionary<PlayerPiece, BoardMoveRoutes> GetRoutesAllPieces(Game game, Player player)
        {
            Dictionary<PlayerPiece, BoardMoveRoutes> routesAll = new Dictionary<PlayerPiece,BoardMoveRoutes>();

            foreach (var piece in player.Pieces)
            {
                BoardMoveRoutes routes = game.GetPossibleRoutes(piece, player.UsedDeck.TopCard.Rank);
                if (routes == null) return null;
                if (routes.Count > 0) routesAll.Add(piece, routes);
            }

            return routesAll;
        }

        public RankedRoutes GetRankedRoutes(Dictionary<PlayerPiece, BoardMoveRoutes> allRoutes, Player player)
        {
            //Best routes in order of best to worst
            //1 - Route where piece is already on super fast track
            //2 - Route where piece is already on fast track
            //3 - Route landing on another player (forward or backward)
            //4 - Route going backwards 4 moves to before home locations
            //5 - Route landing on fast track
            //6 - Route landing on super fast track
            //7 - Route getting out of base
            //8 - Route getting into home
            //9 - Route moving off start location
            //10 - Route progressing in home
            //11 - Routes forward (and all other cases)
            //12 - Routes backward (and all other cases)
            //13 - Route going backwards where start is blocked

            if (allRoutes == null) return null;
            RankedRoutes routes = new RankedRoutes();

            //Loop through all routes for all pieces
            foreach (var item in allRoutes)
            {
                //1 - Route where piece is already on super fast track
                if (item.Key.CurrentLocation.LocationType == LocationType.SuperFastTrack)
                {
                    BoardMoveRoute ftLocationsRanked = BoardManager.GetBestFastTrackLocations(item.Key);
                    foreach (var ftLocation in ftLocationsRanked)
                    {
                        foreach (var route in item.Value)
                        {
                            //Find first route that contains the best fast track location (closest to home)
                            if (route[0] == ftLocation)
                            {
                                routes.Add(new RankedRoute(1, route, item.Key));
                                continue;
                            }
                        }
                    }

                    continue;
                }

                //2 - Route where piece is already on fast track
                if (item.Key.IsOnFastTrack)
                {
                    //Choose last ft route (this would be the one hopping across the most ft locations)
                    routes.Add(new RankedRoute(2, item.Value[item.Value.Count - 1], item.Key));
                    continue;
                }

                //For the other cases, loop through all routes for the piece to find the other highest ranked cases
                foreach (var route in item.Value)
                {
                    //3 - Route landing on another player (forward or backward)
                    if (route[route.Count - 1].Occupied && route[route.Count - 1].OccupiedPlayerPiece.Player != player)
                    {
                        routes.Add(new RankedRoute(3, route, item.Key));
                        continue;
                    }

                    //4 - Route going backwards 4 moves to before home locations
                    if (route.Direction == RouteDirection.Backward)
                    {
                        foreach (var location in route)
                        {
                            if ((Int32)location.LocationOwner == player.Number && location.LocationType == LocationType.Finish)
                            {
                                routes.Add(new RankedRoute(4, route, item.Key));
                                continue;
                            }
                        }
                    }

                    //5 - Route landing on fast track
                    if (route[route.Count - 1].LocationType == LocationType.FastTrack)
                    {
                        routes.Add(new RankedRoute(5, route, item.Key));
                        continue;
                    }

                    //6 - Route landing on super fast track
                    if (route[route.Count - 1].LocationType == LocationType.SuperFastTrack)
                    {
                        routes.Add(new RankedRoute(6, route, item.Key));
                        continue;
                    }

                    //7 - Route getting out of base
                    if (item.Key.CurrentLocation.LocationType == LocationType.Base &&
                        route[route.Count - 1].LocationType == LocationType.Start &&
                        (Int32)route[route.Count - 1].LocationOwner == item.Key.PlayerNumber)
                    {
                        routes.Add(new RankedRoute(7, route, item.Key));
                        continue;
                    }

                    //8 - Route getting into home
                    if (route.StartingLocation.LocationType != LocationType.Home &&
                        route[route.Count - 1].LocationType == LocationType.Home)
                    {
                        routes.Add(new RankedRoute(8, route, item.Key));
                        continue;
                    }

                    //10 - Route progressing in home
                    if (route.StartingLocation.LocationType == LocationType.Home &&
                        route[route.Count - 1].LocationType == LocationType.Home)
                    {
                        routes.Add(new RankedRoute(10, route, item.Key));
                        continue;
                    }

                    //9 - Route moving off start location
                    if (route.StartingLocation.LocationType == LocationType.Start &&
                        (Int32)route.StartingLocation.LocationOwner == item.Key.PlayerNumber)
                    {
                        routes.Add(new RankedRoute(9, route, item.Key));
                        continue;
                    }

                    //13 - Route going backwards where start is blocked
                    if (route.Direction == RouteDirection.Backward &&
                        route[route.Count - 1].LocationType == LocationType.Start &&
                        (Int32)route[route.Count - 1].LocationOwner == item.Key.PlayerNumber)
                    {
                        routes.Add(new RankedRoute(13, route, item.Key));
                        continue;
                    }

                    //11\12 - Routes forward\backward (and all other cases)
                    if (route.Direction == RouteDirection.Forward)
                        routes.Add(new RankedRoute(11, route, item.Key));
                    else
                        routes.Add(new RankedRoute(12, route, item.Key));
                }
            }

            routes.Sort(new RankedRouteComparer());
            return routes;
        }

        public RankedRoute SelectRoute(RankedRoutes routes, ComputerPlayer player)
        {
            if (routes == null) return null;
            if (routes.Count == 0) return null;

            switch (player.Difficulty)
            {
                case GameDifficulty.Easy:   //Always take worst route
                    return routes[routes.Count - 1];
                
                case GameDifficulty.Medium: //Randomly choose route
                    Random r = new Random(DateTime.Now.Millisecond);
                    return routes[r.Next(0, routes.Count - 1)];

                case GameDifficulty.Hard:   //Always take best route
                    return routes[0];
            }

            return null;
        }

        public class RankedRoute
        {
            public Int32 Rank { get; set; }
            public BoardMoveRoute Route { get; set; }
            public PlayerPiece Piece { get; set; }

            public RankedRoute (Int32 rank, BoardMoveRoute route, PlayerPiece piece)
	        {
                this.Rank = rank;
                this.Route = route;
                this.Piece = piece;
	        }

            public Int32 CompareTo(RankedRoute other)
            {
                Int32 value1 = this.Rank;
                Int32 value2 = other.Rank;

                if (value1 > value2)
                    return 1;
                else if (value1 < value2)
                    return -1;
                else
                    return 0;
            }
        }

        public class RankedRoutes : List<RankedRoute>
        {
        }
    }

    public class RankedRouteComparer : IComparer<GameAi.RankedRoute>
    {
        public Int32 Compare(GameAi.RankedRoute x, GameAi.RankedRoute y)
        {
            return x.CompareTo(y);
        }
    }
}
