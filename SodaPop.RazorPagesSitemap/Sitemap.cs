using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SodaPop.RazorPagesSitemap
{
    [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        [XmlElement("url")]
        public List<SitemapNode> Nodes { get; set; }
    }

    public class SitemapNode : IXmlSerializable
    {
        public SitemapNode(string url)
        {
            Url = new Uri(url);
        }

        //Required for XmlSerializer
        public SitemapNode()
        {
        }

        public Uri Url { get; set; }

        public DateTime? LastModified { get; set; }

        public SitemapFrequency? Frequency { get; set; }

        public double Priority { get; set; }

        public XmlSchema GetSchema() => null;
        public void ReadXml(XmlReader reader) => throw new NotImplementedException();
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("loc", Url.ToString());

            if (LastModified.HasValue)
            {
                //https://www.sitemaps.org/protocol.html
                //https://www.w3.org/TR/NOTE-datetime
                writer.WriteElementString("lastmod", LastModified.Value.ToString("yyyy-MM-ddTHH:mmZ"));
            }

            if (Frequency.HasValue)
            {
                writer.WriteElementString("changefreq", Frequency.Value.ToString().ToLower()); //bad hack
            }

            if (Priority != default(double))
            {
                writer.WriteElementString("priority", Priority.ToString());
            }
        }

        /// <summary>
        /// Defines the volatility of content tracked via a sitemap node
        /// <seealso href="http://www.volume9inc.com/2009/03/15/sitemap-xml-why-changefreq-priority-are-important/" />
        /// </summary>
        public enum SitemapFrequency
        {
            Never,
            Yearly,
            Monthly,
            Weekly,
            Daily,
            Hourly,
            Always
        }
    }


}
