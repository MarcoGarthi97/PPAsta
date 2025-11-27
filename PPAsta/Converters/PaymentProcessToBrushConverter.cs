using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PPAsta.Abstraction.Models.Enums;
using System;

namespace PPAsta.Converters
{
    public class PaymentProcessToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PaymentProcess process)
            {
                return process switch
                {
                    PaymentProcess.ToBePaid => new SolidColorBrush(Microsoft.UI.Colors.Red),
                    PaymentProcess.NotFullyPaid => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                    PaymentProcess.Paid => new SolidColorBrush(Microsoft.UI.Colors.Green),
                    _ => new SolidColorBrush(Microsoft.UI.Colors.Gray)
                };
            }

            return new SolidColorBrush(Microsoft.UI.Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
