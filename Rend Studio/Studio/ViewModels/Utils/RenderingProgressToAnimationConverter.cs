using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Studio.ViewModels.Utils
{
    public class RenderingProgressToAnimationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = (RenderingProgressState)value;
            switch (state)
            {
                case RenderingProgressState.Started:
                    return CreateRotationAnimation();
                default:
                    return null;
            }
        }

        private RotateTransform CreateRotationAnimation()
        {
            var animation = new DoubleAnimation(0, 360, TimeSpan.FromMilliseconds(300))
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = false
            };
            var transform = new RotateTransform(0, 12, 12);

            transform.BeginAnimation(RotateTransform.AngleProperty, animation);

            return transform;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
