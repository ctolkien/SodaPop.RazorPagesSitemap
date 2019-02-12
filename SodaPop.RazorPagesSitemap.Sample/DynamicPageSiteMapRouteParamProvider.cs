using System.Collections.Generic;
using System.Threading.Tasks;

namespace SodaPop.RazorPagesSitemap.Sample
{
    public class DynamicPageSiteMapRouteParamProvider : ISitemapRouteParamProvider
    {
        public Task<bool> CanSupplyParamsForPageAsync(string pagePath)
        {
            return Task.FromResult(pagePath == "/DynamicPage");
        }

        public Task<IEnumerable<object>> GetRouteParamsAsync()
        {
            var routeParams = new[] {
                new { id = 1, otherParam = "hello" },
                new { id = 2, otherParam = "bye" }
            };
            return Task.FromResult<IEnumerable<object>>(routeParams);
        }
    }
}
