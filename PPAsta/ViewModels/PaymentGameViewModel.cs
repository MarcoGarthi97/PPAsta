using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Buyer;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Payment;
using PPAsta.Service.Services.PP.PaymentGame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;

namespace PPAsta.ViewModels
{
    public class PaymentGameViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGameService _gameService;
        private readonly ISrvPaymentGameService _paymentGameService;
        private readonly ISrvBuyerService _buyerService;
        private readonly ISrvPaymentService _paymentService;

        public PaymentGameViewModel(ISrvPaymentGameService paymentGameService, ISrvBuyerService buyerService, ISrvPaymentService paymentService, ISrvGameService gameService)
        {
            _paymentGameService = paymentGameService;
            _buyerService = buyerService;
            _paymentService = paymentService;
            _gameService = gameService;
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

        private double? _sellingPrice;
        public double? SellingPrice
        {
            get => _sellingPrice;
            set => SetProperty(ref _sellingPrice!, value);
        }

        private string _buyerName;
        public string? BuyerName
        {
            get => _buyerName;
            set => SetProperty(ref _buyerName!, value);
        }

        private string _number;
        public string? Number
        {
            get => _number;
            set => SetProperty(ref _number!, value);
        }

        private string _purchasePrice;
        public string? PurchasePrice
        {
            get => _purchasePrice;
            set => SetProperty(ref _purchasePrice!, value);
        }

        private double? _shareOwner;
        public double? ShareOwner
        {
            get => _shareOwner;
            set => SetProperty(ref _shareOwner!, value);
        }

        private double? _sharePP;
        public double? SharePP
        {
            get => _sharePP;
            set => SetProperty(ref _sharePP!, value);
        }

        private SrvBuyer _buyer;
        private int _gameId;
        private int _year;
        public async Task LoadDataAsync(SrvGameDetail gameDetail)
        {
            _gameId = gameDetail.Id;
            _year = gameDetail.Year;

            Name = gameDetail.Name;
            Owner = gameDetail.Owner;
            SellingPrice = (double)gameDetail.SellingPrice;

            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(gameDetail.Id);
            if (paymentGame != null && paymentGame.BuyerId.HasValue)
            {
                PurchasePrice = paymentGame.PurchasePrice.ToString();
                SharePP = (double)paymentGame.SharePP!;
                ShareOwner = (double)paymentGame.ShareOwner!;

                await LoadBuyerAsync(paymentGame.BuyerId.Value);
            }
        }

        private async Task LoadBuyerAsync(int buyerId)
        {
            var buyer = await _buyerService.GetBuyerByIdAsync(buyerId);
            LoadBuyer(buyer);
        }

        public void LoadBuyer(SrvBuyer buyer)
        {
            BuyerName = buyer.Name;
            Number = buyer.Number.ToString();

            _buyer = buyer;
        }

        public void CalculateShares(int value)
        {
            double sharePP = (double)value / 10;
            double integerSharePP = Math.Round(sharePP, 2);

            SharePP = integerSharePP;
            ShareOwner = value - integerSharePP;
        }

        public void ClearData()
        {
            _buyer = new SrvBuyer();
            PurchasePrice = "";
            BuyerName = "";
            Number = "";

            CalculateShares(0);
        }

        public async Task InsertPaymentGameAsync()
        {
            if (_buyer?.Name != BuyerName || _buyer?.Number.ToString() != Number)
            {
                await HandleBuyerAsync();
            }

            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(_gameId);

            var oldBuyerId = paymentGame.BuyerId;

            paymentGame.PurchasePrice = Convert.ToDecimal(PurchasePrice);
            paymentGame.SharePP = (decimal)SharePP!;
            paymentGame.ShareOwner = (decimal)ShareOwner!;
            paymentGame.BuyerId = _buyer.Id;
            paymentGame.PaymentProcess = PaymentGameProcess.ToBePaid;
            paymentGame.RUD = DateTime.Now;

            await _paymentGameService.UpdatePaymentGameAsync(paymentGame);

            if (oldBuyerId.HasValue)
            {
                await _paymentService.RemoveGameForPaymentAsync(oldBuyerId.Value);
            }

            await _paymentService.InsertPaymentAsync(paymentGame);
        }

        public async Task HandleBuyerAsync()
        {
            SrvBuyer buyer;

            buyer = await _buyerService.GetBuyerByNumberAsync(ConvertNumber(Number), _year);

            if (buyer == null)
            {
                if (string.IsNullOrEmpty(BuyerName))
                {
                    throw new Exception("Inserire il nome acquirente");
                }

                int number = !string.IsNullOrEmpty(Number) ? ConvertNumber(Number) : await _buyerService.GetNextNumberByYearAsync(_year);
                buyer = new SrvBuyer
                {
                    Number = number,
                    Name = BuyerName,
                    Year = _year,
                };

                await _buyerService.InsertBuyerAsync(buyer);
                buyer = await _buyerService.GetBuyerByNameAsync(BuyerName, _year);

                Number = number.ToString();
                BuyerName = buyer.Name;
            }
            else if (buyer.Name != BuyerName)
            {
                throw new Exception("Numero o nome acquirente errati");
            }
            
            _buyer = buyer;
        }

        public bool CheckField()
        {
            return !string.IsNullOrEmpty(PurchasePrice)
                && ShareOwner != null
                && SharePP != null
                && (_buyer != null || !string.IsNullOrEmpty(BuyerName) || !string.IsNullOrEmpty(Number));
        }

        public int GetYear()
        {
            return _year;
        }

        private int ConvertNumber(string number)
        {
            if (Int32.TryParse(number, out int n))
            {
                return n;
            }
            else
            {
                throw new Exception("Impossibile convertire il campo Numero");
            }
        }
    }
}
