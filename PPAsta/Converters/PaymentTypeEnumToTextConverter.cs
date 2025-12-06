using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PPAsta.Abstraction.Models.Enums;
using System;

namespace PPAsta.Converters
{
    public class PaymentTypeEnumToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PaymentType process)
            {
                return process switch
                {
                    PaymentType.Cash => "Contanti",
                    PaymentType.Paypal => "Paypal",
                    PaymentType.BankTransfer => "Bonifico",
                    PaymentType.Other => "Altro",
                    _ => ""
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
