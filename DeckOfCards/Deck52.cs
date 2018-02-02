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
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace DeckOfCards
{
    public class Deck52 : Deck
    {
        private Int32 _maxCards;
        private List<Card> _cards;

        public Boolean IsAceBiggest { get; set; }
        public Image CardBackImage { get; set; }
        public Image DeckEmptyImage { get; set; }
        public Transform CardTransform { get; set; }
        public Double? CardWidth { get; set; }
        public Double? CardHeight { get; set; }

        public override Int32 MaxCards
        {
            get { return _maxCards; }
        }

        public override List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }

        public Card TopCard
        {
            get 
            {
                return this.Cards.Count > 0 ? this.Cards[this.Cards.Count - 1] : null;
            }
        }

        public Card BottomCard
        {
            get 
            {
                return this.Cards.Count > 0 ? this.Cards[0] : null;
            }
        }

        public Int32 NumCards
        {
            get { return this.Cards.Count; }
        }

        public Boolean HasCards
        {
            get { return this.Cards.Count > 0 ? true : false; }
        }

        public Deck52(Boolean isAceBiggest)
        {
            this._cards = new List<Card>(this.GetMaxCardsNum());
            this.IsAceBiggest = isAceBiggest;
            this._maxCards = this.GetMaxCardsNum();
            this.Deal();
        }

        public Deck52(Boolean isAceBiggest, Boolean shouldShuffle)
        {
            this._cards = new List<Card>(this.GetMaxCardsNum());
            this.IsAceBiggest = isAceBiggest;
            this._maxCards = this.GetMaxCardsNum();
            this.Deal();
            if (shouldShuffle) this.Shuffle();
        }

        public Deck52(Boolean isAceBiggest, Boolean shouldShuffle, Transform transform, Double? width, Double? height)
        {
            this.CardTransform = transform;
            this.CardWidth = width;
            this.CardHeight = height;
            this._cards = new List<Card>(this.GetMaxCardsNum());
            this.IsAceBiggest = isAceBiggest;
            this._maxCards = this.GetMaxCardsNum();
            this.Deal();
            if (shouldShuffle) this.Shuffle();
        }

        public override void Deal()
        {
            this.RemoveAllCards();

            //GraphicResources gr = CardResourceAccessor.Instance.GraphicResources;
            //this.CardBackImage = Utility.CreateImageOfElement(gr.FindName("cardback") as FrameworkElement, this.CardWidth, this.CardHeight);
            //this.CardBackImage = CardResourceAccessor.GetImage("cardback", this.CardWidth, this.CardHeight);
            this.CardBackImage = Utility.CreateImageOfElement(CardResourceAccessor.GetImage("cardback", this.CardWidth, this.CardHeight), this.CardWidth, this.CardHeight);

            if (this.CardTransform != null) this.CardBackImage.RenderTransform = this.CardTransform;

            for (Int32 suitCount = 1; suitCount <= 4; suitCount++)
            {
                CardSuit suit = (CardSuit)suitCount;
                CardColor cardColor = (suit == CardSuit.Diamonds || suit == CardSuit.Hearts) ? CardColor.Red : CardColor.Black;
                for (Int32 rankCount = 1; rankCount <= 13; rankCount++)
                {
                    Card card = new Card(CardType.Regular, cardColor, suit, (CardRank)rankCount);
                    card.IsAceBiggest = this.IsAceBiggest;
                    //card.CardImage = Utility.CreateImageOfElement(gr.FindName(Utility.GetImageResourceName(card)) as FrameworkElement, this.CardWidth, this.CardHeight);
                    card.CardImage = CardResourceAccessor.GetImage(card, this.CardWidth, this.CardHeight);
                    if (this.CardTransform != null) card.CardImage.RenderTransform = this.CardTransform;
                    card.Deck = this;
                }
            }
        }

        public void DrawCard(Int32 numCardsToDraw, Deck toDeck)
        {
            for (Int32 i = 0; i < numCardsToDraw; i++)
            {
                this.MoveCard(this.TopCard, toDeck);
            }
        }

        public void Shuffle()
        {
            this.Shuffle(1);
        }

        public void Shuffle(Int32 numTimes)
        {
            Random rand = new Random();
            for (Int32 count = 0; count < numTimes; count++)
            {
                for (Int32 i = 0; i < this.Cards.Count; i++)
                {
                    Card card = this.Cards[i];
                    this.Cards.Remove(card);
                    this.Cards.Insert(rand.Next(0, this.Cards.Count), card);
                }
            }
        }

        public void AddCard(Card card)
        {
            this.Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            this.Cards.Remove(card);
        }

        public void RemoveAllCards()
        {
            foreach (var card in this.Cards)
            {
                card.CardImage = null;
            }

            this.Cards.Clear();
        }

        public Card GetCard(Int32 number, CardSuit suit)
        {
            if (number == 14) number = 1;
            return GetCard((CardRank)number, suit);
        }

        public Card GetCard(CardRank rank, CardSuit suit)
        {
            return this.Cards.FirstOrDefault(card => (card.Rank == rank) && (card.Suit == suit));
        }

        public Boolean HasCard(Int32 number, CardSuit suit)
        {
            if (number == 14) number = 1;
            return HasCard((CardRank)number, suit);
        }

        public Boolean HasCard(CardRank rank, CardSuit suit)
        {
            return GetCard(rank, suit) != null;
        }

        public void MoveCard(Card card, Deck toDeck)
        {
            card.Deck = toDeck;
        }

        protected virtual Int32 GetMaxCardsNum()
        {
            return 52;
        }

        public void Sort()
        {
            CardComparer cardComparer = new CardComparer();
            this.Cards.Sort(cardComparer);
        }
    }
}
