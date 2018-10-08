using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Sunghoon_App.Converter
{
    public class DateToString :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // The value parameter is the data from the source object.
            DateTime date = (DateTime)value;
            return date.ToString("yymmdd H:mm");
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
