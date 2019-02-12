﻿using System.Collections.Generic;

namespace SodaPop.RazorPagesSitemap
{
    class SiteMapNodeComparer : IEqualityComparer<SitemapNode>
    {
        public bool Equals(SitemapNode x, SitemapNode y)
        {
            return x.Url == y.Url;
        }

        public int GetHashCode(SitemapNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
