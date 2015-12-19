using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Models
{
    [ModelBinder(typeof(FilterOptionFormModelBinder))]
    public class FilterOptionFormModel
    {
        public List<FacetGroupOption> FacetGroups { get; set; }
        public IEnumerable<SelectListItem> Sorting { get; set; }
        public string Sort { get; set; }
        public int Page { get; set; }
        public string Q { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }
}