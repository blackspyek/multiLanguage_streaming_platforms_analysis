using Microsoft.AspNetCore.Http;
using System.Xml;

namespace StreamingTitles.Data.Helper
{
    public class XMLReader
    {
        private string path;
        private XmlNodeList nodes;
        public XMLReader(string path)
        {
            XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(path);
            XmlElement root = doc.DocumentElement;
            this.nodes = root.ChildNodes;

        }
        public XMLReader(IFormFile file)
        {
            XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(file.OpenReadStream());
            XmlElement root = doc.DocumentElement;
            this.nodes = root.ChildNodes;
        }
        public XmlNodeList GetNodes()
        {
            return this.nodes;
        }
    }
}
