using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Studio.ViewModels.Utils
{
    public class RenderingProgressToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = (RenderingProgressState)value;
            switch (state)
            {
                case RenderingProgressState.NotStarted:
                    return null;
                case RenderingProgressState.Started:
                    return "pack://application:,,,/Resources/Icons/yinyan.png";
                case RenderingProgressState.Completed:
                    return "pack://application:,,,/Resources/Icons/tick.png";
                case RenderingProgressState.Failed:
                    return "pack://application:,,,/Resources/Icons/prohibition.png";
                case RenderingProgressState.Aborted:
                    return "pack://application:,,,/Resources/Icons/cross.png";
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
