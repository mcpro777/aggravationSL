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
using System.Windows.Media;

namespace DeckOfCards
{
    public class Deck52Plus : Deck52
    {
        public Deck52Plus(Boolean isAceBiggest) : base(isAceBiggest)
        {
        }

        public Deck52Plus(Boolean isAceBiggest, Boolean shouldShuffle) : base(isAceBiggest, shouldShuffle)
        {
            if (shouldShuffle) this.Shuffle();
        }

        public Deck52Plus(Boolean isAceBiggest, Boolean shouldShuffle, Transform transform, Double? width, Double? height) 
            : base(isAceBiggest, shouldShuffle, transform, width, height)
        {
            if (shouldShuffle) this.Shuffle();
        }

        protected override Int32 GetMaxCardsNum()
        {
            return base.GetMaxCardsNum() + 2;
        }

        public override void Deal()
        {
            base.Deal();
            this.AddJokers();
        }

        protected void AddJokers()
        {
            GraphicResources gr = CardResourceAccessor.Instance.GraphicResources;

            for (Int32 i = 0; i <= 1; i++)
            {
                CardColor cardColor = CardColor.Black;
                if (i == 1) cardColor = CardColor.Red;
                Card card = new Card(CardType.Joker, cardColor, CardSuit.None, CardRank.None);
                card.IsAceBiggest = this.IsAceBiggest;
                card.CardImage = CardResourceAccessor.GetImage(card, this.CardWidth, this.CardHeight);
                //Utility.CreateImageOfElement(gr.FindName(Utility.GetImageResourceName(card)) as FrameworkElement, this.CardWidth, this.CardHeight);
                if (this.CardTransform != null) card.CardImage.RenderTransform = this.CardTransform;
                card.Deck = this;
            }
        }
    }
}
