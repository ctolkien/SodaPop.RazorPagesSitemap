namespace SodaPop.RazorPagesSitemap
{
    public class RazorPagesSitemapOptions
    {
        /// <summary>
        /// When true, with set the <lastmod>value</lastmod> to be based on the time the file  was last written to disk
        /// This only works when using FileBased Razor file provider.
        /// </summary>
        public bool BaseLastModOnLastModifiedTimeOnDisk { get; set; } = true;

        /// <summary>
        /// This prevents duplicates
        /// </summary>
        public bool IgnorePathsEndingInIndex { get; set; } = true;

        /// <summary>
        /// Regex to ignore certain paths
        /// </summary>
        public string IgnoreExpression { get; set; }
    }
}
