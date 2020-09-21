using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Razor.Language;
using System.IO;

namespace SodaPop.RazorPagesSitemap
{
    public class RazorPagesSitemapMiddleware : IMiddleware
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly RazorProjectFileSystem _razorProjectFileSystem;
        private readonly RazorPagesSitemapOptions _options;
        private readonly IEnumerable<ISitemapRouteParamProvider> _routeParamProviders;
        private readonly Regex _ignoreExpression;
        private readonly LinkGenerator _linkGenerator;

        public RazorPagesSitemapMiddleware(
            IActionDescriptorCollectionProvider actionDescriptors,
            IOptions<RazorPagesSitemapOptions> options,
            IEnumerable<ISitemapRouteParamProvider> routeParamProviders,
            LinkGenerator linkGenerator)
        {
            _actionDescriptorCollectionProvider = actionDescriptors;
            _options = options.Value;
            _razorProjectFileSystem = RazorProjectFileSystem.Create(_options.RootDirectoryPath);
            _routeParamProviders = routeParamProviders;

            if (!string.IsNullOrEmpty(_options.IgnoreExpression))
            {
                _ignoreExpression = new Regex(_options.IgnoreExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

                if (_ignoreExpression?.IsMatch(page.ViewEnginePath) == true)
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
                        var rpi = _razorProjectFileSystem.GetItem(page.RelativePath, null);
                        if (rpi.Exists)
                        {
                            node.LastModified = new FileInfo(rpi.PhysicalPath).LastWriteTimeUtc;
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

            //writing synchronously to the response body is not supported by asp.net
            //so we write to a memory stream first, which does support writing back asynchronously
            var serializer = new XmlSerializer(typeof(Sitemap));
            using var ms = new MemoryStream();
            serializer.Serialize(ms, sitemap);
            ms.Position = 0;
            await ms.CopyToAsync(context.Response.Body);
        }
    }
}
