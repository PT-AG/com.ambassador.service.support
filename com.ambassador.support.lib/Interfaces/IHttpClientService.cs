using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.ambassador.support.lib.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> PutAsync(string url, HttpContent content);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<HttpResponseMessage> PatchAsync(string url, HttpContent content);
        Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string url, string token, HttpContent content);
    }
}
