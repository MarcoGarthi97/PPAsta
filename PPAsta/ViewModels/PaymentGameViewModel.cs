using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Buyer;
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
        private readonly ISrvPaymentGameService _paymentGameService;
        private readonly ISrvBuyerService _buyerService;
        private readonly ISrvPaymentService _paymentService;

        public PaymentGameViewModel(ISrvPaymentGameService paymentGameService, ISrvBuyerService buyerService, ISrvPaymentService paymentService)
        {
            _paymentGameService = paymentGameService;
            _buyerService = buyerService;
            _paymentService = paymentService;
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
        public async Task LoadDataAsync(SrvGameDetail gameDetail)
        {
            _gameId = gameDetail.Id;

            Name = gameDetail.Name;
            Owner = gameDetail.Owner;
            SellingPrice = (double)gameDetail.SellingPrice;

            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(gameDetail.Id);
            if (paymentGame != null)
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
            int integerSharePP = (int)Math.Round(sharePP);

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
            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(_gameId);

            paymentGame.PurchasePrice = Convert.ToDecimal(PurchasePrice);
            paymentGame.SharePP = (decimal)SharePP!;
            paymentGame.ShareOwner = (decimal)ShareOwner!;
            paymentGame.BuyerId = _buyer.Id;
            paymentGame.PaymentProcess = PaymentGameProcess.ToBePaid;
            paymentGame.RUD = DateTime.Now;

            int paymentId = await _paymentService.UpsertPaymentAsync(paymentGame);
            paymentGame.PaymentId = paymentId;

            await _paymentGameService.UpdatePaymentGameAsync(paymentGame);
        }

        public bool CheckField()
        {
            return !string.IsNullOrEmpty(PurchasePrice)
                && ShareOwner != null
                && SharePP != null
                && (_buyer != null || !string.IsNullOrEmpty(BuyerName) || !string.IsNullOrEmpty(Number));
        }
    }
}
