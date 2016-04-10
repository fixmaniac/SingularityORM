using System;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    [XmlRoot("entity", Namespace = "http://singularity-orm/schema/entity_struct.xsd")]
    public class Entity : WithName
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof (Entity));
        [XmlAttribute("namespace")]
        public string Namespace;
        [XmlAttribute("tablename")]
        public string TableName;
        [XmlElement("using")]
        public string[] Usings;
        private Enumerator[] enums;
        private Field[] fields;       
        

        [XmlElement("field")]
        public Field[] Fields
        {
            get
            {
                return this.fields != null ? this.fields : new Field[0];
            }
            set
            {
                this.fields = value;
            }
        }

        [XmlElement("enum")]
        public Enumerator[] Enums
        {
            get
            {
                return this.enums != null ? this.enums : new Enumerator[0];
            }
            set
            {
                this.enums = value;
            }
        }

        public static Entity Load(XmlReader xml)
        {
            return (Entity)Entity.serializer.Deserialize(xml);
        }
    }
}
