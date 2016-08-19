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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

using Singularity.ORM.SQL;
using Singularity.ORM.Validation;

namespace Singularity.ORM
{
    /// <summary>
    /// 
    /// </summary>
    public enum ConvertionReasonType
    {
        TableNotExists = 1,
        KeysAdjustment = 2,
        ColumnsChange = 3,       
    }
    /// <summary>
    /// 
    /// </summary>
    public enum KeyType
    {
        PrimaryKey = 1,
        ForeignKey = 2,
        IndexerKey = 3
    }
    /// <summary>
    /// 
    /// </summary>
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    public static class SQLGenerator
    {
        private static readonly string showTables 
                      = "SHOW TABLES LIKE";
        private static readonly string showDatabase
                      = "SHOW DATABASES LIKE";
        private static readonly string columnsCollection
                      = "SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_NAME =";
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this RepositoryCollection<T> source, Action<T> action) 
          where T : RepositoryItem
        {
            foreach (T element in source)
                action(element);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <param name="action"></param>
        internal static void ForEach<T>(this RepositoryCollection<T> source, Predicate<T> filter, Action<T> action)
           where T : RepositoryItem
        {
            foreach (T t in source)
            {
                if (!filter(t))
                {
                    continue;
                }
                action(t);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <param name="action"></param>
        internal static void ForEach<T>(this IEnumerable<T> source, Predicate<T> filter, Action<T> action)
           where T : PropertyInfo
        {
            foreach (T t in source)
            {
                if (!filter(t))
                {
                    continue;
                }
                action(t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public static string JoinFormat(this string[] arr, string separator, string formatString)
        {
            return string.Join(separator, 
                      arr.Select(item => String.Format(formatString, item)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static string GetForeignKeyTableName(this Type type, string property)             
        {
           if (typeof(IBaseRecord).IsAssignableFrom(type)) {
                string name = char.ToUpper(property[0]) + property.Substring(1);
                PropertyInfo pi = type.GetProperty(name,
                                            BindingFlags.Instance |
                                            BindingFlags.Public |
                                            BindingFlags.IgnoreCase);
                if (pi != null) {
                    ForeignKeyAttribute fka = (ForeignKeyAttribute)
                                     pi.GetCustomAttribute(typeof(ForeignKeyAttribute));
                    return fka.TableName;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static int GetStringMaxLength(this Type type, string property)
        {
           if (typeof(IBaseRecord).IsAssignableFrom(type)) {
                string name = char.ToUpper(property[0]) + property.Substring(1);
                PropertyInfo pi = type.GetProperty(name,
                                            BindingFlags.Instance |
                                            BindingFlags.Public |
                                            BindingFlags.IgnoreCase);
                if (pi != null)
                {
                    TextMaxLengthAttribute tmla = (TextMaxLengthAttribute)
                                     pi.GetCustomAttribute(typeof(TextMaxLengthAttribute));
                    return tmla.MaxLength;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsEntityTable(this PropertyInfo prop)
        {

            Type type = prop.PropertyType; 
            return type.IsSubclassOf(typeof(EntityTable));
        }

        /// <summary>
        /// 
        /// </summary>
        public static Hashtable EntitiyItemCollections 
        { 
            get; 
            set; 
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        private static object CollectEntities(ISqlTransaction transaction)
        {            
            var collection = transaction.Provider.Repositories;
             collection.ForEach
                (rep  => rep.NumbersOfTables !=0, delegate(RepositoryItem item) {    
                                                 item.TablesCollection.ForEach                                                 
                    (pi   => pi.IsEntityTable(),  delegate(PropertyInfo pi) {
                        
                        MethodInfo mi = pi.PropertyType.GetMethod
                                        ("GetInstance", 
                                          BindingFlags.Public | 
                                          BindingFlags.Static);
                        EntityTable table = (EntityTable)mi.Invoke
                                      (null, new object[] { transaction });
                        if (table != null)  {                            
                            SQLBuilder builder = new SQLBuilder(table, transaction);                            
                            builder.Generate();                                                 
                        }
                });                                                                                                   
            });
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        private static void CreateEntitiesTables(ISqlTransaction transaction)
        {
            foreach (SQLTable table in EntitiyItemCollections.Keys)
            {                        
                string cmd = "";
                table.BuildSQL(transaction.Provider, ref cmd);
                table.ExecSQL(cmd, transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        private static void CreateEntitiesTablesKeys(ISqlTransaction transaction)
        {
            foreach (SQLTable table in EntitiyItemCollections.Keys)
            {                
                table.UpdateKeys(transaction);               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public static void Initialize(ISqlTransaction transaction)
        {
            SQLGenerator.EntitiyItemCollections = new Hashtable();
            CollectEntities(transaction);
            CreateEntitiesTables(transaction);
            CreateEntitiesTablesKeys(transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsTableExist(ISqlTransaction transaction, string name)
        {
            var dataReader = QueryReader(transaction, String.Format("{0} '{1}'", showTables, name));
            if (dataReader != null)
            {
                object result = null;
                dataReader.Read();
                try
                {
                    result = dataReader.GetValue(0);
                }
                catch (InvalidOperationException) { }                
                dataReader.Dispose();
                return !(result == null);                
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IEnumerable<string> getColumnsInfo(ISqlTransaction transaction, string name)
        {
            var dataReader = QueryReader(transaction, String.Format("{0} '{1}' AND TABLE_SCHEMA = '{2}'", 
                                            columnsCollection, 
                                            name, 
                                            transaction.Provider.Credentials.Database));
            if (dataReader != null)
            {
                while (dataReader.Read()) {
                    yield return dataReader.GetString(0);
                }
                dataReader.Dispose();
                
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string[] GetColumnsInfo(ISqlTransaction transaction, string name)
        {
            return getColumnsInfo(transaction, name).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static IDataReader QueryReader(ISqlTransaction transaction, string query)
        {
            if (transaction.Provider.Connection.State != ConnectionState.Open)
                transaction.Provider.Connection.Open();
            var result = new SQLQuery
            {
                CommandText = string.Format(query),
                Connection = transaction.Provider.Connection
            }.ExecuteReader();            
            return result;
        }        
    }

    
    /// <summary>
    /// 
    /// </summary>
    internal  class SQLBuilder
    {
        private System.Delegate handler;

        // properties
        public string TableName 
        { 
            get; 
            set; 
        }
        public EntityTable Table 
        { 
            get; 
            set; 
        }
        public ISqlTransaction Transaction 
        { 
            get; 
            set; 
        }
        internal Hashtable Map 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        public SQLBuilder(EntityTable table, ISqlTransaction transaction)
        {            
            this.Table = table;
            this.TableName = (string)table.RowType.GetField
                                ("tableName", 
                                   BindingFlags.NonPublic | 
                                   BindingFlags.Static).GetValue(null);
            this.Transaction = transaction;

            Map = new Hashtable();                       
            Map.Add(typeof(bool), "TINYINT(1)");
            Map.Add(typeof(System.Int16), "SMALLINT");
            Map.Add(typeof(System.Int32), "INT");
            Map.Add(typeof(System.Int64), "BIGINT");  
            Map.Add(typeof(System.Guid), "CHAR(36)");
            Map.Add(typeof(System.Double), "DOUBLE");
            Map.Add(typeof(System.TimeSpan), "TIME");
            Map.Add(typeof(System.Decimal), "DECIMAL");
            Map.Add(typeof(System.DateTime), "DATETIME");
            
        }       

        /// <summary>
        /// 
        /// </summary>
        public void Generate()
        {
            ConvertionReasonType? reason;
            if (TryGetIsRequired(this, out reason))
            {
                switch (reason)
                {
                    case ConvertionReasonType.TableNotExists: CreateTable(TableName);
                        break;
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        internal void CreateTable(string tableName)
        {
            SQLTable table = new SQLTable(tableName, this.Table.RowType);
            string cmd = String.Empty;
            IDictionary<string, Type> fields = SQLprovider.PropertiesFromType
                <IDictionary<string, Type>>(this.Table.RowType);
            foreach (KeyValuePair<string, Type> kvp in fields)
            {
                string fieldName = kvp.Key;
                Type   fieldType = kvp.Value;
                SQLTableColumn column = null;
                if (fieldName.Equals("id")) 
                {
                    column = new SQLTableColumn("id")
                    {
                        ColumnType = "INT(11)",
                        IsMendatory = true
                    };
                    table.Columns.Add(column);
                }
                else 
                {
                    if (fieldType.IsEnum) {
                            /// if enum
                            string[] values = System.Enum.GetNames(fieldType);
                            column = new SQLTableColumn(fieldName)
                            {
                                ColumnType = String.Format("ENUM({0})", values.JoinFormat(",", "'{0}'"))
                            };
                    }
                    else if (fieldType == typeof(String)) {
                            /// if string
                            column = new SQLTableColumn(fieldName)
                            {
                                ColumnType = String.Format
                                ("VARCHAR({0})", this.Table.RowType.GetStringMaxLength(fieldName))
                            };
                    }
                    else if (typeof(IBaseRecord).IsAssignableFrom(fieldType))  {
                            /// if entity relation
                            column = new SQLTableColumn(fieldName)
                            {
                                ColumnType = "INT(11)"                            
                            };
                    }                    
                    else  {
                            /// any
                            column = new SQLTableColumn(fieldName)
                            {
                                ColumnType = (string)Map[fieldType]
                            };
                    }

                    column.IsMendatory = isMendatory(fieldName);
                    table.Columns.Add(column);
                }
            }
            InitializeKeys(table);
            SQLGenerator.EntitiyItemCollections.Add(table, false);
            
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        internal void InitializeKeys(SQLTable table)
        {
            Type[] keys = this.Table.GetType().GetNestedTypes
                               (BindingFlags.Public).Where(t => typeof(EntityKey).IsAssignableFrom(t)).
                                           ToArray();
            SQLTableKey primaryKey = new SQLTableKey
                        (KeyType.PrimaryKey, "id");
            table.Keys.Add(primaryKey);
            foreach (Type type in keys) {
                SQLTableKey key = null;
                PropertyInfo pi = type.GetProperty("Item");
                ParameterInfo[] pars = pi.GetIndexParameters(); 
                if (pars.Length == 1 &&
                    pars.All(p => typeof(IBaseRecord).IsAssignableFrom(p.ParameterType))) {                       
                        key = new SQLTableKey
                            (KeyType.ForeignKey, pars.FirstOrDefault().Name) 
                            {
                                Name = GetKeyFieldName(type)
                            };                       
                } else {                    
                        key = new SQLTableKey
                            (KeyType.IndexerKey, pars.ToList().ConvertAll(p => p.Name).ToArray());
               }
               table.Keys.Add(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetKeyFieldName(Type type)
        {
            var fi = type.GetField("KeyFieldName", BindingFlags.Public | BindingFlags.Static);
            if (fi != null)
            {
                return fi.GetValue(null).ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        private bool isMendatory(string fieldname)
        {
            string propertyName = char.ToUpper(fieldname[0]) + fieldname.Substring(1);
            PropertyInfo pi = this.Table.RowType.GetProperty(propertyName, 
                BindingFlags.Public | 
                BindingFlags.Instance);
            if (pi != null) {
                MendatoryAttribute attr = (MendatoryAttribute)
                                 pi.GetCustomAttribute(typeof(MendatoryAttribute));
                return !(attr == null);
            }
             return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        /// <param name="source"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal bool TryGetIsRequired<TBuilder>(TBuilder source, out ConvertionReasonType? reason) 
            where TBuilder : SQLBuilder
        {
            reason = null;
            if (SQLGenerator.IsTableExist(source.Transaction, source.TableName))
            {
                string[] entityFieldsNames  = SQLprovider.getFieldsNames(source.Table.RowType);
                string[] sqltableFieldNames = SQLGenerator.GetColumnsInfo(source.Transaction, source.TableName);
                if (Enumerable.SequenceEqual(entityFieldsNames, sqltableFieldNames)) {
                     return false;
                }
                else {
                      reason = ConvertionReasonType.ColumnsChange;
                        return true;
                }
            }
            reason = ConvertionReasonType.TableNotExists;
            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SQLTable
    {        
        public ICollection<ISQLTableColumn> Columns { get; set; }
        public ICollection<ISQLTableKey> Keys { get; set; }
        public string TableName { get; private set; }
        public Type RowType { get; set; }

        public SQLTable(string tablename, Type rowtype)
        {
            this.TableName = tablename;
            this.RowType = rowtype;
            this.Columns = new List<ISQLTableColumn>();
            this.Keys = new List<ISQLTableKey>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="cmd"></param>
        public void BuildSQL(SQLprovider dbProvider, ref string cmd)
        {
            StringBuilder sql = new StringBuilder();
            sql.Insert(0, String.Format("CREATE TABLE {0} ( \r\n",TableName));
             foreach (SQLTableColumn col in this.Columns) {
                 var isNotNull = col.IsMendatory ? "NOT NULL" : String.Empty;
                 var isAutoIncrement = col.ColumnName.Equals("id") ? "AUTO_INCREMENT" : string.Empty;
                 sql.AppendFormat("{0} {1} {2} {3},", col.ColumnName, col.ColumnType, isNotNull, isAutoIncrement);
                 sql.Append(Environment.NewLine);
            }
             foreach (SQLTableKey key in this.Keys) {
                 switch (key.Type) {
                     case KeyType.PrimaryKey : sql.AppendFormat
                         ("PRIMARY KEY ({0}),\r\n", key.Fields[0]);  
                             break;                   
                 }
             }
             var index = sql.ToString().LastIndexOf(',');
             if (index >= 0)
                 sql.Remove(index, 1);
                 sql.AppendFormat(") ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET={0}", 
                     dbProvider.Credentials.Collation);

                 cmd = sql.ToString();
        }

        public void UpdateKeys(ISqlTransaction transaction)
        {            
            foreach (SQLTableKey key in this.Keys)
            {
                switch (key.Type)
                {
                    case KeyType.ForeignKey: {
                       var cmd = String.Format("ALTER TABLE {0} ADD FOREIGN KEY ({1}) REFERENCES {2} (id) \r\n", TableName,key.Name,
                                this.RowType.GetForeignKeyTableName(key.Name));
                        ExecSQL(cmd, transaction);
                        break;
                    }
                    case KeyType.IndexerKey: {
                        var cmd = String.Format("ALTER TABLE {0} ADD INDEX ({1})\r\n", TableName,String.Join(",", key.Fields));
                        ExecSQL(cmd, transaction);
                        break;
                    }
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="transaction"></param>
        public void ExecSQL(string cmd, ISqlTransaction transaction)
        {            
            if (transaction.Provider.Connection.State != ConnectionState.Open)
                transaction.Provider.Connection.Open();
            var query = new SQLQuery
            {
                CommandText = string.Format(cmd),
                Connection = transaction.Provider.Connection
            };
            query.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SQLTableColumn : ISQLTableColumn {

        public string ColumnName { get; private set; }
        public string ColumnType { get; set; }
        public bool IsMendatory { get; set; }

        public SQLTableColumn(string name)
        {
            this.ColumnName = name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SQLTableKey : ISQLTableKey {
        public String Name { get; set; }
        public KeyType Type { get; set; }
        public string[] Fields {get; set; }

        public SQLTableKey(KeyType type, params string[] fields)
        {
            this.Type = type;
            this.Fields = fields;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISQLTableColumn {        
        string ColumnType {get; set;}
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISQLTableKey {       
        KeyType Type {get; set;}
    }
  
}
