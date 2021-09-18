namespace TimeStampNote.Models
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return string.Format("{0:00000}", (long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}