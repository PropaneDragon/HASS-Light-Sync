using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightSync.UI.Controls
{
    internal class NormalisedPanel : Panel
    {
        public static readonly AttachedProperty<double> XProperty = AvaloniaProperty.RegisterAttached<NormalisedPanel, Control, double>("X");
        public static readonly AttachedProperty<double> YProperty = AvaloniaProperty.RegisterAttached<NormalisedPanel, Control, double>("Y");

        public static double GetX(Control c) => c.GetValue(XProperty);
        public static void SetX(Control c, double v) => c.SetValue(XProperty, v);
        public static double GetY(Control c) => c.GetValue(YProperty);
        public static void SetY(Control c, double v) => c.SetValue(YProperty, v);

        static NormalisedPanel()
        {
            AffectsParentArrange<NormalisedPanel>(XProperty, YProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
                child.Measure(Size.Infinity);

            return default;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
            {
                var size = child.DesiredSize;
                var x = GetX(child) * finalSize.Width - size.Width / 2;
                var y = GetY(child) * finalSize.Height - size.Height / 2;

                child.Arrange(new Rect(new Point(x, y), size));
            }

            return finalSize;
        }
    }
}
