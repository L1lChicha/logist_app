using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Infrastructure.Service
{
    public class IndexConverterService : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Мы передали CollectionView через x:Reference, ловим его здесь
            if (parameter is CollectionView collectionView)
            {
                var list = collectionView.ItemsSource as System.Collections.IList;
                if (list != null && value != null)
                {
                    // +1 чтобы нумерация шла с 1, а не с 0
                    return list.IndexOf(value) + 1;
                }
            }
            return 0; // Если не нашли или ошибка
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
