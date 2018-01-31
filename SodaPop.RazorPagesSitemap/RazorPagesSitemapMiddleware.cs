using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SodaPop.RazorPagesSitemap
{
    public class RazorPagesSitemapMiddleware : IMiddleware
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly RazorProject _razorProject;
        private readonly RazorPagesSitemapOptions _options;
        private readonly Regex _ignoreExpression;

        public RazorPagesSitemapMiddleware(IActionDescriptorCollectionProvider actionDescriptors, RazorProject razorProject, IOptions<RazorPagesSitemapOptions> options)
        {
            _actionDescriptorCollectionProvider = actionDescriptors;
            _razorProject = razorProject;
            _options = options.Value;

            if (!string.IsNullOrEmpty(_options.IgnoreExpression))
            {
                _ignoreExpression = new Regex(_options.IgnoreExpression, RegexOptions.Compiled);
            }
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var baseDomain = context.Request.Scheme + "://" + context.Request.Host + "/";

            var sitemap = new Sitemap();

            var nodes = new List<SitemapNode>();

            foreach (PageActionDescriptor page in _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(x => x is PageActionDescriptor))
            {
                if (_options.IgnorePathsEndingInIndex && page.ViewEnginePath.EndsWith("/index"))
                {
                    continue;
                }

                if (_ignoreExpression != null && _ignoreExpression.IsMatch(page.ViewEnginePath))
                {
                    continue;
                }

                var node = new SitemapNode(baseDomain + page.AttributeRouteInfo.Template);

                if (_options.BaseLastModOnLastModifiedTimeOnDisk)
                {
                    if (_razorProject.GetItem(page.RelativePath) is FileProviderRazorProjectItem rpi)
                    {
                        node.LastModified = rpi.FileInfo.LastModified.ToUniversalTime().DateTime;
                    }
                }

                nodes.Add(node);
            }

            sitemap.Nodes = nodes;

            context.Response.Headers.Add("Content-Type", "application/xml");

            var serializer = new XmlSerializer(typeof(Sitemap));
            serializer.Serialize(context.Response.Body, sitemap);

            return Task.CompletedTask;
        }
    }
}
