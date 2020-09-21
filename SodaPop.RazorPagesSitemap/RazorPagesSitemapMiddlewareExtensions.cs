using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Routing;
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

        //public static IEndpointRouteBuilder UseRazorPagesSitemap(this IEndpointRouteBuilder builder, string path = "sitemap.xml")
        //{
        //    return builder.Map(new PathString(path), x => x.CreateApplicationBuilder().UseMiddleware<RazorPagesSitemapMiddleware>());
        //}

        public static IServiceCollection AddRazorPagesSitemap(this IServiceCollection services)
        {
            return services.AddRazorPagesSitemap(setupAction: null);
        }

        public static IServiceCollection AddRazorPagesSitemap(this IServiceCollection services, Action<RazorPagesSitemapOptions> setupAction)
        {
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
            services.AddTransient<RazorPagesSitemapMiddleware>();

            return services;
        }
    }
}
