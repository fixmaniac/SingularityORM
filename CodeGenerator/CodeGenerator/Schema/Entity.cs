using System;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    [XmlRoot("entity", Namespace = "http://singularity-orm/schema/entity_struct.xsd")]
    public class Entity : WithName
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(Entity));
        [XmlAttribute("namespace")]
        public string Namespace;
        [XmlAttribute("tablename")]
        public string TableName;
        [XmlAttribute("repository")]
        public string Repository;
        [XmlElement("using")]
        public string[] Usings;
        private Key[] keys;
        private Field[] fields;
        private Enumerator[] enums;



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

        [XmlElement("key")]
        public Key[] Keys
        {
            get
            {
                return this.keys != null ? this.keys : new Key[0];
            }
            set
            {
                this.keys = value;
            }
        }

        public static Entity Load(XmlReader xml)
        {
            return (Entity)Entity.serializer.Deserialize(xml);
        }
    }
}
