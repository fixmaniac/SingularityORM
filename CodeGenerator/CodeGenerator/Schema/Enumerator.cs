using System;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public class Enumerator : WithName
    {
        [XmlElement("value")]
        public string[] Items;
    }
}
