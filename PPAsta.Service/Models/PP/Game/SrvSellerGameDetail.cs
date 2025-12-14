using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPAsta.Service.Models.PP.Game
{
    public class SrvSellerGameDetail : SrvFields, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Year { get; set; }
        public string Buyer { get; set; }
        public PaymentSellerProcess PaymentSellerProcess { get; set; }
        public PaymentType? PaymentType { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ShareOwner { get; set; }
        public decimal? SharePP { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
