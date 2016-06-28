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
        /// <param name="collection"></param>
        private static object collectEntities(ISqlTransaction transaction)
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
        public static void Initialize(ISqlTransaction transaction)
        {
            collectEntities(transaction);
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
            this.TableName = (string)table.EntityType.GetField
                                ("tableName", 
                                   BindingFlags.NonPublic | 
                                   BindingFlags.Static).GetValue(null);
            this.Transaction = transaction;

            Map = new Hashtable();                       
            Map.Add(typeof(System.String), "VARCHAR");
            Map.Add(typeof(System.Int16), "SMALLINT");
            Map.Add(typeof(System.Int32), "INT");
            Map.Add(typeof(System.Int64), "BIGINT");  
            Map.Add(typeof(System.Guid), "CHAR(36)");
            Map.Add(typeof(System.Double), "DOUBLE");
            Map.Add(typeof(System.TimeSpan), "TIME");
            Map.Add(typeof(System.Decimal), "DECIMAL");
            Map.Add(typeof(System.DateTime), "DATETIME");
            
        }       

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

        internal void CreateTable(string tableName)
        {
            SQLTable table = new SQLTable(tableName, this.Table.EntityType);

            IDictionary<string, Type> fields = SQLprovider.PropertiesFromType
                <IDictionary<string, Type>>(this.Table.EntityType);
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
                                ColumnType = String.Format("ENUM({0})",String.Join(",",values))
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
            table.BuildSQL();
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
                        (KeyType.ForeignKey, pars.FirstOrDefault().Name);                       
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
        /// <param name="fieldname"></param>
        /// <returns></returns>
        private bool isMendatory(string fieldname)
        {
            string propertyName = char.ToUpper(fieldname[0]) + fieldname.Substring(1);
            PropertyInfo pi = this.Table.EntityType.GetProperty(propertyName, 
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
                string[] entityFieldsNames  = SQLprovider.getFieldsNames(source.Table.EntityType);
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
        
        
        public void BuildSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Insert(0, String.Format("CREATE TABLE '{0}' ( \r\n",TableName));
             foreach (SQLTableColumn col in this.Columns) {
                 var isNotNull = col.IsMendatory ? "NOT NULL" : String.Empty;
                 sql.AppendFormat("'{0}' {1} {2},  \r\n", col.ColumnName, col.ColumnType, isNotNull);
            }
             foreach (SQLTableKey key in this.Keys) {
                 switch (key.Type) {
                     case KeyType.PrimaryKey : sql.AppendFormat("PRIMARY KEY ({0})\r\n", key.Fields[0]);  
                             break;
                     case KeyType.ForeignKey : sql.AppendFormat("FOREIGN KEY ({0}) REFERENCES {1} (id) ON DELETE SET NULL \r\n",key.Fields[0],
                         SQLprovider.getTableName(this.RowType));
                             break;
                     case KeyType.IndexerKey : sql.AppendFormat("INDEX ({0})\r\n", String.Join(",", key.Fields)); 
                             break;
                 }
             }             
             sql.Append(") ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8");
             throw new Exception(sql.ToString());
        }
    }

    internal class SQLTableColumn : ISQLTableColumn {

        public string ColumnName { get; private set; }
        public string ColumnType { get; set; }
        public bool IsMendatory { get; set; }

        public SQLTableColumn(string name)
        {
            this.ColumnName = name;
        }
    }

    internal class SQLTableKey : ISQLTableKey {
        public KeyType Type { get; set; }
        public string[] Fields {get; set; }

        public SQLTableKey(KeyType type, params string[] fields)
        {
            this.Type = type;
            this.Fields = fields;
        }
    }

    public interface ISQLTableColumn {        
        string ColumnType {get; set;}
    }

    public interface ISQLTableKey {       
        KeyType Type {get; set;}
    }
  
}
