using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PPAsta.Abstraction.Models.Enums;
using System;

namespace PPAsta.Converters
{
    public class SellerProcessToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PaymentSellerProcess process)
            {
                return process switch
                {
                    PaymentSellerProcess.NotPaid => new SolidColorBrush(Microsoft.UI.Colors.Red),
                    PaymentSellerProcess.PaidByBuyer => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                    PaymentSellerProcess.PaidToSeller => new SolidColorBrush(Microsoft.UI.Colors.Green),
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
