using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Devart.Data.MySql;
using Devart.Common;
using Singularity.ORM.Enum;
using Singularity.ORM.Events;

namespace Singularity.ORM.SQL
{
    public class SQLTransaction : ISqlTransaction, IDisposable
    {
        public System.Data.IsolationLevel Isolationlevel { get; private set; }
        public MySqlConnection Connection { get; private set; }
        public SQLprovider Provider { get; private set; }
        public int LastInsertedId { get; set; }
        internal MySqlTransaction Transaction { get; set; }
        internal Queue<BusinessObject> QueueInTransaction { get; private set; }
        private TableCollection tables;
        public TableCollection Tables
        {
            get
            {
                return this.tables;
            }
        }


        internal SQLTransaction(MySqlConnection connection, SQLprovider provider)
        {
            this.Connection = connection;
            this.Provider = provider;
            this.QueueInTransaction = new Queue<BusinessObject>();
            this.tables = new TableCollection(this);
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            this.Transaction = this.Connection.BeginTransaction(isolationLevel);
            this.Isolationlevel = isolationLevel;
        }

        IQueryable<IGrouping<IBaseRecord, BusinessObject>> Collection
        {
            get
            {
                return this.QueueInTransaction.AsQueryable().GroupBy(q => q.Row);
            }
        }

        private string[] GetQuerends(ref Dictionary<BusinessObject, string> diff)
        {
            Queue<string> arr = new Queue<string>();
            var collection = this.QueueInTransaction.AsQueryable().GroupBy(q => q.Row);
            var _diff = new Dictionary<BusinessObject, string>();
            collection.ToList().ForEach(delegate(IGrouping<IBaseRecord, BusinessObject> group)
            {
                string result = "";
                BusinessObject bus = group.FirstOrDefault();
                string   table  = SQLprovider.getTableName(bus.Type);
                string[] fields = SQLprovider.getPropertiesNames(bus.Type);
                object[] values = SQLprovider.getValues(bus.Row, fields).ToArray();
                var dict = group.Where(g => !string.IsNullOrEmpty(g.PropertyName))
                    .ToDictionary(g => g.PropertyName, g => g.Value);
                if (bus.State == FieldState.Modified
                                 && dict.Count == 0)
                    return;
                switch (bus.State)
                {
                    case FieldState.Added: result = SQLprovider.create;
                        result = String.Format(result,
                                   table,
                                   String.Join(",", fields),
                                   String.Join(",", values.ToList().ConvertAll
                                     (v => String.Format("'{0}'", v)).ToArray()));                       
                        break;
                    case FieldState.Modified: result = SQLprovider.update;
                        result = String.Format(result,
                                   table, dict.JoinFormat(",", "`{0}` = '{1}'"),
                                           bus.ID);
                        break;
                    case FieldState.Deleted: result = SQLprovider.delete;
                        result = String.Format(result, table, bus.ID);
                        break;

                }
                _diff.Add(bus, result);
                arr.Enqueue(result);
            });
            diff = _diff;
            return arr.ToArray();
        }

        private IEnumerable<SQLQuery> PrepareCommands(params string[] queries)
        {
            foreach (string query in queries)
            {
                yield return new SQLQuery()
                {
                    CommandText = query,
                    Connection = this.Connection,
                    Transaction = this.Transaction,
                };
            }
        }

        internal int GetLastInsertedId()
        {
            SQLQuery cmd = new SQLQuery();
            cmd.CommandText = "Select LAST_INSERT_ID()";
            cmd.Connection = this.Connection;
            var result = cmd.ExecuteReader();
            result.Read();
            int w = result.GetUInt16(0);
            result.Dispose();
            return w;
        }

        public event CommitEventHandler OnCommit;
        protected virtual void onCommit(CommitEventArgs e)
        {
            if (OnCommit != null)
                OnCommit(this, e);
        }

        private void isChange(IBaseRecord rec, ref SQLQuery cmd, FieldState state)
        {
            var group = this.Collection.Where
                    (i => i.Key == rec).FirstOrDefault();
            BusinessObject bus = group.FirstOrDefault();
            KeyValuePair<string, object>[] kvp = group.
                 Where(p => !String.IsNullOrEmpty(p.PropertyName)).ToList().ConvertAll
                   (p => new KeyValuePair<string, object>(p.PropertyName, p.Value)).ToArray();

            string   table  = SQLprovider.getTableName(bus.Type);
            string[] fields = SQLprovider.getPropertiesNames(bus.Type);
            object[] values = SQLprovider.getValues(bus.Row, fields).ToArray();
            string result = "";
            switch (state)
            {
                case FieldState.Added: result = SQLprovider.create;
                    result = String.Format(result,
                               table,
                               String.Join(",", fields),
                               String.Join(",", values.ToList().ConvertAll
                                   (v => String.Format("'{0}'", v)).ToArray()));                   
                    break;
                case FieldState.Modified: result = SQLprovider.update;
                    result = String.Format(result,
                               table, kvp.JoinFormat(",", "`{0}` = '{1}'"),
                                       rec.Id);
                    break;
            }
            cmd.CommandText = result;
        }

        public void Commit()
        {
            onCommit(new CommitEventArgs(this, Collection));
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
                IsolationLevel level = this.Transaction.IsolationLevel;
                this.Transaction.Dispose();
                this.Transaction = this.Connection.BeginTransaction(level);
            }
            Dictionary<BusinessObject, string> marker = null;
            PrepareCommands(GetQuerends(ref marker)).ToList().ForEach(delegate(SQLQuery cmd)
            {
                BusinessObject businessObj = marker.Where
                          (kvp => kvp.Value == cmd.CommandText).FirstOrDefault().Key;
                IBaseRecord rec = businessObj.Row;
                if (businessObj.State == FieldState.Added
                 || businessObj.State == FieldState.Modified)
                {
                    isChange(rec, ref cmd, businessObj.State);
                }
                //cmd.BeforeExecute += new BeforeExecuteEventHandler(cmd_BeforeExecute);
                cmd.ExecuteNonQuery();
                this.LastInsertedId = GetLastInsertedId();
                rec.Id = this.LastInsertedId;
            });
            this.Transaction.Commit();
            this.Connection.Close();
        }

        void cmd_BeforeExecute(object sender, SqlQueryEventArgs e)
        {
           
        }
        public void Rollback()
        {
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
                IsolationLevel level = this.Transaction.IsolationLevel;
            }
            this.Transaction.Rollback();
        }

        void IDisposable.Dispose()
        {
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
                IsolationLevel level = this.Transaction.IsolationLevel;
            }
            this.Transaction.Dispose();
        }
    }
}
