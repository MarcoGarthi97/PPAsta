using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.Payment;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Payment;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.Google
{
    public interface ISrvGoogleSpreadsheetService : IForServiceCollectionExtension
    {
        Task<string> GetGoogleSpreadsheetDataAsync(string url);
    }

    public class SrvGoogleSpreadsheetService : ISrvGoogleSpreadsheetService
    {
        private readonly ILogger<SrvGoogleSpreadsheetService> _logger;

        public SrvGoogleSpreadsheetService(ILogger<SrvGoogleSpreadsheetService> logger)
        {
            _logger = logger;
        }        

        public async Task<string> GetGoogleSpreadsheetDataAsync(string url)
        {
            _logger.LogInformation(nameof(GetGoogleSpreadsheetDataAsync) + " start");

            url = ValidateUrl(url);

            using var client = new HttpClient();
            var csvData = await client.GetStringAsync(
                url);

            _logger.LogInformation(nameof(GetGoogleSpreadsheetDataAsync) + " end");

            return csvData;
        }

        private string ValidateUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains(@"https://docs.google.com/spreadsheets"))
                {
                    if (url.Contains("export"))
                    {
                        if (!url.Contains("format=csv"))
                        {
                            url = url.Substring(url.IndexOf("?")) + "format=csv";
                        }
                    }
                    else
                    {
                        if (url.Contains("?"))
                        {
                            int endIndex = url.Substring(0, url.IndexOf("?")).LastIndexOf('/') + 1;
                            url = url.Substring(0, endIndex);
                        }

                        string s = "export?format=csv";
                        if (url[url.Length - 1] != '/')
                        {
                            s = @"/" + s;
                        }

                        url += s;
                    }
                }
                else
                {
                    throw new Exception("Il file online non è di Google Drive");
                }
            }
            else
            {
                throw new Exception("Url non valido");
            }
            
            return url;
        }
    }
}
