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
using DeckOfCards;
using Aggravation.GameEngine.UserControls;

namespace Aggravation.GameEngine
{
    public class Players : List<Player>
    {
    }

    public class Player
    {
        public String Name { get; set; }
        public Int32 Number { get; set; }
        public Deck52Plus CurrentDeck { get; set; }
        public Deck52Plus UsedDeck { get; set; }
        public List<PlayerPiece> Pieces { get; set; }
        public Boolean IsFinished { get; set; }
        public Boolean StartsFirst { get; set; }
        public PlayerGamePanel GamePanel { get; set; }
        public Gender Gender { get; set; }

        public Player()
        {
            this.Pieces = new List<PlayerPiece>();
            this.IsFinished = false;
            this.StartsFirst = false;
        }

        public Player(String name, Int32 number)
        {
            this.Pieces = new List<PlayerPiece>();
            this.IsFinished = false;
            this.StartsFirst = false;
            this.Name = name;
            this.Number = number;
        }

        public Player(String name, Int32 number, Deck52Plus currentDeck, Deck52Plus usedDeck)
        {
            this.Pieces = new List<PlayerPiece>();
            this.IsFinished = false;
            this.StartsFirst = false;
            this.Name = name;
            this.Number = number;
            this.CurrentDeck = currentDeck;
            this.UsedDeck = usedDeck;
        }

        public Int32 CompareTo(Player other)
        {
            Int32 value1 = this.Number;
            Int32 value2 = other.Number;

            if (value1 > value2)
                return 1;
            else if (value1 < value2)
                return -1;
            else
                return 0;
        }
    }

    public class PlayerComparer : IComparer<Player>
    {
        public Int32 Compare(Player x, Player y)
        {
            return x.CompareTo(y);
        }
    }
}
