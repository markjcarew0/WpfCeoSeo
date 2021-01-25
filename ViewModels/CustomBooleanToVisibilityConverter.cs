// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomBooleanToVisibilityConverter.cs" company="MarkJC">
//   Author Mark Carew
// </copyright>
// <summary>
//   Defines the CustomBooleanToVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CeoSeoViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// custom bool to visibility converter
    /// </summary>
    /// <remarks>
    /// returns visibility depending a boolean binding
    /// </remarks>
    public sealed class CustomBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the default hide method.
        /// </summary>
        public Visibility DefaultHideMethod { get; set; }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibilityState = value != null && (bool)value;
            if (visibilityState)
            {
                return Visibility.Visible;
            }

            return DefaultHideMethod == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Hidden;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// the usual
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}