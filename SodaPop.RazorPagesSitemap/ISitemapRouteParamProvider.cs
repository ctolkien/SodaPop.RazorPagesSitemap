using System.Collections.Generic;
using System.Threading.Tasks;

namespace SodaPop.RazorPagesSitemap
{
    public interface ISitemapRouteParamProvider
    {
        Task<bool> CanSupplyParamsForPageAsync(string pagePath);
        Task<IEnumerable<object>> GetRouteParamsAsync();
    }
}
