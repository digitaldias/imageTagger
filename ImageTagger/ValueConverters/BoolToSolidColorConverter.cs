using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageTagger.ValueConverters
{
    public class BoolToSolidColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool) value;
            if(boolValue)
            {
                return new SolidColorBrush(Colors.DeepSkyBlue);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
