using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Accounting.Converter
{

    public class GenderConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString()== Gender.Male.ToString())
                return "男";
            else if (value.ToString() == Gender.Female.ToString())
                return "女";
            else
                return "";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
