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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Aggravation.Game.Ui
{
    public partial class NewGameWindow : UserControl
    {
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

        public NewGameWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(NewGameWindow_Loaded);
        }

        private void NewGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnStart.Click += new RoutedEventHandler(btnStart_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
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
                {
                    var set = false;
                    while (!set)
                    {
                        startNum = rand.Next(1, 6);
                        if (playerPrompts.Any(p => p.PlayerNumber == startNum)) set = true;
                    }
                }
            }

            if (!playerPrompts.Any(p => p.PlayerNumber == startNum)) startNum = playerPrompts.First().PlayerNumber;
            this.RaiseStart(startNum, playerPrompts);
        }

        public void ShowCancel()
        {
            this.btnCancel.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideCancel()
        {
            this.btnCancel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
