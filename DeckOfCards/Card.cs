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
using System.Windows.Controls;
using System.Collections.Generic;

namespace DeckOfCards
{
    public class Card : IComparable<Card>
    {
        private Deck _deck;

        public CardType CardType { get; set; }
        public CardColor CardColor { get; set; }
        public CardSuit Suit { get; set; }
        public CardRank Rank { get; set; }
        public Boolean FaceUp { get; set; }
        public Boolean IsAceBiggest { get; set; }
        public Image CardImage { get; set; }

        public Deck Deck
        {
            get { return _deck; }
            set
            {
                if (_deck == null)
                {
                    _deck = value;
                    _deck.Cards.Add(this);
                }
                else if (_deck != value)
                {
                    _deck.Cards.Remove(this);
                    _deck = value;
                    _deck.Cards.Add(this);
                }
            }
        }

        public Int32 NumericValue
        {
            get { return (Int32)this.Rank; }
        }

        public String StringValue
        {
            get 
            {
                switch (this.Rank)
                {
                    case CardRank.Ace:
                        return "A";
                    case CardRank.Jack:
                        return "J";
                    case CardRank.Queen:
                        return "Q";
                    case CardRank.King:
                        return "K";
                    default:
                        return this.NumericValue.ToString();
                }
            }
        }

        public Card()
        {
            this.FaceUp = false;
            this.IsAceBiggest = false;
        }

        public Card(CardType cardType, CardColor cardColor, CardSuit suit, CardRank rank)
        {
            this.FaceUp = false;
            this.IsAceBiggest = false;

            this.CardType = cardType;
            this.CardColor = cardColor;
            this.Suit = suit;
            this.Rank = rank;
        }

        public void Flip()
        {
            this.FaceUp = this.FaceUp ? false : true;
        }

        public Int32 CompareTo(Card other)
        {
            Int32 value1 = this.NumericValue;
            Int32 value2 = other.NumericValue;

            if (this.IsAceBiggest)
            {
                if (value1 == 1) value1 = 14;
                if (value2 == 1) value2 = 14;
            }

            if (value1 > value2)
                return 1;
            else if (value1 < value2)
                return -1;
            else
                return 0;
        }
        
        public override String ToString()
        {
            return this.NumericValue + " of " + this.Suit.ToString();
        }
    }

    public class CardComparer : IComparer<Card>
    {
        public Int32 Compare(Card x, Card y)
        {
            if (x.Suit != y.Suit)
            {
                if (x.Suit < y.Suit)
                    return 1;
                else 
                    return -1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }
    }
}
