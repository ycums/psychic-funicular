using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace Pg01.Views.Converters
{
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var aggregate = this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
            Debug.WriteLine(aggregate);
            return aggregate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}