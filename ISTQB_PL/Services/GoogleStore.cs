using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ISTQB_PL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class GoogleStore : IGoogleStore<Item>
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string _spreadsheetId = "12LNWL7DtknrKTYQMrf3Hsn-coaMn5Lscg4QMv-tDN-I"; //id udostępnionego pliku
        public static string _SheetName = "questions"; //questions
        private static readonly string ApplicationName = "DRiU";

        public static SheetsService SheetsService { get; set; }

        private List<Item> items = new List<Item>();
        
        public GoogleStore()
        {

        }

        public async Task<IEnumerable<Item>> GoogleSheetsConnect(bool forceRefresh = false)
        {
            GoogleCredential credential;
            var assembly = typeof(MockDataStore).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("ISTQB_PL.dazzling-spirit-383513-0f9dd5bb5d8c.json");
            using (var reader = new StreamReader(stream))
            {
                credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            SheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            //GET DATA FROM Google Sheets
            if (SheetsService == null)
                await GoogleSheetsConnect(true);

            var range = $"{_SheetName}!R2C1:R200C9";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;

            try
            {
                var response = appendRequest.Execute();

                foreach (var str in response.Values)
                {
                    items.Add(
                        new Item
                        {
                            Id = str[0].ToString(),
                            Content = str[1].ToString(),
                            Answer_a = str[2].ToString(),
                            Answer_b = str[3].ToString(),
                            Answer_c = str[4].ToString(),
                            Answer_d = str[5].ToString(),
                            Answer_right = str[6].ToString(),
                            Str_Picture = str[7].ToString(),
                            Str_Explanation = str[8].ToString()
                        }
                    );
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
            }

            return await Task.FromResult(items);
        }
    }
}
