using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devart.Data.MySql;
using Singularity.ORM.SQL;
using Singularity.ORM.Map;
using Singularity.ORM.Events;
using Singularity.ORM.Exceptions;
using Singularity.ORM.Conditions;

namespace Singularity.ORM.Reader
{
    #region SQL Actions
    /// <summary>
    ///  
    /// </summary>
    [DataObject]
    public abstract class SQLActionBase
    {
        protected MySqlConnection Connection { get; set; }
        protected SQLprovider Provider { get; set; }
        IEnumerable<Type> entities;

        internal SQLQuery CreateCommand()
        {
            return new SQLQuery(String.Empty);
        }

        protected T Result<T>(Type type, SQLCondition cond)
        {
            return (T)get(typeof(T), type, cond);
        }
        public bool IsProperly(string tablename, string propertyname)
        {
            Type type = null;
            return IsProperly(tablename, propertyname, ref type);
        }
        public bool IsProperly(string tablename, string propertyname, ref Type type)
        {
            entities = SQLprovider.GetAllEntities();
            Type entity = entities.Where(en => tablename == SQLprovider.getTableName(en)).FirstOrDefault();
            if (entity != null)
            {
                PropertyInfo pi = entity.GetProperty(propertyname.Trim(),
                    BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    type = pi.PropertyType;
                    return true;
                }

            }
            return false;
        }
        /// <summary>
        /// Build join condition responsible for translation nested conditions
        /// </summary>
        /// <param name="type">Entity type</param>
        /// <param name="table">Table name</param>
        /// <param name="condition">Text translated condition</param>
        /// <param name="newCondition">ref new condition</param>
        /// <returns></returns>
        private string BuildJoinCondition(Type type, string table, string condition, out string newCondition)
        {

            string query = SQLprovider.read;
            string[] conds = new string[] { "AND", "OR" };
            string[] conds1 = new string[] { "=", "<>", "like", "not like", "is null", "is not null" };
            string[] arr = condition.Split(conds,
                        StringSplitOptions.RemoveEmptyEntries);
            List<string> arr2 = new List<string>();
            foreach (string s1 in arr)
            {
                if (conds1.Any(c => s1.IndexOf(c) != -1))
                    arr2.Add(s1.Split(conds1, StringSplitOptions.None)[0]);
            }
            List<Tuple<string, string>> joinedValues = new List<Tuple<string, string>>();
            List<string> joinedTables = new List<string>();
            List<string> addedTables = new List<string>();
            addedTables.Add(table);
            foreach (string s in arr2)
            {
                Type currentType = type;
                string currentTable = "";
                if (s.Contains('.') && s.Split('.').Length > 1)
                {
                    string[] fds = s.Split('.');
                    for (int i = 0; i < fds.Length; ++i)
                    {
                        currentTable = SQLprovider.getTableNameByField(currentType, fds[i], out currentType);
                        if (string.IsNullOrEmpty(currentTable)
                                || currentTable.Equals(table))
                            continue;
                        if (!joinedTables.Contains(currentTable))
                        {
                            joinedTables.Add(currentTable);
                        }
                        joinedValues.Add(Tuple.Create(currentTable, s));
                    }
                }
            }

            for (int i = 0; i < joinedTables.Count; ++i)
            {
                string joinedTableName = joinedTables[i];
                string _key = "";
                string _table = "";
                var tuple = joinedValues.Where(val => val.Item1 == joinedTableName).LastOrDefault();
                var fields = tuple.Item2.Split('.');
                Type _type = null;
                bool foundKey = false;
                foreach (string _field in fields)
                {
                    foreach (string _tablename in addedTables)
                    {
                        bool check = IsProperly(_tablename, _field, ref _type);
                        if (check && _type != null
                            && typeof(EntityProvider).IsAssignableFrom(_type)
                            && SQLprovider.getTableName(_type) == joinedTableName)
                        {
                            _table = _tablename;
                            _key = _field;
                            addedTables.Add(joinedTableName);
                            foundKey = true;
                            break;
                        }
                    }
                    if (foundKey)
                        break;
                    else continue;
                }
                query = query.Replace("WHERE", String.Format("JOIN {0} ON {1}.{2} = {0}.Id",
                        joinedTableName,
                        _table,
                        _key));
                query = query.Insert(query.IndexOf("{2}"), "WHERE ");
            }

            //joinedTables.ToList().ForEach(delegate(string joinedTableName)
            //{
            //    var tuple = joinedValues.Where(val => val.Item1 == joinedTableName).LastOrDefault();
            //    // var key = tuple.Item2.Split('.')[tuple.Item2.Split('.').Length - 2];
            //    var key = tuple.Item2.Split('.')[0];
            //    query = query.Replace("WHERE", String.Format("JOIN {0} ON {1}.{2} = {0}.Id",
            //        joinedTableName,
            //        table,
            //        key));
            //    query = query.Insert(query.IndexOf("{2}"), "WHERE ");
            //});

            joinedValues.ForEach(delegate(Tuple<string, string> tuple)
            {
                if (!tuple.Item2.Contains('.'))
                    return;
                string[] items = tuple.Item2.Split('.');
                bool isProperly = IsProperly(tuple.Item1, items[items.Length - 1]);
                if (!isProperly)
                    return;
                condition = condition.Replace
                    (tuple.Item2, String.Format(" {0}.{1} ", tuple.Item1, items[items.Length - 1]));

            });
            newCondition = condition;
            return query;
        }

        /// <summary>
        /// Primary data reader
        /// </summary>
        /// <param name="resultType">Type of returned value</param>
        /// <param name="type">Entity type</param>
        /// <param name="cond">Condition <typeparamref name="SQLCondition"/></param>
        /// <returns>Specific data type record or collection</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        private object get(Type resultType, Type type, SQLCondition cond)
        {

            if (this.Connection.State == ConnectionState.Closed)
                this.Connection.Open();
            SQLQuery cmd = this.CreateCommand();
            string tableName = SQLprovider.getTableName(type);
            string[] fields = SQLprovider.getFieldsNames(type);
            if (cond.Condition.Contains('.'))
            {
                string joinCondition = string.Empty;
                string joinQuery = BuildJoinCondition(type, tableName, cond.Condition, out joinCondition);
                cmd.CommandText = string.Format(joinQuery,
                String.Join(",", fields),
                tableName,
                joinCondition);
            }
            else
            {
                cmd.CommandText = string.Format(SQLprovider.read,
                    String.Join(",", fields),
                    tableName,
                    cond.Condition);
            }
            cmd.Connection = this.Connection;
            cmd.BeforeExecute += new BeforeExecuteEventHandler(cmd_BeforeExecute);
            object result = cmd.ExecuteReader();
            MySqlDataReader reader = result as MySqlDataReader;
            Type[] props = SQLprovider.PropertiesFromType<List<Type>>(type).ToArray();
            List<object> values = new List<object>();
            if (reader.FieldCount != props.Length)
                throw new InvalidDatabaseStructureException(tableName);

            while (reader.Read())
            {
                object obj = null;
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (i == 0 && obj == null)
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    if (obj != null)
                    {
                        string field = !char.IsNumber(reader.GetName(i)[0])
                            ? reader.GetName(i) : string.Format("_{0}", reader.GetName(i));
                        object value = null;
                        if (reader.GetValue(i).GetType() == typeof(DBNull))
                        {
                            if (props[i] == typeof(int))
                                value = 0;
                            else if (props[i] == typeof(string))
                                value = string.Empty;
                        }
                        else
                        {
                            IEntityMapper EntityMapper = null;
                            if (props[i] == typeof(bool))
                            {
                                EntityMapper = new BoolMapper
                                            (Provider, reader.GetValue(i));
                                value = EntityMapper.Map();
                            }
                            else if (props[i].IsEnum && reader.GetValue(i).GetType() == typeof(string))
                            {
                                var genericType = typeof(EnumMapper<>).MakeGenericType(props[i]);
                                EntityMapper = (IEntityMapper)Activator.CreateInstance(genericType,
                                new object[] {
                                            Provider,
                                            reader.GetString(i)
                                         });
                                value = EntityMapper.Map();
                            }
                            else if (typeof(INotifyPropertyChanged).IsAssignableFrom(props[i])
                                  && typeof(IBaseRecord).IsAssignableFrom(props[i]))
                            {
                                EntityMapper = new BusinessMapper
                                           (Provider, reader.GetValue(i), props[i]);
                                value = EntityMapper.Map();
                            }
                            else if (props[i] == typeof(string) &&
                                    reader.GetValue(i).GetType() == typeof(byte[]))
                            {
                                EntityMapper = new ByteStringMapper
                                               (Provider, reader.GetValue(i));
                                value = EntityMapper.Map();
                            }
                            else
                                value = reader.GetValue(i);
                        }
                        type.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).
                             SetValue(obj, value);
                    }
                    if (i == reader.FieldCount - 1)
                        values.Add(obj);
                }
            }
            cmd.Connection.Close();
            if (resultType == typeof(IEnumerable<object>))
                return values;
            else
                if (resultType == typeof(IBaseRecord))
                    return values.Cast<IBaseRecord>().FirstOrDefault();
                else
                    return null;
        }

        void cmd_BeforeExecute(object sender, SqlQueryEventArgs e)
        {
            MySqlConnection conn = e.Connection;
            if (conn.State == System.Data.ConnectionState.Broken
                || conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }
    }
    #endregion
}
