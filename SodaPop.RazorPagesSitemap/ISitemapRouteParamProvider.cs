using System.Collections.Generic;
using System.Threading.Tasks;

namespace SodaPop.RazorPagesSitemap
{
    /// <summary>
    /// Supports injecting your own implementation for providing route parameters to specific razor pages
    /// </summary>
    public interface ISitemapRouteParamProvider
    {
        /// <summary>
        /// Return true if the <paramref name="pagePath"/> matches the Razor Page path you want to supply route parameters for
        /// </summary>
        /// <param name="pagePath">Path of the Razor Page</param>
        /// <returns></returns>
        Task<bool> CanSupplyParamsForPageAsync(string pagePath);

        /// <summary>
        /// Yields objects matching the route parameters for your specific Razor Page implementation
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<object>> GetRouteParamsAsync();
    }
}
