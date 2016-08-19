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
using Devart.Data.MySql;
using Singularity.ORM.Events;


namespace Singularity.ORM.SQL
{
    internal class SQLQuery : MySqlCommand
    {
        public SQLQuery()
            : base()
        {
        }
        public SQLQuery(string commandText)
            : base(commandText)
        {
            this.CommandText = commandText;
        }

        public SQLQuery(string commandText, MySqlConnection connection)
            : base(commandText, connection)
        {
            this.CommandText = commandText;
            this.Connection = connection;
        }


        public event BeforeExecuteEventHandler BeforeExecute;
        protected virtual void onBeforeExecute(SqlQueryEventArgs e)
        {
            if (BeforeExecute != null)
                BeforeExecute(this, e);
        }

        public new string CommandText
        {
            get
            {
                return base.CommandText;
            }
            set
            {
                base.CommandText = value;
            }
        }

        public new MySqlDataReader ExecuteReader()
        {
            onBeforeExecute(new SqlQueryEventArgs(this.Connection));
            return base.ExecuteReader();
        }
    }
}
