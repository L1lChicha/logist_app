using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                // Return string with current culture (comma or dot)
                return d.ToString(CultureInfo.CurrentCulture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                // Try parsing with current culture (allows comma if culture is RU/DE etc)
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out double result))
                {
                    return result;
                }
                // Fallback: replace comma with dot and try again (for mixed scenarios)
                if (double.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double resultInvariant))
                {
                    return resultInvariant;
                }
            }
            return 0.0; // Or keep old value
        }
    }
}
