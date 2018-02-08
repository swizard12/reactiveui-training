using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Collections;
using System.Windows.Controls;

namespace WiTrainingSuite.View
{
    public class FullNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string fName = values[0] as string;
            string lName = values[1] as string;

            if (!string.IsNullOrEmpty(fName) && !string.IsNullOrEmpty(lName))
            {
                return $"{fName} {lName}";
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}