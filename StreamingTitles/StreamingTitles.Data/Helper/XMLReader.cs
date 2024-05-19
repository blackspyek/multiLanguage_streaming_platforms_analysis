using Microsoft.AspNetCore.Http;
using System.Xml;
using System.Xml.Linq;

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
        public XMLReader()
        {

        }

        public XmlNodeList GetNodes()
        {
            return this.nodes;
        }

        public async Task<XDocument> ProcessData(IFormFile data)
        {
            if (data.Length > 0 && data.FileName.EndsWith(".xml"))
            {
                using (var reader = new StreamReader(data.OpenReadStream()))
                {
                    string xmlString = await reader.ReadToEndAsync();
                    XDocument doc = XDocument.Parse(xmlString);
                    return doc;
                }
            }
            else
            {
                throw new ArgumentException("Invalid file. Please upload a valid XML file.");
            }
        }
    }
}
