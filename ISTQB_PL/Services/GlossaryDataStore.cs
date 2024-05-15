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
    public class GlossaryDataStore : IDataGlossary<Glossary>
    {
        //zmienne
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string _spreadsheetId = "12LNWL7DtknrKTYQMrf3Hsn-coaMn5Lscg4QMv-tDN-I"; //id udostępnionego pliku
        //public static string _SheetName = "questions"; //questions
        private static readonly string ApplicationName = "DRiU";

        private static SheetsService SheetsService { get; set; }

        private readonly List<Glossary> itemsGlossary = new List<Glossary>();
        private int MainFontSize { get; set; }

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

        public async Task<IEnumerable<Glossary>> GoogleSheetsGlossary()
        {
            GoogleConnect();

            //GET DATA FROM Google Sheets
            if (SheetsService == null)
                await GoogleSheetsGlossary();

            var range = $"{"qlossary"}!R2C1:R2000C3";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;

            itemsGlossary.Clear();
            try
            {
                var response = appendRequest.Execute();

                foreach (var items in response.Values)
                {
                    itemsGlossary.Add(
                        new Glossary
                        {
                            Id = items[0].ToString(),
                            Name = items[1].ToString(),
                            Description = items[2].ToString(),
                            MyFontSize = MainFontSize
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
            return await Task.FromResult(itemsGlossary);
        }
    }
}
