namespace EPiServer.Reference.Commerce.Domain.Contracts.Services
{
    public interface ICookieService
    {
        string Get(string cookie);

        void Set(string cookie, string value);
    }
}