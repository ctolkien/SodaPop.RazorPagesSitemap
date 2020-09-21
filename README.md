# SodaPop.RazorPagesSitemap

Generates a sitemap based on the layout of your Razor Pages website.

### Status

[![Build Status](https://dev.azure.com/chadtolkien/RazorPagesSitemap/_apis/build/status/ctolkien.SodaPop.RazorPagesSitemap?branchName=master)](https://dev.azure.com/chadtolkien/RazorPagesSitemap/_build/latest?definitionId=1&branchName=master)

## Install

```
dotnet add package SodaPop.RazorPagesSitemap
```

## Setup

In your `ConfigureServices` method add:

```csharp
services.AddRazorPagesSitemap();
```

In your `Configure` method add (ideally _before_ MVC):
```csharp
app.UseRazorPagesSitemap();
```

## Configuration

You can pass options when configuring the middleware to customize behaviour:

```csharp
services.AddRazorPagesSitemap(options =>
{
    // This will set the <lastmod /> element to be based on the last modified time on disk
    options.BaseLastModOnLastModifiedTimeOnDisk = true; // default is true

    // This will avoid adding duplicate from matching `/mypath` and `/mypath/index`
    options.IgnorePathsEndingInIndex = true; // default is true

    // Allows you to opt out certain patch matches based on regular expressions. In this case
    // we're skipping the pages matching /error/123
    options.IgnoreExpression = @"error\/\d{3}$"; // default is null
});
```

By default, the sitemap will be available at `/sitemap.xml`. You can customize this in your `Configure` method:

```csharp
app.UseRazorPagesSitemap("/myAlternatePath");
```

## Handling Dynamic Page Routes

For Razor Pages which work against dynamic route parameters, you can implement `ISitemapRouteParamProvider` to supply the appropriate route parameters to generates sitemap nodes. This scenario is useful if you content is generated from a database.

### Example

```csharp
    public class ProductSitemapProvider : ISitemapRouteParamProvider
    {
        private readonly MyProductsDatabase _context;

        public class ProductSitemapProvider(MyProductsDatabase context)
        {
            _context = context;
        }

        public Task<bool> CanSupplyParamsForPageAsync(string pagePath)
        {
            return Task.FromResult(pagePath == "/Products");
        }

        public async Task<IEnumerable<object>> GetRouteParamsAsync()
        {
            var products = await _context.Products.ToListAsync();

            var routeParams = new List<object>();
            foreach(var product in products)
            {
                //given a `@page /products/{id}/{slug}`
                routeParams.Add(new { id = product.Id, slug = product.Slug  });
            }

            return routeParams;
        }
    }
```

Then register this in `Startup`

```csharp
services.AddTransient<ISitemapRouteParamProvider, ProductsSitemapProvider>();
```


