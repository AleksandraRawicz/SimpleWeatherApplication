using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Services
{
    public class ApiCaller
    {
        public static async Task<ApiResponse> Get(string url)
        {
            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(url);

                if (request.IsSuccessStatusCode)
                    return new ApiResponse() { Response = await request.Content.ReadAsStringAsync() };
                return new ApiResponse() { ErrorInfo = request.ReasonPhrase };
            }
        }

    }

    public class ApiResponse
    {
        public string ErrorInfo { get; set; }
        public bool Succesfull { get => string.IsNullOrEmpty(ErrorInfo); }
        public string Response { get; set; }
    }
}
