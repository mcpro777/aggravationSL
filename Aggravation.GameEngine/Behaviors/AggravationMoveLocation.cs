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
using System.Windows.Interactivity;
using System.ComponentModel;

namespace Aggravation.GameEngine.Behaviors
{
#if !SILVERLIGHT && (BLEND3 || BLEND4)
	[Microsoft.Windows.Design.ToolboxCategory("Aggravation")]
#endif
    /// <summary>
    /// Represents a location a player piece can move to
    /// </summary>
    [Category("Aggravation")]
    [Description("Represents a location a player piece can move to")]
    public class AggravationMoveLocation : Behavior<FrameworkElement>
    {
        private BoardMoveLocation _boardMoveLocation = new BoardMoveLocation();

        public BoardMoveLocation BoardMoveLocation
        {
            get { return _boardMoveLocation; }
        }

        [Category("Aggravation")]
        [Description("Location Type")]
        public LocationType LocationType
        {
            get { return this.BoardMoveLocation.LocationType; }
            set { this.BoardMoveLocation.LocationType = value; }
        }

        [Category("Aggravation")]
        [Description("Location Number")]
        public Int32 LocationNumber
        {
            get { return this.BoardMoveLocation.LocationNumber; }
            set { this.BoardMoveLocation.LocationNumber = value; }
        }

        [Category("Aggravation")]
        [Description("Location Owner")]
        public LocationOwner LocationOwner
        {
            get { return this.BoardMoveLocation.LocationOwner; }
            set { this.BoardMoveLocation.LocationOwner = value; }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.BoardMoveLocation.VisualElement = this.AssociatedObject;
            this.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.BoardMoveLocation.SetValue(BoardManager.IsLocationProperty, false);
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
            this.BoardMoveLocation.SetValue(BoardManager.IsLocationProperty, true);
        }
    }
}
