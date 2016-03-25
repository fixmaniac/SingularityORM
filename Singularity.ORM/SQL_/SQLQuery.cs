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
