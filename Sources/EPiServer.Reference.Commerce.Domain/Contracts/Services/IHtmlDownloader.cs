namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface IHtmlDownloader
    {
        string Download(string baseUrl, string relativeUrl);
    }
}