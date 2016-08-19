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
        [XmlAttribute("unique")]
        public bool Unique;

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
