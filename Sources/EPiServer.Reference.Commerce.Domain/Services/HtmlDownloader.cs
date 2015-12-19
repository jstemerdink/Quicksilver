using System;
using System.Net;
using System.Net.Http;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;

namespace EPiServer.Reference.Commerce.Domain.Services
{
    public abstract class HtmlDownloader : IHtmlDownloader
    {
        public virtual string Download(string baseUrl, string relativeUrl)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(baseUrl) })
            {
                var fullUrl = client.BaseAddress + relativeUrl;

                var response = client.GetAsync(fullUrl).Result;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        string.Format("Request to '{0}' was unsuccessful. Content:\n{1}",
                                        fullUrl, response.Content.ReadAsStringAsync().Result));
                }

                return response.Content.ReadAsStringAsync().Result;
            }
           
        }
    }
}