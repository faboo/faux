using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Project
{
    public static class UIElementExtension
    {

        public static TObject FindVisualParent<TObject>(this IInputElement child, Func<TObject, bool> test) where TObject : UIElement
        {
            if (child is UIElement)
                return FindVisualParent<TObject>(child as UIElement, test);
            else
                return null;
        }

        public static TObject FindVisualParent<TObject>(this UIElement child, Func<TObject, bool> test) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = child as UIElement; // VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null && test(found))
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }

        public static TObject FindVisualChild<TObject>(this UIElement parent) where TObject : UIElement
        {
            int children = 0;

            if (parent == null)
            {
                return null;
            }

            children = VisualTreeHelper.GetChildrenCount(parent);

            for (int idx = 0; idx < children; ++idx)
            {
                UIElement child = VisualTreeHelper.GetChild(parent, idx) as UIElement;

                TObject found = child as TObject;

                if (found != null)
                    return found;
                else if ((found = FindVisualChild<TObject>(child)) != null)
                    return found;
            }

            return null;
        }
    }
}
