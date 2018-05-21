using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using WiTrainingSuite.ViewModel.ConfigMenuItems;

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

    public class ConfigMenuTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Application win = Application.Current;

            if (item.GetType() == typeof(ConfigDeptViewModel))
                return win.FindResource("configDeptTemplate") as DataTemplate;

            if (item.GetType() == typeof(ConfigDocumentTagViewModel))
                return win.FindResource("configDocumentTagTemplate") as DataTemplate;

            if (item.GetType() == typeof(ConfigRoleViewModel))
                return win.FindResource("configRoleTemplate") as DataTemplate;

            if (item.GetType() == typeof(ConfigShiftViewModel))
                return win.FindResource("configShiftTemplate") as DataTemplate;

            if (item.GetType() == typeof(ConfigSkillViewModel))
                return win.FindResource("configSkillTemplate") as DataTemplate;

            if (item.GetType() == typeof(ConfigVariantViewModel))
                return win.FindResource("configVariantTemplate") as DataTemplate;

            return null;
        }
    }
}