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

namespace Aggravation.GameEngine
{
    public class BoardMoveLocationComparer : IComparer<BoardMoveLocation>
    {
        public Int32 Compare(BoardMoveLocation x, BoardMoveLocation y)
        {
            if (x.LocationType == LocationType.Base ||
                x.LocationType == LocationType.SuperFastTrack ||
                y.LocationType == LocationType.Base ||
                y.LocationType == LocationType.SuperFastTrack)
            {
                return 0;
            }
            else
            {
                return x.CompareTo(y);
            }
        }
    }

    public class FastTrackLocationComparer : IComparer<BoardMoveLocation>
    {
        public Int32 Compare(BoardMoveLocation x, BoardMoveLocation y)
        {
            if (x.LocationType == LocationType.FastTrack &&
                y.LocationType == LocationType.FastTrack)
            {
                return x.CompareTo(y);
            }
            else
            {
                return 0;
            }
        }
    }

    public class ReverseFastTrackLocationComparer : IComparer<BoardMoveLocation>
    {
        public Int32 Compare(BoardMoveLocation x, BoardMoveLocation y)
        {
            if (x.LocationType == LocationType.FastTrack &&
                y.LocationType == LocationType.FastTrack)
            {
                return x.CompareToReverse(y);
            }
            else
            {
                return 0;
            }
        }
    }
}
