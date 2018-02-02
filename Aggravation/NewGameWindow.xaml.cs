using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Aggravation
{
    public partial class NewGameWindow : UserControl
    {
        private Panel parent = null;

        public event EventHandler<EventArgs> Cancel;
        private void RaiseCancel()
        {
            if (Cancel != null) Cancel.Invoke(this, new EventArgs());
        }

        public event EventHandler<NewGameWindowStartEventArgs> Start;
        private void RaiseStart(Int32 startPlayerNum, List<NewPlayerPrompt> playerPrompts)
        {
            if (Start != null) Start.Invoke(this, new NewGameWindowStartEventArgs(startPlayerNum, playerPrompts));
        }

        public NewGameWindow(FrameworkElement parent)
        {
            InitializeComponent();
        }

        public NewGameWindow(Panel parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.Loaded += new RoutedEventHandler(NewGameWindow_Loaded);            
        }

        private void NewGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnStart.Click += new RoutedEventHandler(btnStart_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.RaiseCancel();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Int32 startNum = 0;
            List<NewPlayerPrompt> playerPrompts = new List<NewPlayerPrompt>();

            if ((Boolean)this.rb1.IsChecked) startNum = 1;
            if ((Boolean)this.rb2.IsChecked) startNum = 2;
            if ((Boolean)this.rb3.IsChecked) startNum = 3;
            if ((Boolean)this.rb4.IsChecked) startNum = 4;
            if ((Boolean)this.rb5.IsChecked) startNum = 5;
            if ((Boolean)this.rb6.IsChecked) startNum = 6;

            foreach (var element in this.LayoutRoot.Children)
            {
                if (element is NewPlayerPrompt)
                {
                    NewPlayerPrompt prompt = element as NewPlayerPrompt;
                    if (prompt.IsActivated) playerPrompts.Add(prompt);
                }
            }

            if ((Boolean)this.rbRandom.IsChecked)
            {
                Random rand = new Random(DateTime.Now.Millisecond);
                if (playerPrompts.Count == 1)
                    startNum = 0;
                else
                    startNum = rand.Next(0, playerPrompts.Count - 1);
            }

            this.Hide();
            this.RaiseStart(playerPrompts[startNum].PlayerNumber, playerPrompts);
        }

        public void Show(Boolean hideCancel)
        {
            this.btnCancel.IsEnabled = !hideCancel;
            if (!this.parent.Children.Contains(this)) this.parent.Children.Add(this);
            this.Visibility = System.Windows.Visibility.Visible;
            
            Point movePoint = GameEngine.Utility.GetSnapPoint(this, this.parent);
            this.SetValue(Canvas.LeftProperty, movePoint.X);
            this.SetValue(Canvas.TopProperty, movePoint.Y);
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
