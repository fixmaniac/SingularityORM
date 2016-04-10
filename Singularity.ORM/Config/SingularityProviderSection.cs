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
