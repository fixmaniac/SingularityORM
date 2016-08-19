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
using System.Configuration;
using System.Xml;
using Singularity.ORM.Enum;


namespace Singularity.ORM
{
    class SingularityProviderSection : ConfigurationSection
    {
        [ConfigurationProperty("ServerAddress", IsRequired = true)]
        //[StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 80)]
        public string ServerAddress
        {
            get { return (string)this["ServerAddress"]; }
            set { this["ServerAddress"] = value; }
        }

        [ConfigurationProperty("PortNumber", DefaultValue = 3306)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 65535, MinValue = 1)]
        public int PortNumber
        {
            get { return (int)this["PortNumber"]; }
            set { this["PortNumber"] = value; }
        }

        [ConfigurationProperty("UserName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("Database", IsRequired = true)]
        public string Database
        {
            get { return (string)this["Database"]; }
            set { this["Database"] = value; }
        }

        [ConfigurationProperty("Encrypted")]
        public bool Encrypted
        {
            get { return (bool)this["Encrypted"]; }
            set { this["Encrypted"] = value; }
        }

        [ConfigurationProperty("Collation", DefaultValue = Collation.UTF8)]
        public Collation Collation
        {
            get { return (Collation)this["Collation"]; }
            set { this["Collation"] = value; }
        }
    }
}
