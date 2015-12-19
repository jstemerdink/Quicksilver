using System.Linq;

using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Extensions
{
    public static class ContentLoaderExtensions
    {
        public static T GetFirstChild<T>(this IContentLoader contentLoader, ContentReference contentReference) where T : IContentData
        {
            return contentLoader.GetChildren<T>(contentReference).FirstOrDefault();
        }
    }
}