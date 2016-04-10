using System;
using System.Xml.Serialization;

namespace CodeGenerator.Schema
{
    public class WithName
    {
        private string name;

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public WithName()
        {
        }

        public WithName(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
