using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ISTQB_PL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace ISTQB_PL.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        //zmienne
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        //id udostępnionego pliku
        private static readonly string _spreadsheetId = "12LNWL7DtknrKTYQMrf3Hsn-coaMn5Lscg4QMv-tDN-I"; 
        private static readonly string ApplicationName = "DRiU";
        private Xamarin.Forms.Color MainTextColor {  get; set; }

        private static SheetsService SheetsService { get; set; }

        public MockDataStore()
        {
            MainTextColor = (Xamarin.Forms.Color)Application.Current.Resources["JasnyTekst"];
        }

        private readonly List<Item> items = new List<Item>();

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        bool visible_picture = false;

        //connect to google Sheets
        private void GoogleConnect()
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

        public async Task<IEnumerable<Item>> GoogleSheetsExam(bool forceRefresh)
        {
            if(forceRefresh == true)
            {
                items.Clear();
            }
            if (items.Count == 0)
            {
                GoogleConnect();

                //GET DATA FROM Google Sheets
                if (SheetsService == null)
                    await GoogleSheetsExam(true);

                var range = $"{"questions"}!R2C1:R2000C12";

                var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                    MajorDimensionEnum.ROWS;

                items.Clear();

                try
                {
                    var response = appendRequest.Execute();

                    foreach (var item in response.Values)
                    {
                        if (item[7].ToString() != "null")
                        {
                            visible_picture = true;
                        }
                        else
                        {
                            visible_picture = false;
                        }
                        items.Add(
                            new Item
                            {
                                Id = item[0].ToString(),
                                MyContent = item[1].ToString(),
                                Answer_a = item[2].ToString(),
                                Answer_b = item[3].ToString(),
                                Answer_c = item[4].ToString(),
                                Answer_d = item[5].ToString(),
                                Answer_right = item[6].ToString(),
                                Str_Picture = item[7].ToString(),
                                Visible_Picture = visible_picture,
                                Str_Explanation = item[8].ToString(),
                                Exp_Picture = item[9].ToString(),
                                Rozdzial = item[10].ToString(),
                                Wersja_Sylabus = item[11].ToString(),
                                IsSelected = false,
                                Answer_color = "null",
                                ItemMainTextColor = MainTextColor
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

            return await Task.FromResult(items);
        }

        public async Task<bool> GoogleSheetsLogin(string login, string passwd)
        {
            GoogleConnect();

            if (SheetsService == null)
                await GoogleSheetsLogin(login, passwd);

            //GET DATA FROM Google Sheets
            var range = $"{"login"}!R2C1:R2000C4";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;

            try
            {
                var response = appendRequest.Execute();

                DateTime dateNow = DateTime.Now.Date;
                DateTime expirenceData;
                foreach (var row in response.Values)
                {
                    expirenceData = DateTime.Parse(row[3].ToString());
                    

                    if (row[1].ToString().ToLower() == login.ToLower() && row[2].ToString() == passwd)
                    {
                        if (expirenceData > dateNow.Date)
                        {
                            return await Task.FromResult(true);
                        }
                        else
                        {
                            try
                            {
                                await DependencyService.Get<IAlertService>().DisplayAlert("Uwaga!!!", "Ważność Twojego konta wygasła, skontaktuj się z wrecz@wp.pl", "OK");
                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DependencyService.Get<IAlertService>().DisplayAlert("Title", ex.ToString(), "OK");
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> GoogleLastLogin(DateTime actualdate, string login)
        {
            GoogleConnect();

            var range = $"{"login"}!R2C1:R2000C5";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            appendRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.
                MajorDimensionEnum.ROWS;
            string row = "";
            try
            {
                var response = appendRequest.Execute();

                for(int i = 0; i < response.Values.Count; i++)
                {
                    if (response.Values[i][1].ToString().ToLower() == login.ToLower())
                    {
                        row = (i+2).ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            ValueRange body = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { actualdate } }
            };
            
            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = 
                SheetsService.Spreadsheets.Values.Update(body, _spreadsheetId, $"login!R{row}C5");
            updateRequest.ValueInputOption = 
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            //UpdateValuesResponse update 
                _ = updateRequest.Execute();

            return await Task.FromResult(false);
        }
    }
}