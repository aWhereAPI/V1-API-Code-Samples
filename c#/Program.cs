using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace aWhere.Api.Weather.Sample {
    internal class Program {
	
        #region Constants

        public const String CONSUMER_KEY = "";
        public const String CONSUMER_SECRET = "";

        public const String HOST = "https://api.awhere.com";
        public static readonly String OAUTHTOKEN = GetOAuthToken().Result;

        #endregion Constants



        #region Methods

        /// <summary>
        /// Formats and encodes credentials appropriately for Basic Authentication
        /// </summary>
        private async static Task<String> GetOAuthToken() {
            String oauthToken = String.Empty;

            using (var httpClient = new HttpClient()) {
	
                httpClient.DefaultRequestHeaders.Accept.Clear();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(CONSUMER_KEY + ":" + CONSUMER_SECRET)));

                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                HttpResponseMessage response = await httpClient.PostAsync(HOST + "/oauth/token", content);

                if (response.IsSuccessStatusCode) {
                    dynamic jsonResponse = await response.Content.ReadAsAsync<ExpandoObject>();
                    oauthToken = (String)jsonResponse.access_token;
                }
            }

            return oauthToken;
        }

        /// <summary>
        /// Builds a URL with an example query string for demonstration of GET request.
        /// </summary>
        private static string BuildUrlWithExampleQueryString(bool useFahrenheit) {
	
            DateTime dateTime = DateTime.Today;
    
        	string dayString = dateTime.Day >= 10 ? dateTime.Day.ToString() : "0" + dateTime.Day;
            
			string monthString = dateTime.Month >= 10 ? dateTime.Month.ToString() : "0" + dateTime.Month;

            string dateString = dateTime.Year + "-" + dateTime.Month + "-" + dayString;

            string url = HOST + "/v1/weather" + "?latitude=30.25&longitude=-97.75&startDate=" + dateString + "&attribute=maxTemperature&attribute=precip";

            if (useFahrenheit) {
                url += "&temperatureUnits=fahrenheit";
            }

            return url;
        }


        /// <summary>
        /// Sample GET request to the Weather API, using the Web API 2.1 client library
        /// </summary>
        /// <returns>Task resulting in the Json response</returns>
        private static async Task<string> GetWeatherJson(string queryString) {
            string weatherResponse = string.Empty;

            using (var httpClient = new HttpClient()) {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OAUTHTOKEN);

                HttpResponseMessage response = await httpClient.GetAsync(queryString);
                if (response.IsSuccessStatusCode) {
                    weatherResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return weatherResponse;
        }


        private static void Main(string[] args) {
            Console.WriteLine("Select a temperature unit option:");
            Console.WriteLine();
            Console.WriteLine("1. Make requests in Celsius mode");
            Console.WriteLine("2. Make requests in Fahrenheit mode");
            bool useFahrenheit = false;

            string jsonResponse = string.Empty;

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1) {
                useFahrenheit = false;
            } else if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2) {
                useFahrenheit = true;
            }

            jsonResponse = string.Empty;

            string queryString = BuildUrlWithExampleQueryString(useFahrenheit);
            jsonResponse = GetWeatherJson(queryString).Result;
            Console.WriteLine(jsonResponse);

            Console.WriteLine();
            Console.WriteLine("Press any key to Exit...");
            Console.ReadKey();
        }

        #endregion Methods
    }
}