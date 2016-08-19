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
