using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Buyer;
using PPAsta.Service.Services.PP.PaymentGame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class PaymentGameViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvPaymentGameService _paymentGameService;
        private readonly ISrvBuyerService _buyerService;

        public PaymentGameViewModel(ISrvPaymentGameService paymentGameService, ISrvBuyerService buyerService)
        {
            _paymentGameService = paymentGameService;
            _buyerService = buyerService;
        }

        private string _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name!, value);
        }

        private string _owner;
        public string? Owner
        {
            get => _owner;
            set => SetProperty(ref _owner!, value);
        }

        private decimal? _sellingPrice;
        public decimal? SellingPrice
        {
            get => _sellingPrice;
            set => SetProperty(ref _sellingPrice!, value);
        }

        public string _buyerName;
        public string? BuyerName
        {
            get => _buyerName;
            set => SetProperty(ref _buyerName!, value);
        }

        public string _number;
        public string? Number
        {
            get => _number;
            set => SetProperty(ref _number!, value);
        }

        public string _purchasePrice;
        public string? PurchasePrice
        {
            get => _purchasePrice;
            set => SetProperty(ref _purchasePrice!, value);
        }

        public decimal? _shareOwner;
        public decimal? ShareOwner
        {
            get => _shareOwner;
            set => SetProperty(ref _shareOwner!, value);
        }

        public decimal? _sharePP;
        public decimal? SharePP
        {
            get => _sharePP;
            set => SetProperty(ref _sharePP!, value);
        }

        private SrvBuyer _buyer;

        public void LoadData(SrvGameDetail gameDetail)
        {
            Name = gameDetail.Name;
            Owner = gameDetail.Owner;
            SellingPrice = gameDetail.SellingPrice;
        }

        public void LoadBuyer(SrvBuyer buyer)
        {
            BuyerName = buyer.Name;
            Number = buyer.Number.ToString();

            _buyer = buyer;
        }

        public void CheckTextBoxForDigits(string text)
        {
            string numbers = string.Empty;

            foreach (var c in text)
            {
                if (char.IsDigit(c))
                {
                    numbers += c;
                }
            }

            if (Int32.TryParse(numbers, out int n))
            {
                PurchasePrice = n.ToString();
                Shares(n);
            }
            else
            {
                PurchasePrice = "";
            }
        }

        private void Shares(int value)
        {
            decimal sharePP = (decimal)value / 10;
            int integerSharePP = (int)Math.Round(sharePP);

            SharePP = integerSharePP;
            ShareOwner = value - integerSharePP;
        }
    }
}
