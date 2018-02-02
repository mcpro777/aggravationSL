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
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;

namespace Aggravation.GameEngine
{
    public static class BoardManager
    {
        private static Boolean _isInitialized;
        internal static IDictionary<DependencyObject, BoardMoveLocation> Locations = new Dictionary<DependencyObject, BoardMoveLocation>();
        internal static IDictionary<DependencyObject, PlayerPiece> Pieces = new Dictionary<DependencyObject, PlayerPiece>();
        public static List<BoardMoveLocation> TrackLocationsSorted = new List<BoardMoveLocation>();
        public static List<BoardMoveLocation> BaseLocationsSorted = new List<BoardMoveLocation>();
        public static List<BoardMoveLocation> HomeLocationsSorted = new List<BoardMoveLocation>();
        public static List<BoardMoveLocation> FastTrackLocationsSorted = new List<BoardMoveLocation>();
        public static BoardMoveLocation SuperFastTrack = null;

        public static readonly DependencyProperty IsLocationProperty =
            DependencyProperty.RegisterAttached("IsLocation", typeof(Boolean), typeof(BoardManager), new PropertyMetadata(new PropertyChangedCallback(OnIsLocationChanged)));

        public static void SetIsLocation(DependencyObject o, Boolean value)
        {
            o.SetValue(IsLocationProperty, value);
        }

        public static Boolean GetIsLocation(DependencyObject o)
        {
            return (Boolean)o.GetValue(IsLocationProperty);
        }

        private static void OnIsLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            if (!_isInitialized) Initialize();

            BoardMoveLocation location = d as BoardMoveLocation;

            if ((Boolean)e.NewValue)
                SaveLocationIfNeeded(location);
            else
                RemoveLocationIfSaved(location);
        }

        public static readonly DependencyProperty IsPieceProperty =
            DependencyProperty.RegisterAttached("IsPiece", typeof(Boolean), typeof(BoardManager), new PropertyMetadata(new PropertyChangedCallback(OnIsPieceChanged)));

        public static void SetIsPiece(DependencyObject o, Boolean value)
        {
            o.SetValue(IsPieceProperty, value);
        }

        public static Boolean GetIsPiece(DependencyObject o)
        {
            return (Boolean)o.GetValue(IsPieceProperty);
        }

        private static void OnIsPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            if (!_isInitialized) Initialize();

            PlayerPiece piece = d as PlayerPiece;

            if ((Boolean)e.NewValue)
                SavePieceIfNeeded(piece);
            else
                RemovePieceIfSaved(piece);
        }

        private static void Initialize()
        {
            _isInitialized = true;
        }

        internal static void SaveLocationIfNeeded(BoardMoveLocation location)
        {
            if (!Locations.ContainsKey(location.VisualElement))
            {
                Locations.Add(location.VisualElement, location);

                if (location.LocationType == LocationType.Base)
                {
                    BaseLocationsSorted.Add(location);
                    BaseLocationsSorted.Sort(new BoardMoveLocationComparer());
                }

                if (location.LocationType == LocationType.Regular ||
                    location.LocationType == LocationType.Start ||
                    location.LocationType == LocationType.Finish ||
                    location.LocationType == LocationType.FastTrack)
                {
                    TrackLocationsSorted.Add(location);
                    TrackLocationsSorted.Sort(new BoardMoveLocationComparer());
                }

                if (location.LocationType == LocationType.FastTrack)
                {
                    FastTrackLocationsSorted.Add(location);
                    FastTrackLocationsSorted.Sort(new FastTrackLocationComparer());
                }

                if (location.LocationType == LocationType.Home)
                {
                    HomeLocationsSorted.Add(location);
                    HomeLocationsSorted.Sort(new BoardMoveLocationComparer());
                }
                
                if (location.LocationType == LocationType.SuperFastTrack) SuperFastTrack = location;
            }
        }

        internal static void RemoveLocationIfSaved(BoardMoveLocation location)
        {
            if (Locations.ContainsKey(location.VisualElement))
            {
                Locations.Remove(location.VisualElement);
                BaseLocationsSorted.Remove(location);
                TrackLocationsSorted.Remove(location);
                FastTrackLocationsSorted.Remove(location);
                HomeLocationsSorted.Remove(location);
            }
        }

        internal static void SavePieceIfNeeded(PlayerPiece piece)
        {
            if (!Pieces.ContainsKey(piece.VisualElement))
            {
                Pieces.Add(piece.VisualElement, piece);
            }
        }

        internal static void RemovePieceIfSaved(PlayerPiece piece)
        {
            if (Pieces.ContainsKey(piece.VisualElement))
            {
                Pieces.Remove(piece.VisualElement);
            }
        }

        public static void SetPlayerPieces(Player player)
        {
            foreach (var item in Pieces.Where(item => item.Value.PlayerNumber == player.Number))
            {
                item.Value.Player = player;
                player.Pieces.Add(item.Value);
            }
        }

        public static Int32 GetTrackLocationIndex(BoardMoveLocation location)
        {
            return TrackLocationsSorted.BinarySearch(location, new BoardMoveLocationComparer());
        }

        public static BoardMoveLocation GetPlayerStartLocation(Player player)
        {
            return TrackLocationsSorted.FirstOrDefault(item => item.LocationOwner.GetHashCode() == player.Number && item.LocationType == LocationType.Start);
        }

        public static BoardMoveLocation GetPlayerFinishLocation(Player player)
        {
            return TrackLocationsSorted.FirstOrDefault(item => (Int32) item.LocationOwner == player.Number && item.LocationType == LocationType.Finish);
        }

        public static BoardMoveLocation GetPlayerFastTrackLocation(Player player)
        {
            return FastTrackLocationsSorted.FirstOrDefault(item => (Int32) item.LocationOwner == player.Number && item.LocationType == LocationType.FastTrack);
        }

        public static BoardMoveRoute GetPlayerBaseLocations(Int32 playerNumber)
        {
            BoardMoveRoute locations = new BoardMoveRoute();
            locations.AddRange(BaseLocationsSorted.Where(item => (Int32) item.LocationOwner == playerNumber));

            return locations;
        }

        public static BoardMoveRoute GetPlayerHomeLocations(Player player)
        {
            BoardMoveRoute locations = new BoardMoveRoute();
            locations.AddRange(HomeLocationsSorted.Where(item => (Int32) item.LocationOwner == player.Number));
            locations.Sort(new BoardMoveLocationComparer());

            return locations;
        }

        public static List<PlayerPiece> GetPlayerPieces(Player player)
        {
            return (from item in Pieces where item.Value.Player == player select item.Value).ToList();
        }

        public static BoardMoveRoute GetValidFastTrackLocations(PlayerPiece piece)
        {
            BoardMoveRoute locations = new BoardMoveRoute();
            locations.StartingLocation = piece.CurrentLocation;
            locations.Direction = RouteDirection.Forward;
            BoardMoveLocation playerFt = BoardManager.GetPlayerFastTrackLocation(piece.Player);
            Int32 playerFtIndex = FastTrackLocationsSorted.BinarySearch(playerFt, new FastTrackLocationComparer());

            //Get sorted list starting with the players fast track location
            for (Int32 i = playerFtIndex; i <= FastTrackLocationsSorted.Count - 1; i++)
            {
                locations.Add(FastTrackLocationsSorted[i]);
            }
            if (playerFtIndex  > 0)
            {
                for (Int32 i = 0; i < playerFtIndex; i++)
                {
                    locations.Add(FastTrackLocationsSorted[i]);
                }
            }

            //Get rid of locations behind current and current
            Int32 currentIndex = Utility.SequentialSearch<BoardMoveLocation>(locations, piece.CurrentLocation);
            for (Int32 i = currentIndex; i >= 0; i--)
            {
                locations.RemoveAt(i);
            }            

            return locations;
        }

        public static BoardMoveRoute GetBestFastTrackLocations(PlayerPiece piece)
        {
            BoardMoveRoute locations = new BoardMoveRoute();
            BoardMoveLocation playerFt = BoardManager.GetPlayerFastTrackLocation(piece.Player);
            Int32 playerFtIndex = FastTrackLocationsSorted.BinarySearch(playerFt, new FastTrackLocationComparer());

            //Get sorted list starting with the players fast track location
            for (Int32 i = playerFtIndex; i <= FastTrackLocationsSorted.Count - 1; i++)
            {
                locations.Add(FastTrackLocationsSorted[i]);
            }
            if (playerFtIndex > 0)
            {
                for (Int32 i = 0; i < playerFtIndex; i++)
                {
                    locations.Add(FastTrackLocationsSorted[i]);
                }
            }

            //Sorts fast track locations starting with home fast track to decreasing fast track locations
            locations.Sort(new ReverseFastTrackLocationComparer());

            return locations;
        }

        public static BoardMoveLocation GetNextTrackLocation(BoardMoveLocation location, RouteDirection direction)
        {
            if (direction == RouteDirection.Forward)
            {
                if (location == TrackLocationsSorted[TrackLocationsSorted.Count - 1])
                    return TrackLocationsSorted[0];
                else
                    return TrackLocationsSorted[TrackLocationsSorted.BinarySearch(location, new BoardMoveLocationComparer()) + 1];
            }
            else if (direction == RouteDirection.Backward)
            {
                if (location == TrackLocationsSorted[0])
                    return TrackLocationsSorted[TrackLocationsSorted.Count - 1];
                else
                    return TrackLocationsSorted[TrackLocationsSorted.BinarySearch(location, new BoardMoveLocationComparer()) - 1];
            }

            return null;
        }

        public static BoardMoveRoute GetValidRegularTrackRouteFromLocation(BoardMoveLocation location, PlayerPiece piece, Int32 numMoves, Boolean partialRoute)
        {
            Boolean validRoute = true;
            Boolean backwards = (!partialRoute && numMoves == 4);
            BoardMoveRoute locations = new BoardMoveRoute();
            locations.StartingLocation = piece.CurrentLocation;
            locations.Direction = backwards ? RouteDirection.Backward : RouteDirection.Forward;
            Boolean onFinish = (!partialRoute && location.LocationType == LocationType.Finish && (Int32)location.LocationOwner == piece.PlayerNumber);

            for (Int32 numRegular = 1; numRegular <= numMoves; numRegular++)
            {
                //Check for occupied piece and invalidate route if trying to pass own piece
                BoardMoveLocation nextLocation = null;
                if (locations.Count == 0)
                    nextLocation = BoardManager.GetNextTrackLocation(location, locations.Direction);
                else
                    nextLocation = BoardManager.GetNextTrackLocation(locations[locations.Count - 1], locations.Direction);

                if (nextLocation.Occupied && nextLocation.OccupiedPlayerPiece.PlayerNumber == piece.PlayerNumber)
                    validRoute = false;
                else
                {
                    if (onFinish)
                    {
                        if (backwards)
                            locations.Add(nextLocation);
                        else
                            nextLocation = location;
                    }
                    else
                        locations.Add(nextLocation);
                }

                //If you hit your own finish location, branch into the home area
                if (!backwards &&
                    (nextLocation.LocationType == LocationType.Finish &&
                    (Int32)nextLocation.LocationOwner == piece.PlayerNumber))
                {
                    BoardMoveRoute homeLocations = BoardManager.GetPlayerHomeLocations(piece.Player);
                    Int32 movesLeft = (numMoves - locations.Count);
                    if (movesLeft > homeLocations.Count)
                        validRoute = false;
                    else
                    {
                        //Check to move into home location
                        for (Int32 homeCount = 1; homeCount <= movesLeft; homeCount++)
                        {
                            if (homeLocations[homeCount - 1].Occupied)
                            {
                                validRoute = false;
                                break;
                            }
                            else
                                locations.Add(homeLocations[homeCount - 1]);
                        }
                    }

                    break;
                }
            }

            if (!validRoute) return null;
            return locations;
        }

        public static Boolean ContainsFinish(PlayerPiece piece, Int32 routeLength)
        {
            Int32 currentIndex = BoardManager.GetTrackLocationIndex(piece.CurrentLocation);
            return BoardManager.TrackLocationsSorted.BinarySearch(currentIndex, routeLength, BoardManager.GetPlayerFinishLocation(piece.Player), new BoardMoveLocationComparer()) > -1;
        }
    }
}
