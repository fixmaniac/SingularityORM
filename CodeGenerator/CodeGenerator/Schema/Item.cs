using System;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public class Item 
    {
        [XmlAttribute("value")]
        public string Value;
    }
}
