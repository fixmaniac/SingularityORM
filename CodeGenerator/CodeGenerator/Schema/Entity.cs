/*
 *  Copyright (c) 2016, Łukasz Ligocki.
 *  All rights reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at

 *  http://www.apache.org/licenses/LICENSE-2.0

 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

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
