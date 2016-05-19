using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public enum KeyType
    {
        PrimaryKey,
        ForeignKey
    }
    public class Key
    {
        [XmlAttribute("type")]
        public KeyType Type;
        [XmlAttribute("column")]
        public string Column;
        [XmlAttribute("table")]
        public string Table;
        [XmlAttribute("children")]
        public string Children;
    }
}
