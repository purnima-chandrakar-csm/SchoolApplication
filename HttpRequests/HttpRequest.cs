using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public interface IHttpRequest
    {
        Task<string> PostRequestAsync(string URI, string parameterValues);
        Task<string> GetRequestAsync(string URI, string parameterValues);
    }
    public class HttpRequest : IHttpRequest
    {
        public readonly KeyList _objKeyList;

        public HttpRequest(IOptions<KeyList> objKeyList)
        {
            _objKeyList = objKeyList.Value;            
        }
     
    public async Task<string> PostRequestAsync(string URI, string parameterValues)
    {
        string BaseURI = _objKeyList.WebApiurl;
        string URL = BaseURI + URI;
        string jsonString = null;

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpContent c = new StringContent(parameterValues, Encoding.UTF8, "application/json");

            // 1. Await the POST response asynchronously
            HttpResponseMessage response = await client.PostAsync(URL, c);

            if (response.IsSuccessStatusCode)
            {
                // 2. Await the string reading asynchronously
                string fullContent = await response.Content.ReadAsStringAsync();
                jsonString = fullContent.Trim('"');
            }
        }
        return jsonString;
    }


    public async Task<string> GetRequestAsync(string URI, string parameterValues)
        {
            string BaseURI = _objKeyList.WebApiurl;
            // Append parameter values to the URL if they exist
            string URL = BaseURI + URI + (string.IsNullOrEmpty(parameterValues) ? "" : parameterValues);
            string jsonString = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Use GetAsync instead of PostAsync; GET requests do not have a body (HttpContent)
                HttpResponseMessage response = await client.GetAsync(URL);
                if (response.IsSuccessStatusCode)
                {
                    jsonString = response.Content.ReadAsStringAsync()
                                                   .Result
                                                   .Trim('"');
                }
            }
            return jsonString;
        }


    }
}
