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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeckOfCards
{
    public static class Utility
    {
        public static Image CreateImageOfElement(FrameworkElement element, Double? width, Double? height)
        {
            Image result = new Image();
            result.Source = new WriteableBitmap(element, element.RenderTransform);
            result.Width = width.HasValue ? width.Value : element.ActualWidth;
            result.Height = height.HasValue ? height.Value : element.ActualHeight;
            result.CacheMode = new BitmapCache();

            return result;
        }

        public static String GetImageResourceName(Card card)
        {
            String name = null;
            if (card.CardType == CardType.Joker)
            {
                if (card.CardColor == CardColor.Black) 
                    name = "joker1";
                else
                    name = "joker2";
            }
            else if (card.CardType == CardType.Regular)
            {
                switch (card.Suit)
                {
                    case CardSuit.Clubs:
                        name = "club";
                        break;
                    case CardSuit.Diamonds:
                        name = "dia";
                        break;
                    case CardSuit.Hearts:
                        name = "heart";
                        break;
                    case CardSuit.Spades:
                        name = "spade";
                        break;
                }

                switch (card.Rank)
                {
                    case CardRank.Ace:
                        name += "ace";
                        break;
                    case CardRank.King:
                        name += "king";
                        break;
                    case CardRank.Queen:
                        name += "queen";
                        break;
                    case CardRank.Jack:
                        name += "jack";
                        break;
                    case CardRank.Deuce:
                        name += "2";
                        break;
                    case CardRank.Three:
                        name += "3";
                        break;
                    case CardRank.Four:
                        name += "4";
                        break;
                    case CardRank.Five:
                        name += "5";
                        break;
                    case CardRank.Six:
                        name += "6";
                        break;
                    case CardRank.Seven:
                        name += "7";
                        break;
                    case CardRank.Eight:
                        name += "8";
                        break;
                    case CardRank.Nine:
                        name += "9";
                        break;
                    case CardRank.Ten:
                        name += "10";
                        break;
                }
            }

            return name;
        }
    }
}
