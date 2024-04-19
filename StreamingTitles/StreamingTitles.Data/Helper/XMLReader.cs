using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public XmlNodeList GetNodes()
        {
            return this.nodes;
        }
    }
}
