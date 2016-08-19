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
        /// Basic properties
        protected MySqlConnection Connection { get; set; }
        protected SQLprovider Provider { get; set; }
        protected List<Type> entities { get; set; }

        /// <summary>
        ///  Create SQL query 
        /// </summary>
        /// <returns></returns>
        internal SQLQuery CreateCommand()
        {
            return new SQLQuery(String.Empty);
        }

        /// <summary>
        ///  Return result 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="cond"></param>
        /// <returns></returns>
        protected T Result<T>(Type type, SQLCondition cond)
        {
            try
            {
                return (T)get(typeof(T), type, cond);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected T Result<T>(Type type, SQLCondition cond, params string[] columns)
        {
            try
            {
                return (T)get(typeof(T), type, cond, columns);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method that verify is field belongs to a specific table
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public bool IsProperly(string tablename, string propertyname)
        {
            Type type = null;
            return IsProperly(tablename, propertyname, ref type);
        }
        /// <summary>
        /// Method that verify is field belongs to a specific table
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="propertyname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsProperly(string tablename, string propertyname, ref Type type)
        {
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
        ///  Method verified is property has a database relation (key)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool IsForeignKey(Type source, string propertyName, ref Type type, ref string tableName)
        {
            var props = source.GetProperties().Where(prop
                      => Attribute.IsDefined(prop, typeof(ForeignKeyAttribute)));
            if (props.Count() == 0)
                return false;
            PropertyInfo pi = props.Where(p
                      => p.Name == propertyName.Trim()).FirstOrDefault();
            if (pi == null)
                return false;
            ForeignKeyAttribute attr = (ForeignKeyAttribute)
                                 pi.GetCustomAttribute(typeof(ForeignKeyAttribute));
            tableName = attr.TableName.ToLower();
            type = pi.PropertyType;
            return true;
        }

        /// <summary>
        /// SQL Conditions array
        /// </summary>
        string[] conds = new string[] 
                                        { 
                                            "AND", 
                                            "OR" 
                                        };

        /// <summary>
        /// Record conditions array
        /// </summary>
        string[] conds1 = new string[] 
                                        { 
                                            "=", 
                                            "<>", 
                                            "like", 
                                            "not like", 
                                            "is null", 
                                            "is not null", 
                                            " in" 
                                        };


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
            entities = new List<Type>();
            string query = SQLprovider.read;
            string[] arr = condition.Split(conds,
                           StringSplitOptions.RemoveEmptyEntries);
            List<string> arr2 = new List<string>();
            foreach (string s1 in arr)
            {
                if (conds1.Any(c => s1.IndexOf(c) != -1))
                    arr2.Add(s1.Split(conds1, StringSplitOptions.None)[0]);
            }

            List<string> joinedTables = new List<string>();
            List<string> addedTables = new List<string>();
            List<Tuple<string, string>> joinedValues = new List<Tuple<string, string>>();


            addedTables.Add(table);

            foreach (string s in arr2)
            {
                Type currentType = type;
                entities.Add(type);
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
                string _tbl = "";
                bool foundKey = false;

                foreach (string _field in fields)
                {
                    foreach (string _tablename in addedTables)
                    {                       
                        bool isKey = IsProperly(_tablename, _field, ref _type);
                        if (isKey && _type != null
                            && typeof(EntityProvider).IsAssignableFrom(_type)
                            && SQLprovider.getTableName(_type) == joinedTableName)
                        {                           
                            entities.Add(_type);
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
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="sourceValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetConvert(Type sourceType, object sourceValue, out object value)
        {
            IEntityMapper EntityMapper = null;
            value = null;
            if (sourceType == typeof(bool)) 
                {
                  EntityMapper = new BoolMapper
                                  (Provider, sourceValue);
                  value = EntityMapper.Map();
                }
                else if (sourceType.IsEnum && sourceValue.GetType() == typeof(string)) 
                {
                    var genericType = typeof(EnumMapper<>).MakeGenericType(sourceType);
                    EntityMapper = (IEntityMapper)Activator.CreateInstance(genericType,
                    new object[] {
                                Provider,
                                sourceValue
                                });
                    value = EntityMapper.Map();                                
                }
                else if (typeof(INotifyPropertyChanged).IsAssignableFrom(sourceType) && 
                         typeof(IBaseRecord).IsAssignableFrom(sourceType))  
                {
                    EntityMapper = new BusinessMapper
                                (Provider, sourceValue, sourceType);
                    value = EntityMapper.Map();
                }
                else if (sourceType == typeof(Guid))
                {
                    EntityMapper = new GuidMapper
                                    (Provider, sourceValue);
                    value = EntityMapper.Map();
                }
                else if (sourceType == typeof(string) 
                          && sourceValue.GetType() == typeof(byte[]))  {
                    EntityMapper = new ByteStringMapper
                                    (Provider, sourceValue);
                    value = EntityMapper.Map();
                }
            return value != null;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        private object get(Type resultType, Type type, SQLCondition cond)
        {
            return get(resultType, type, cond, null);
        }

        /// <summary>
        /// Primary data reader
        /// </summary>
        /// <param name="resultType">Type of returned value</param>
        /// <param name="type">Entity type</param>
        /// <param name="cond">Condition <typeparamref name="SQLCondition"/></param>
        /// <returns>Specific data type record or collection</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        private object get(Type resultType, Type type, SQLCondition cond, params string[] columns)
        {

            if (this.Connection.State == ConnectionState.Closed)
                this.Connection.Open();
            
            string tableName = SQLprovider.getTableName(type);
            var _columns = SQLprovider.PropertiesFromType<IDictionary<string, Type>>(type);

            SQLQuery cmd = this.CreateCommand();
            List<Type> _props = new List<Type>();
            List<string> _fields = new List<string>();
           
            foreach (KeyValuePair<string, Type> kvp in _columns) {
                string _columnName = kvp.Key;
                Type _columnType = kvp.Value;
                if (_columnName.StartsWith("_")) {
                    _columnName = _columnName.Remove(0, 1);
                    _fields.Add(String.Format("{0}.{1}", tableName, _columnName));
                }
                else
                    _fields.Add(String.Format("{0}.{1}", tableName, _columnName));
                
                _props.Add(kvp.Value);
            }

            Type[] props = _props.ToArray();
            string[] fields = _fields.ToArray();
                     
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
            this.Provider.OnQuery(new ProviderQueryEventArgs(this.Provider, cmd.CommandText));
            object result = cmd.ExecuteReader();
            MySqlDataReader reader = result as MySqlDataReader;           
            List<object> values = new List<object>();
            if (reader.FieldCount != props.Length)
                throw new InvalidDatabaseStructureException(tableName);

            while (reader.Read())
            {
                object obj = null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i == 0 && obj == null)
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    if (obj != null)
                    {                       
                        string field = !char.IsNumber(reader.GetName(i)[0])
                                ? reader.GetName(i) 
                                : string.Format("_{0}", reader.GetName(i));
                        object value = null;
                        if (reader.GetValue(i).GetType() == typeof(DBNull)) {
                            if (props[i] == typeof(int))
                                value = 0;
                            else if (props[i] == typeof(string))
                                value = string.Empty;
                        }
                        else {
                            if (!TryGetConvert(props[i], reader.GetValue(i), out value))
                                    value = reader.GetValue(i);
                        }
                        try
                        {                            
                            type.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).
                                 SetValue(obj, value);
                        }
                        catch (Exception)
                        {                            
                        }
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
