using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SodaPop.RazorPagesSitemap
{
    public static class RazorPagesSitemapMiddlewareExtensions
    {
        public static IApplicationBuilder UseRazorPagesSitemap(this IApplicationBuilder builder, string path = "/sitemap.xml")
        {
            return builder.Map(new PathString(path), x => x.UseMiddleware<RazorPagesSitemapMiddleware>());
        }

        public static IServiceCollection AddRazorPagesSitemap(this IServiceCollection services)
        {
            return services.AddRazorPagesSitemap(_ => new RazorPagesSitemapOptions());
        }

        public static IServiceCollection AddRazorPagesSitemap(this IServiceCollection services, Action<RazorPagesSitemapOptions> setupAction)
        {
            services.Configure(setupAction);
            services.AddTransient<RazorPagesSitemapMiddleware>();

            return services;
        }
    }
}
