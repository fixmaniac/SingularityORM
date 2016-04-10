using System;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public class Field : WithName, IComparable, IComparable<Field>
    {
        internal Entity entity = (Entity)null;

        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("length")]
        public int Length;
        [XmlAttribute("description")]
        public string Description;
        [XmlAttribute("mendatory")]
        public bool Mendatory;

        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo((Field)obj);
        }

        public int CompareTo(Field field)
        {
            return string.Compare(this.Name, field.Name, true);
        }
    }
}
