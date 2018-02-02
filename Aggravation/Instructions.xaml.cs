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
    public partial class Instructions : UserControl
    {
        private Panel parent = null;

        public event EventHandler<EventArgs> Cancel;
        private void RaiseCancel()
        {
            if (Cancel != null) Cancel.Invoke(this, new EventArgs());
        }

        public Instructions(Panel parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.Loaded += new RoutedEventHandler(Instructions_Loaded);
        }

        private void Instructions_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.RaiseCancel();
        }

        public void Show()
        {
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
