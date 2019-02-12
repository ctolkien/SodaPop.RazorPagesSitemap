using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
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
        private readonly IEnumerable<ISitemapRouteParamProvider> _routeParamProviders;
        private readonly Regex _ignoreExpression;
        private readonly LinkGenerator _linkGenerator;

        public RazorPagesSitemapMiddleware(
            IActionDescriptorCollectionProvider actionDescriptors,
            RazorProject razorProject,
            IOptions<RazorPagesSitemapOptions> options,
            IEnumerable<ISitemapRouteParamProvider> routeParamProviders,
            LinkGenerator linkGenerator)
        {
            _actionDescriptorCollectionProvider = actionDescriptors;
            _razorProject = razorProject;
            _options = options.Value;
            _routeParamProviders = routeParamProviders;

            if (!string.IsNullOrEmpty(_options.IgnoreExpression))
            {
                _ignoreExpression = new Regex(_options.IgnoreExpression, RegexOptions.Compiled);
            }

            _linkGenerator = linkGenerator;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var baseDomain = context.Request.Scheme + "://" + context.Request.Host;

            var nodes = new HashSet<SitemapNode>();

            var pages = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(x => x is PageActionDescriptor);
            foreach (PageActionDescriptor page in pages)
            {
                if (_options.IgnorePathsEndingInIndex && page.AttributeRouteInfo.Template.EndsWith("index", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (_ignoreExpression != null && _ignoreExpression.IsMatch(page.ViewEnginePath))
                {
                    continue;
                }

                var newNodes = new List<SitemapNode>();

                // add any dynamic nodes
                foreach(var provider in _routeParamProviders)
                {
                    if (await provider.CanSupplyParamsForPageAsync(page.ViewEnginePath)) {
                        foreach (var routeParams in await provider.GetRouteParamsAsync())
                        {
                            var node = new SitemapNode(baseDomain + _linkGenerator.GetPathByPage(page.ViewEnginePath, null, routeParams));
                            newNodes.Add(node);
                        }
                    }
                }

                // add node if we didn't add any dynamic ones
                if (newNodes.Count == 0)
                {
                    var node = new SitemapNode(baseDomain + _linkGenerator.GetPathByPage(page.ViewEnginePath));
                    newNodes.Add(node);
                }

                foreach (var node in nodes)
                {
                    if (_options.BaseLastModOnLastModifiedTimeOnDisk)
                    {
                        if (_razorProject.GetItem(page.RelativePath) is FileProviderRazorProjectItem rpi)
                        {
                            node.LastModified = rpi.FileInfo.LastModified.ToUniversalTime().DateTime;
                        }
                    }
                }

                foreach (var node in newNodes)
                {
                    nodes.Add(node);
                }
            }

            var sitemap = new Sitemap()
            {
                Nodes = nodes.ToList()
            };

            context.Response.ContentType = "application/xml";

            var serializer = new XmlSerializer(typeof(Sitemap));
            serializer.Serialize(context.Response.Body, sitemap);
        }
    }
}
