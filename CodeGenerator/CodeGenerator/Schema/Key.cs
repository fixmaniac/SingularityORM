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
        ForeignKey,
        IndexKey
    }
    public class Key : WithName
    {
       
        [XmlAttribute("type")]
        public KeyType Type;
        //[XmlAttribute("column")]
        //public string Column;
        [XmlAttribute("table")]
        public string Table;
        [XmlAttribute("children")]
        public string Children;
        [XmlElement("column")]
        public KeyColumn[] Columns;
        
    }
}
