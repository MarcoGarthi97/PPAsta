using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Abstraction.Models.Enums
{
    public enum PaymentGameProcess
    {
        Insert = 0,
        ToBePaid = 1,
        Paid = 2
    }

    public enum PaymentProcess
    {
        ToBePaid = 0,
        NotFullyPaid = 1,
        Paid = 2
    }

    public enum PaymentType
    {
        Cash = 0,
        Paypal = 1,
        BankTransfer = 2,
        Other = 3
    }
}
