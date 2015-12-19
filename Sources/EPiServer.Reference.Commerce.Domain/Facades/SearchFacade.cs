using System;

using Mediachase.Commerce.Core;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search;
using Mediachase.Search.Extensions;

using StringCollection = System.Collections.Specialized.StringCollection;

namespace EPiServer.Reference.Commerce.Domain.Facades
{
    public class SearchFacade
    {
        public enum SearchProviderType
        {
            Lucene,
            Unknown
        }

        private SearchManager _searchManager;
        private SearchProviderType _searchProviderType;
        private bool _initialized;

        public virtual ISearchResults Search(CatalogEntrySearchCriteria criteria)
        {
            this.Initialize();
            return this._searchManager.Search(criteria);
        }

        public virtual SearchProviderType GetSearchProvider()
        {
            this.Initialize();
            return this._searchProviderType;
        }

        public virtual SearchFilter[] SearchFilters
        {
            get { return SearchFilterHelper.Current.SearchConfig.SearchFilters; }
        }

        public virtual StringCollection GetOutlinesForNode(string code)
        {
            return SearchFilterHelper.GetOutlinesForNode(code);
        }

        private void Initialize()
        {
            if (this._initialized)
            {
                return;
            }
            this._searchManager = new SearchManager(AppContext.Current.ApplicationName);
            this._searchProviderType = this.LoadSearchProvider();
            this._initialized = true;
        }

        private SearchProviderType LoadSearchProvider()
        {
            var element = SearchConfiguration.Instance.SearchProviders;
            if (element.Providers == null ||
                String.IsNullOrEmpty(element.DefaultProvider) ||
                String.IsNullOrEmpty(element.Providers[element.DefaultProvider].Type))
            {
                return SearchProviderType.Unknown;
            }

            var providerType = Type.GetType(element.Providers[element.DefaultProvider].Type);
            var baseType = Type.GetType("Mediachase.Search.Providers.Lucene.LuceneSearchProvider, Mediachase.Search.LuceneSearchProvider");
            if (providerType == null || baseType == null)
            {
                return SearchProviderType.Unknown;
            }
            if (providerType == baseType || providerType.IsSubclassOf(baseType))
            {
                return SearchProviderType.Lucene;
            }

            return SearchProviderType.Unknown;
        }


    }
}