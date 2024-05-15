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
    public class SylabusDataStore : IDataSylabus<Sylabus>
    {
        //zmienne
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string _spreadsheetId = "12LNWL7DtknrKTYQMrf3Hsn-coaMn5Lscg4QMv-tDN-I"; //id udostępnionego pliku
        //public static string _SheetName = "questions"; //questions
        private static readonly string ApplicationName = "DRiU";

        private static SheetsService SheetsService { get; set; }

        private readonly List<Sylabus> itemsSylabus = new List<Sylabus>();
        private readonly List<string> WhatIsSylabus = new List<string>();
        
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

        public async Task<string> GoogleSheetsWhatIsSylabus()
        {
            GoogleConnect();
            //GET DATA FROM Google Sheets
            if (SheetsService == null)
                await GoogleSheetsWhatIsSylabus();
            var range = $"{"whatisistqb"}!R2C1:R2C1";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;
            try
            {
                var response = appendRequest.Execute();
                foreach (var items in response.Values)
                {
                    WhatIsSylabus.Add(items[0].ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }

            return await Task.FromResult(WhatIsSylabus[0]);
        }

        public async Task<IEnumerable<Sylabus>> GoogleSheetsSylabus(bool forceRefresh)
        {
            if (forceRefresh == true)
            {
                itemsSylabus.Clear();
            }
            if (itemsSylabus.Count == 0)
            {
                GoogleConnect();

                //GET DATA FROM Google Sheets
                if (SheetsService == null)
                    await GoogleSheetsSylabus(true);

                var range = $"{"sylabus"}!R2C1:R2000C10";

                var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                    MajorDimensionEnum.ROWS;

                itemsSylabus.Clear();

                try
                {
                    var response = appendRequest.Execute();

                    foreach (var items in response.Values)
                    {
                        itemsSylabus.Add(
                            new Sylabus
                            {
                                Id = items[0].ToString(),
                                Rozdzial = items[1].ToString(),
                                Rozdzial_description = items[2].ToString(),
                                Podrozdzial = items[3].ToString(),
                                Podrozdzial_description = items[4].ToString(),
                                Podpodrozdzial = items[5].ToString(),
                                Podpodrozdzial_description = items[6].ToString(),
                                Content = items[7].ToString(),
                                Umiejetnosci = items[8].ToString(),
                                Wersja = items[9].ToString()
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
            }
            return await Task.FromResult(itemsSylabus);
        }

    }
}
