using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PPAsta.Abstraction.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Converters
{
    public class PaymentGameProcessToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is PaymentGameProcess process)
            {
                return process switch
                {
                    PaymentGameProcess.Insert => new SolidColorBrush(Microsoft.UI.Colors.Gray),
                    PaymentGameProcess.ToBePaid => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                    PaymentGameProcess.Paid => new SolidColorBrush(Microsoft.UI.Colors.Green),
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
