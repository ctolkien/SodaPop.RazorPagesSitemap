# SodaPop.RazorPagesSitemap

Generates a sitemap based on the layout of your Razor Pages website.

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
    options.BaseLastModOnLastModifiedTimeOnDisk = true; //default == true. This will set the <lastmod /> element to be based on the last modified time on disk
    options.IgnorePathsEndingInIndex = true; //default == true. This will avoid adding duplicate from matching `/mypath` and `/mypath/index`
    options.IgnoreExpression = @"error\/\d{3}$"; //default is null. Allows you to opt out certain patch matches based on regular expressions. In this case we're skipping the pages matching /error/123
});
```

By default, the sitemap will be available at `/sitemap.xml`. You can customize this in your `Configure` method:

```csharp
app.UseRazorPagesSitemap("/myAlternatePath");
```

