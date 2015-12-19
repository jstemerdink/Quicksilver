using System;
using System.Web;
using System.Web.Mvc;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class ControllerExceptionHandler : Domain.Handlers.ControllerExceptionHandler
    {
        
    }
}