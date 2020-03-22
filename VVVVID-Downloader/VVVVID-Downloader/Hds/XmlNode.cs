using System.Collections.Generic;
using System.Linq;

namespace VVVVID_Downloader.Hds
{
    public class XmlNode
    {
        private readonly string _xml;
        private string _innerString = null;
        public XmlNode(string s)
        {
            _xml = s;
            Tag = s.ReMatch("<([^? ]+?)[ >]");
        }

        public string Tag { get; }

        public string InnerString =>
            _innerString ?? (_innerString = _xml.ReMatch($@"<{Tag}.*?>\s*([\s\S]*?)\s*</{Tag}>"));

        public IEnumerable<XmlNode> Nodes =>
            InnerString.ReMatches(@"<([^? ]+).*?>[\s\S]*?</\1>", 0).Select(s => new XmlNode(s));

        public string this[string attribute] => _xml.ReMatch($"<{Tag}.*?{attribute}=\"(.*?)\".*?>");

        public static implicit operator XmlNode(string s) => new XmlNode(s);
    }
}
