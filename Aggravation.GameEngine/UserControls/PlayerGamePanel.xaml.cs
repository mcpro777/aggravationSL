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
using System.Windows.Media.Animation;
using Aggravation.Infrastructure.Constants;

namespace Aggravation.GameEngine.UserControls
{
    public partial class PlayerGamePanel : UserControl
    {
        public Player Player { get; set; }

        public event EventHandler<PlayerDrawsEventArgs> PlayerDraws;
        private void RaisePlayerDraws()
        {
            if (PlayerDraws != null) PlayerDraws.Invoke(this, new PlayerDrawsEventArgs(this.Player));
        }

        public event EventHandler<PlayerDrawCompletedEventArgs> PlayerDrawCompleted;
        private void RaisePlayerDrawCompleted()
        {
            if (PlayerDrawCompleted != null) PlayerDrawCompleted.Invoke(this, new PlayerDrawCompletedEventArgs(this.Player));
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
            "IsActive", typeof(Boolean),
            typeof(PlayerGamePanel), new PropertyMetadata(IsActivePropertyChanged)
            );

        private static void IsActivePropertyChanged(Object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PlayerGamePanel)sender).activeindicator.Visibility = (Boolean)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public Boolean IsActive
        {
            get { return (Boolean)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsDrawEnabledProperty =
            DependencyProperty.Register(
            "IsDrawEnabled", typeof(Boolean),
            typeof(PlayerGamePanel), new PropertyMetadata(IsDrawEnabledPropertyChanged)
            );

        private static void IsDrawEnabledPropertyChanged(Object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PlayerGamePanel)sender).btnDraw.IsEnabled = (Boolean)args.NewValue;
        }

        public Boolean IsDrawEnabled
        {
            get { return (Boolean)GetValue(IsDrawEnabledProperty); }
            set { SetValue(IsDrawEnabledProperty, value); }
        }
        
        public Panel CurrentDeckPanel
        {
            get { return this.spCurrentDeck; }
        }

        public Panel UsedDeckPanel
        {
            get { return this.spUsedDeck; }
        }

        public PlayerGamePanel()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PlayerGamePanel_Loaded);
        }

        private void PlayerGamePanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.activeindicator.Visibility = Visibility.Collapsed;
            this.btnDraw.Click += new RoutedEventHandler(btnDraw_Click);
            this.btnDraw.IsEnabled = false;
        }

        public void Initialize()
        {
            this.txtName.Text = "Player " + this.Player.Number.ToString() + ":  " + this.Player.Name;

            this.computer.Visibility = System.Windows.Visibility.Collapsed;
            this.male.Visibility = System.Windows.Visibility.Collapsed;
            this.female.Visibility = System.Windows.Visibility.Collapsed;
            if (this.Player is ComputerPlayer)
                this.computer.Visibility = System.Windows.Visibility.Visible;
            else
            {
                if (this.Player.Gender == Gender.Male)
                    this.male.Visibility = System.Windows.Visibility.Visible;
                else
                    this.female.Visibility = System.Windows.Visibility.Visible;
            }

            this.SetInitialDecks();
        }

        public void SetInitialDecks()
        {
            this.spCurrentDeck.Children.Clear();
            this.spUsedDeck.Children.Clear();

            this.spCurrentDeck.Children.Add(this.Player.CurrentDeck.CardBackImage);

            if (this.Player.UsedDeck.DeckEmptyImage != null)
                this.spUsedDeck.Children.Add(this.Player.CurrentDeck.DeckEmptyImage);
        }

        public void SetCurrentEmpty()
        {
            this.spCurrentDeck.Children.Clear();

            if (this.Player.CurrentDeck.DeckEmptyImage != null)
                this.spCurrentDeck.Children.Add(this.Player.CurrentDeck.DeckEmptyImage);
        }

        public void DrawCard()
        {
            Boolean forward = false;
            Image cardImage = this.Player.UsedDeck.TopCard.CardImage;
            Image cardBack = this.Player.CurrentDeck.CardBackImage;

            Image cardBack2 = DeckOfCards.Utility.CreateImageOfElement(cardBack, null, null);
            this.spCurrentDeck.Children.Add(cardBack2);

            cardImage.Visibility = System.Windows.Visibility.Collapsed;

            this.spCurrentDeck.Children.Add(cardImage);

            Storyboard frontToBackStoryboard = new Storyboard();
            Storyboard backToFrontStoryboard = new Storyboard();

            Duration d = new Duration(new TimeSpan(0, 0, 0, 0, Constants.CardFlipDuration));
            Behaviors.Swivel.InvokeSwivel(frontToBackStoryboard, backToFrontStoryboard, cardImage, cardBack2, Behaviors.RotationDirection.LeftToRight, d, ref forward);
            this.MoveX(cardImage, 76);
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            this.RaisePlayerDraws();
        }

        public void DrawComputer()
        {
            this.RaisePlayerDraws();
        }

        public void SetToken(PlayerPiece piece)
        {
            this.tokenPanel.Children.Add(DeckOfCards.Utility.CreateImageOfElement(piece.VisualElement, this.tokenPanel.Width, this.tokenPanel.Height));
        }

        private void MoveX(FrameworkElement element, Double xNum)
        {
            TransformGroup tg = new TransformGroup();
            TranslateTransform tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            tg.Children.Add(tt);
            element.RenderTransform = tg;

            DoubleAnimation daLeft = new DoubleAnimation();
            daLeft.To = xNum;
            daLeft.Duration = new Duration(new TimeSpan(0, 0, 0, 0, Constants.CardAnimationDelay));
            Storyboard.SetTarget(daLeft, element);
            Storyboard.SetTargetProperty(daLeft, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

            Storyboard sbMoveCardToDeck = new Storyboard();
            sbMoveCardToDeck.Children.Add(daLeft);
            sbMoveCardToDeck.Completed += new EventHandler(sbMoveCardToDeck_Completed);
            sbMoveCardToDeck.Begin();
        }

        protected void sbMoveCardToDeck_Completed(object sender, EventArgs e)
        {
            this.RaisePlayerDrawCompleted();   
        }
    }
}
