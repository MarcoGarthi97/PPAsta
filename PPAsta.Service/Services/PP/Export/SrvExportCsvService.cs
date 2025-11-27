using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Export
{
    public interface ISrvExportCsvService : IForServiceCollectionExtension
    {
        void ExportCsvGameDetails(IEnumerable<SrvGameDetail> gameDetails, string filePath);
    }

    public class SrvExportCsvService : ISrvExportCsvService
    {
        public SrvExportCsvService() { }

        public void ExportCsvGameDetails(IEnumerable<SrvGameDetail> gameDetails, string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Articolo; Venditore; Prezzo di partenza; Acquirente; Pagato; Prezzo Acquisto; Quota PP; Quota Venditore");

            foreach (var gameDetail in gameDetails)
            {
                string isPayed = gameDetail.PaymentProcess == Abstraction.Models.Enums.PaymentGameProcess.Paid ? "Si" : "No";
                sb.AppendLine($"{gameDetail.Name}; {gameDetail.Owner}; {gameDetail.SellingPrice}; {gameDetail.Buyer}; {isPayed}; {gameDetail.PurchasePrice}; {gameDetail.SharePP}; {gameDetail.ShareOwner}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
