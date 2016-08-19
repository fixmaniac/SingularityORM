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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            this.Transaction = this.Connection.BeginTransaction(isolationLevel);
            this.Isolationlevel = isolationLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        IQueryable<IGrouping<IBaseRecord, BusinessObject>> Collection
        {
            get
            {
                return this.QueueInTransaction.AsQueryable().GroupBy(q => q.Row);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diff"></param>
        /// <returns></returns>
        private string[] GetQuerends(ref Dictionary<BusinessObject, string> diff)
        {
            Queue<string> arr = new Queue<string>();
            var collection = this.QueueInTransaction.AsQueryable().GroupBy(q => q.Row);
            var _diff = new Dictionary<BusinessObject, string>();
            collection.ToList().ForEach(delegate(IGrouping<IBaseRecord, BusinessObject> group)
            {
                string result = "";
                BusinessObject bus = group.FirstOrDefault();
                string table = SQLprovider.getTableName(bus.Type);
                string[] fields = SQLprovider.getPropertiesNames(bus.Type);
                object[] values = SQLprovider.getValues(bus.Row, fields).ToArray();
                var dict = group.Where(g => !string.IsNullOrEmpty(g.PropertyName))
                    .GroupBy(p => p.PropertyName)
                    .Select(p => p.LastOrDefault())
                    .ToDictionary(
                    g => g.PropertyName,
                    g => g.Value is string
                      && ((string)g.Value).Contains("'") ? ((string)g.Value).Replace("'", "''") : g.Value);
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
                result = result.Replace("'(null)'", "null");
                _diff.Add(bus, result);
                arr.Enqueue(result);
            });
            diff = _diff;
            return arr.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        public event CommitEventHandler OnCommit;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void onCommit(CommitEventArgs e)
        {
            if (OnCommit != null)
                OnCommit(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="cmd"></param>
        /// <param name="state"></param>
        private void isChange(IBaseRecord rec, ref SQLQuery cmd, FieldState state)
        {
            var group = this.Collection.Where
                    (i => i.Key == rec).FirstOrDefault();
            BusinessObject bus = group.FirstOrDefault();
            KeyValuePair<string, object>[] kvp = group.
                 Where(p => !String.IsNullOrEmpty(p.PropertyName)).ToList().ConvertAll
                 (p => new KeyValuePair<string, object>(p.PropertyName,
                     p.Value is string && ((string)p.Value).Contains("'")
                     ? ((string)p.Value).Replace("'", "''") : p.Value)).ToArray();

            string table = SQLprovider.getTableName(bus.Type);
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
            result = result.Replace("'(null)'", "null");
            cmd.CommandText = result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            onCommit(new CommitEventArgs(this, Collection));
            IsolationLevel level = this.Transaction.IsolationLevel;
            RefreshTransaction(level);
            Dictionary<BusinessObject, string> marker = null;
            PrepareCommands(GetQuerends(ref marker)).ToList().ForEach(delegate(SQLQuery cmd)
            {
                BusinessObject businessObj = marker.Where
                          (kvp => kvp.Value == cmd.CommandText
                              && !kvp.Key.Commited).FirstOrDefault().Key;
                IBaseRecord rec = businessObj.Row;
                if (businessObj.State == FieldState.Added
                 || businessObj.State == FieldState.Modified)
                {
                    isChange(rec, ref cmd, businessObj.State);
                }
                this.Provider.OnQuery(new ProviderQueryEventArgs(this.Provider, cmd.CommandText));
                cmd.ExecuteNonQuery();
                this.LastInsertedId = GetLastInsertedId();
                rec.Id = this.LastInsertedId;
                businessObj.Commited = true;
            });
            this.Transaction.Commit();
            this.Connection.Close();
            RefreshTransaction(level);
            foreach (KeyValuePair<BusinessObject, string> kvp in marker)
            {
                ((EntityProvider)kvp.Key.Row).OnCommited();
            }
            this.Connection.Close();
        }

        void RefreshTransaction(IsolationLevel level)
        {
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
                this.Transaction.Dispose();
                this.Transaction = this.Connection.BeginTransaction(level);
            }
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
            this.Provider.Repositories.Clear();
            this.Transaction.Dispose();
        }
    }
}
