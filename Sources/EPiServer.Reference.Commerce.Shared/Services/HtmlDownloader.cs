using EPiServer.ServiceLocation;
using System;
using System.Net;
using System.Net.Http;

using EPiServer.Reference.Commerce.Domain.Contracts.Services;

namespace EPiServer.Reference.Commerce.Shared.Services
{
    [ServiceConfiguration(typeof(IHtmlDownloader), Lifecycle = ServiceInstanceScope.Singleton)]
    public class HtmlDownloader : Domain.Services.HtmlDownloader
    {
        
    }
}