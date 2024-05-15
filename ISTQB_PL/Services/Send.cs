//using GoogleSheetsHelper;
using System;
using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.Reflection;
using System.Linq;
using Xamarin.Forms.Internals;

namespace ISTQB_PL.Services
{
    public class Send
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string spreadsheetId = "1e9nHy4brmTT4UKLLMhD3QJyqM8W5OJp9Aq9jbadpGLM"; //id udostępnionego pliku 1e9nHy4brmTT4UKLLMhD3QJyqM8W5OJp9Aq9jbadpGLM
        public readonly string _SheetName = "driu";
        private static readonly string ApplicationName = "DRiU";
        public static SheetsService SheetsService { get; set; }

        ~Send()
        {
            SheetsService = null;
        }
        // constructor
#pragma warning disable
        public Send()
        {
        }

        //public Send(string sheetName,
        //    string sendFormaZgloszenia, string sendKto, string sendBU,
        //    string sendAplikacja, string sendNazwaZlecenia)
        //{
        //    _SheetName = sheetName;
        //    SendFormaZgloszenia = sendFormaZgloszenia;
        //    SendKto = sendKto;
        //    SendBU = sendBU;
        //    SendAplikacja = sendAplikacja;
        //    SendNazwaZlecenia = sendNazwaZlecenia;
        //}

        public static void GoogleSheetsConnect()
        {
            GoogleCredential credential;

            var assembly = typeof(Send).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("DRiUv4.dazzling-spirit-383513-0f9dd5bb5d8c.json");
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

        public void SendData(string nazwisko, string formazgloszenia, string kto, string bu, string aplikacja,
            string nazwazlecenia, string macadress)
        {
            if (SheetsService == null)
                GoogleSheetsConnect();

            var range = $"{_SheetName}!A:G";
            ValueRange valueRange = new ValueRange();
            var objectList = new List<object>()
            {
                nazwisko, formazgloszenia, kto, bu, aplikacja, nazwazlecenia,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
                macadress
            };
            valueRange.Values = new List<IList<object>>() { objectList };

            var appendRequest = SheetsService.Spreadsheets.Values.Append(valueRange,
                spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            appendRequest.Execute();
        }

        public static string CheckVersion()
        {
            if (SheetsService == null)
                GoogleSheetsConnect();
            var range = "version!A2:A2";
            var appendRequest = SheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = appendRequest.Execute();

            return response.Values[0][0].ToString() ?? "null";
        }

        public static Dictionary<string, string> CheckWho()
        {
            if (SheetsService == null)
                GoogleSheetsConnect();
            var range = "who!A:B";

            var appendRequest = SheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = appendRequest.Execute();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            for (int i = 0; i < response.Values.Count; i++)
            {
                keyValuePairs.Add(response.Values[i][1].ToString(), response.Values[i][0].ToString());
            }
            return keyValuePairs;
            //return response.Values[0][0].ToString() ?? "null";
        }

        public static void SendWho(string nazwisko, string deviceId)
        {
            if (SheetsService == null)
                GoogleSheetsConnect();

            ValueRange valueRange = new ValueRange();
            var range = "who!A:B";
            var objectListRequest = SheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = objectListRequest.Execute();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            for (int i = 0; i < response.Values.Count; i++)
            {
                keyValuePairs.Add(response.Values[i][1].ToString(), response.Values[i][0].ToString());
            }

            int RowNr = keyValuePairs.Keys.IndexOf(deviceId);
            if (keyValuePairs.ContainsKey(deviceId))
            {
                ValueRange valueCell = new ValueRange();
                var cellList = new List<object>()
                {
                    nazwisko
                };
                valueCell.Values = new List<IList<object>>() { cellList };
                var updateRequest = SheetsService.Spreadsheets.Values.Update(valueCell,
                    spreadsheetId, $"who!A{RowNr + 1}:A{RowNr + 1}");
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                updateRequest.Execute();
            }
            else
            {
                //append new name and DeviceId
                ValueRange appendRange = new ValueRange();
                var appendList = new List<object>()
                {
                    nazwisko, deviceId
                };
                appendRange.Values = new List<IList<object>>() { appendList };
                var appendRequest = SheetsService.Spreadsheets.Values.Append(appendRange,
                            spreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

                appendRequest.Execute();
            }
        }
    }
}
