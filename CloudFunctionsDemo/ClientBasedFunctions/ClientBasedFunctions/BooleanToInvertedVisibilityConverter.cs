// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SEALAzureFuncClient
{
    /// <summary>
    /// Converter that inverts the boolean value received
    /// </summary>
    public class BooleanToInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bval)
            {
                if (bval)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
