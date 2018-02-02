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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aggravation.GameEngine
{
    public static class Utility
    {
        public static Int32 SequentialSearch<T>(List<T> list, T itemToFind)
        {
            for (Int32 i = 0; i < list.Count; i++)
            {
                Object temp = list[i];
                Object temp2 = itemToFind;
                if (temp == temp2) return i;
            }

            return -1;
        }

        public static Point GetSnapPoint(FrameworkElement moveObject, FrameworkElement locationObject)
        {
            Double x = (Double)locationObject.GetValue(Canvas.LeftProperty) - (moveObject.Width / 2);
            Double y = (Double)locationObject.GetValue(Canvas.TopProperty) - (moveObject.Height / 2);
            x += locationObject.Width / 2;
            y += locationObject.Height / 2;
            return new Point(x, y);
        }

        public static Image CreateImageOfElement(FrameworkElement element)
        {
            Image result = new Image();
            result.Source = new WriteableBitmap(element, element.RenderTransform);
            result.Width = element.ActualWidth;
            result.Height = element.ActualHeight;
            result.CacheMode = new BitmapCache();

            return result;
        }

        public static T Clone<T>(T source)
        {
            T cloned = (T)Activator.CreateInstance(source.GetType());

            foreach (var curPropInfo in source.GetType().GetProperties()
                        .TakeWhile(curPropInfo => curPropInfo.Name != "Name" && curPropInfo.Name != "Source")
                                .Where(curPropInfo => curPropInfo.GetGetMethod() != null && (curPropInfo.GetSetMethod() != null)))
            {
                // Handle Non-indexer properties
                if (curPropInfo.Name != "Item")
                {
                    // get property from source
                    object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { });

                    // clone if needed
                    if (getValue != null && getValue is DependencyObject)
                        getValue = Clone((DependencyObject)getValue);

                    // set property on cloned
                    if (getValue != null)
                    {
                        curPropInfo.GetSetMethod().Invoke(cloned, new object[] { getValue });
                    }
                }
                    // handle indexer
                else
                {
                    // get count for indexer
                    int numberofItemInColleciton =
                        (int)
                        curPropInfo.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(source, new object[] { });

                    // run on indexer
                    for (int i = 0; i < numberofItemInColleciton; i++)
                    {
                        // get item through Indexer
                        object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { i });

                        // clone if needed
                        if (getValue != null && getValue is DependencyObject)
                            getValue = Clone((DependencyObject)getValue);
                        // add item to collection
                        curPropInfo.ReflectedType.GetMethod("Add").Invoke(cloned, new object[] { getValue });
                    }
                }
            }

            return cloned;
        }

        public static Boolean TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            Boolean result = !dictionary.ContainsKey(key);

            if (result)
            {
                dictionary.Add(key, value);
            }

            return result;
        }

        public static FrameworkElement CreateHighlightOverlay()
        {
            Ellipse overlay = new Ellipse();
            overlay.Height = 32.0;
            overlay.Width = 32.0;
            RadialGradientBrush rgb = new RadialGradientBrush();
            rgb.RadiusX = 0.75199997425079346;
            rgb.RadiusY = 0.75199997425079346;
            GradientStop gs1 = new GradientStop();
            Color c1 = new Color();
            c1.A = 255;
            c1.R = 175;
            c1.G = 177;
            c1.B = 30;
            gs1.Color = c1;
            gs1.Offset = 0.983;
            GradientStop gs2 = new GradientStop();
            Color c2 = new Color();
            c2.A = 233;
            c2.R = 233;
            c2.G = 145;
            c2.B = 30;
            gs2.Color = c2;
            rgb.GradientStops.Add(gs1);
            rgb.GradientStops.Add(gs2);
            overlay.Fill = rgb;
            return overlay;
        }

        public static RadialGradientBrush CreateFastTrackBrush()
        {
            Color c1 = new Color();
            c1.A = 255;
            c1.R = 212;
            c1.G = 212;
            c1.B = 212;
            Color c2 = new Color();
            c2.A = 255;
            c2.R = 0;
            c2.G = 19;
            c2.B = 65;
            GradientStopCollection gsc = new GradientStopCollection();
            GradientStop gs = new GradientStop();
            gs.Color = c1;
            gsc.Add(gs);
            GradientStop gs2 = new GradientStop();
            gs2.Color = c2;
            gsc.Add(gs2);
            gs2.Offset = 1;
            RadialGradientBrush b = new RadialGradientBrush(gsc);
            return b;
        }
    }
}
