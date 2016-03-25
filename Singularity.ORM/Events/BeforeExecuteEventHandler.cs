using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.MySql;

namespace Singularity.ORM.Events
{
    /// <summary>
    /// Handler for SQLQuery command before it execute  <see cref="SQLQuery"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void BeforeExecuteEventHandler(object sender, SqlQueryEventArgs e);

    public class SqlQueryEventArgs : EventArgs
    {
        public SqlQueryEventArgs(MySqlConnection connection)
        {
            this.Connection = connection;
        }
        public MySqlConnection Connection { get; set; }

    }
}
