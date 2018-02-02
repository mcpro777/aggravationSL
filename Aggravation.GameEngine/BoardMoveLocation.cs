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
using System.Windows;
using System.Collections.Generic;

namespace Aggravation.GameEngine
{
    public class BoardMoveRoutes : List<BoardMoveRoute>
    {
    }

    public class BoardMoveRoute : List<BoardMoveLocation>
    {
        public RouteDirection Direction { get; set; }
        public BoardMoveLocation StartingLocation { get; set; }

        public BoardMoveRoute() : base()
        {
        }

        public BoardMoveRoute(BoardMoveLocation startingLocation) : base()
        {
            this.StartingLocation = startingLocation;
        }
    }

    public class BoardMoveLocation : DependencyObject
    {
        public LocationType LocationType { get; set; }
        public Boolean Occupied { get; set; }
        public PlayerPiece OccupiedPlayerPiece { get; set; }
        public Int32 LocationNumber { get; set; }
        public LocationOwner LocationOwner { get; set; }
        public FrameworkElement VisualElement { get; set; }

        public BoardMoveLocation()
        {
        }

        public Int32 CompareTo(BoardMoveLocation other)
        {
            Int32 value1 = this.LocationNumber;
            Int32 value2 = other.LocationNumber;

            if (value1 > value2)
                return 1;
            else if (value1 < value2)
                return -1;
            else
                return 0;
        }

        public Int32 CompareToReverse(BoardMoveLocation other)
        {
            Int32 value1 = this.LocationNumber;
            Int32 value2 = other.LocationNumber;

            if (value1 < value2)
                return 1;
            else if (value1 > value2)
                return -1;
            else
                return 0;
        }

        public override string ToString()
        {
            return "Location " + this.LocationNumber.ToString() + ", Type " + this.LocationType.ToString();
        }
    }
}
