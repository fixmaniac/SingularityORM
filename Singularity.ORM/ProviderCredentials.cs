using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.Enum;

namespace Singularity.ORM
{
    #region Credentials
    [Serializable]
    public class ProviderCredentials
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public Collation Collation { get; set; }

        internal string ConnectionString
        {
            get
            {
                return String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};Charset=utf8",
                    this.Server,
                    this.Port,
                    this.Database,
                    this.User,
                    this.Password);
            }
        }
    }

    #endregion

}
