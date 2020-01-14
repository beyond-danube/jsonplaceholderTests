using RestSharp;
using RestSharp.Serialization.Json;
using RestSharp.Extensions;
using System;
using System.IO;

namespace Helpers
{
    public class RequestHelper
    {
        private RestClient Client;
        private RestRequest Request;

        public RequestHelper()
        {
            Client = new RestClient();
            Request = new RestRequest();
        }

        public RequestHelper SetMethod(Method method)
        {
            Request.Method = method;

            return this;
        }

        public RequestHelper SetStringBody(string body)
        {
            Request.AddJsonBody(body);

            return this;
        }

        public IRestResponse GetResponse(string url)
        {
            Client.BaseUrl = new Uri(url);

            IRestResponse response = Client.Execute(Request);

            return response;
        }

        public T GetResponse<T>(string url)
        {
            var response = new JsonDeserializer().Deserialize<T>(GetResponse(url));

            return response;
        }

        public void SaveImage(string url, string fileToSave)
        {
            Client.BaseUrl = new Uri(url);

            var imgBytes = Client.DownloadData(Request);

            File.WriteAllBytes(fileToSave, imgBytes);
        }
    }
}