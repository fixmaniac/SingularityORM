using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public class Key : WithName
    {
        [XmlAttribute("relationtype")]
        public KeyRelationType RelationType;
    }
}
