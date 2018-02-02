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

namespace DeckOfCards
{
    /// <summary>
    /// Singleton class to get card graphic resources
    /// </summary>
    public sealed class CardResourceAccessor
    {
        private GraphicResources _graphicResources = null;
        private System.Collections.Generic.Dictionary<String, Image> elements = new System.Collections.Generic.Dictionary<String, Image>();
        private static Object syncRoot = new Object();

        private static volatile CardResourceAccessor _instance;

        public static CardResourceAccessor Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null) _instance = new CardResourceAccessor();
                    }
                }

                return _instance; 
            }
        }

        public GraphicResources GraphicResources
        {
            get 
            { 
                return _graphicResources; 
            }
        }

        private CardResourceAccessor()
        {
            _graphicResources = new GraphicResources();
        }

        public static Image GetImage(Card card, Double? width, Double? height)
        {
            String cardResourceName = Utility.GetImageResourceName(card);
            String objectKey = cardResourceName + width.ToString() + height.ToString(); //cache key
            return GetImageFromName(objectKey, cardResourceName, width, height);
        }

        public static Image GetImage(String name, Double? width, Double? height)
        {
            String objectKey = name + width.ToString() + height.ToString(); //cache key
            return GetImageFromName(objectKey, name, width, height);
        }

        private static Image GetImageFromName(String objectKey, String cardResourceName, Double? width, Double? height)
        {
            if (!Instance.elements.ContainsKey(objectKey))
            {
                var element = Utility.CreateImageOfElement(Instance.GraphicResources.FindName(cardResourceName) as FrameworkElement, width, height);
                Instance.elements.Add(objectKey, element);  //Cache image so it doesn't have to be created from scratch every time (way faster)
                Image image = Utility.CreateImageOfElement(element, width, height);     //Copy new image from template
                return image;
            }
            else
            {
                //Get cached item
                var element = Instance.elements[objectKey] as Image;
                Image image = Utility.CreateImageOfElement(element, width, height);     //Copy new image from template
                return image;
            }
        }
    }
}
