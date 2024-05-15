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
    public class SylabusWersjaDataStore : IDataSylabusWersja<SylabusWersja>
    {
        //zmienne
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string _spreadsheetId = "12LNWL7DtknrKTYQMrf3Hsn-coaMn5Lscg4QMv-tDN-I"; //id udostępnionego pliku
        //public static string _SheetName = "questions"; //questions
        private static readonly string ApplicationName = "DRiU";

        private static SheetsService SheetsService { get; set; }

        private readonly List<SylabusWersja> ItemsSylabusWersja = new List<SylabusWersja>();

        public void GoogleConnect()
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

        }

        public async Task<IEnumerable<SylabusWersja>> GoogleSheetsSylabusWersja()
        {
            GoogleConnect();

            //GET DATA FROM Google Sheets
            if (SheetsService == null)
                await GoogleSheetsSylabusWersja();

            var range = $"{"sylabus_wersja"}!R2C1:R2000C4";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;

            ItemsSylabusWersja.Clear();

            try
            {
                var response = appendRequest.Execute();

                foreach (var items in response.Values)
                {
                    ItemsSylabusWersja.Add(
                        new SylabusWersja
                        {
                            Id = items[0].ToString(),
                            Sylabus = items[1].ToString(),
                            Jezyk = items[2].ToString(),
                            Wersja = items[3].ToString(),
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
            
            return await Task.FromResult(ItemsSylabusWersja);
        }
    }
}
