using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data;
using Devart.Common;
using Devart.Data.MySql;

/// Custom
using Singularity.ORM.Enum;
using Singularity.ORM.Reader;
using Singularity.ORM.Events;
using Singularity.ORM.Exceptions;
using Singularity.ORM.Validation;

namespace Singularity.ORM.SQL
{
    public class SQLprovider : IDisposable
    {
        private static volatile SQLprovider instance;
        private SQLTransaction transaction;
        public MySqlConnection Connection { get; set; }
        protected MySqlTransaction Transaction { get; set; }
        public ProviderCredentials Credentials { get; set; }
        internal static HybridDictionary byType;
        private static readonly string[] reserved = new string[] { "PropertyChanged", "Item", "Id" };

        private FindByAction actionFindBy;
        public FindByAction FindBy
        {
            get { return actionFindBy; }
        }

        private FindByIdAction actionFindById;
        public FindByIdAction FindById
        {
            get { return actionFindById; }
        }

        private GelAllRowsAction actionGetAllRows;
        public GelAllRowsAction GetRows
        {
            get { return actionGetAllRows; }
        }

        // CRUD Impl
        public static readonly string create = "INSERT INTO {0} ({1}) VALUES ({2})";
        public static readonly string read = "SELECT {0} FROM {1} WHERE {2}";
        public static readonly string update = "UPDATE {0} SET {1} WHERE id = {2}";
        public static readonly string delete = "DELETE FROM {0} WHERE id = {1}";

        #region Singleton pattern Impl
        public static SQLprovider GetInstance(ProviderCredentials credentials)
        {
            if (instance == null)
            {
                instance = new SQLprovider(credentials);
            }

            return instance;
        }
        #endregion

        #region (..) ctor
        public SQLprovider()
        {
            // Read Config

            var config = (SingularityProviderSection)
                 ConfigurationManager.GetSection("SingularityProvider");
            if (config == null)
                throw new ArgumentNullException("SingularityProvider", "Brakujący parametr ");
            ProviderCredentials __credentials = new ProviderCredentials()
            {
                Server    = config.ServerAddress,
                Port      = config.PortNumber,
                User      = config.UserName,
                Password  = config.Password,
                Database  = config.Database,
                Collation = config.Collation
            };

            // Connection 

            this.Credentials = __credentials;
            Connect(__credentials);

            // Handle Access to data methods

            RepositoryHandle();
        }
        public SQLprovider(ProviderCredentials credentials)
        {
            // Connection 

            this.Credentials = credentials;
            Connect(credentials);

            // Handle Access to data methods

            RepositoryHandle();

        }

        internal void RepositoryHandle()
        {
            actionGetAllRows = new GelAllRowsAction(this);
            actionFindById = new FindByIdAction(this);
            actionFindBy = new FindByAction(this);
            SQLprovider.byType = new HybridDictionary();
        }

        internal void Connect(ProviderCredentials credentials)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = credentials.ConnectionString;
            conn.LocalFailover = true;
            conn.ConnectionLost += new ConnectionLostEventHandler(conn_ConnectionLost);
            this.Connection = conn;
        }
        #endregion


        static void conn_ConnectionLost(object sender, ConnectionLostEventArgs e)
        {
            if (e.Cause == ConnectionLostCause.Execute)
            {
                if (e.Context == ConnectionLostContext.None)
                    e.RetryMode = RetryMode.Reexecute;
                else
                    e.RetryMode = RetryMode.Raise;
            }
            else
                e.RetryMode = RetryMode.Raise;
        }

        /// <summary>
        /// Get unique key ID value for particular Entity object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int getId(object obj)
        {
            FieldInfo fid = obj.GetType().GetField
                ("id", BindingFlags.NonPublic | BindingFlags.Instance);
            return fid == null ? -1 : (int)fid.GetValue(obj);
        }

        /// <summary>
        /// Registers a new instance of Entity object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="trans"></param>
        public void AddNew(object obj, ISqlTransaction trans)
        {
            setAction(FieldState.Added, obj, trans);
        }

        /// <summary>
        /// Initalize editing on a particular Entity object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="trans"></param>
        internal void SetEdit(object obj, ISqlTransaction trans)
        {
            setAction(FieldState.Modified, obj, trans);
        }

        /// <summary>
        /// Marking up Entity object as signed to delete in current transaction
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="trans"></param>
        internal void Delete(object obj, ISqlTransaction trans)
        {
            setAction(FieldState.Deleted, obj, trans);
        }

        /// <summary>
        /// Primary method invoking suitable action depending of current state of Entity
        /// </summary>
        /// <param name="state"></param>
        /// <param name="obj"></param>
        /// <param name="trans"></param>
        private void setAction(FieldState state, object obj, ISqlTransaction trans)
        {
            Type type = obj.GetType();
            int id = 0;
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(type)
                        && typeof(IBaseRecord).IsAssignableFrom(type))
            {
                INotifyPropertyChanged impl = (INotifyPropertyChanged)obj;
                ((EntityProvider)obj)["State"] = state;
                ((EntityProvider)obj)["CurrentTransaction"] = trans;
                impl.PropertyChanged += impl_PropertyChanged;
                id = getId(impl);
                SQLTransaction _trans = (SQLTransaction)trans;
                _trans.QueueInTransaction.Enqueue
                       (new BusinessObject(state,
                        (IBaseRecord)obj, type, string.Empty, null));
                this.transaction = _trans;
                this.Transaction = _trans.Transaction;
            }
            else
                throw new InvalidCastException("Dany obiekt nie jest pełnoprawnym obiektem biznesowym");
        }

        /// <summary>
        /// Event handler gather changes upon Entity objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void impl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DbPropertyChangedEventArgs args = (DbPropertyChangedEventArgs)e;
            object row = args.Row;
            Type type = row.GetType();
            int id = getId(row);
            string field = args.PropertyName;

            BusinessObject business = this.transaction.QueueInTransaction.Where
                        (q => q.Row == row && q.PropertyName == String.Empty).FirstOrDefault();
            if (business == null)
                return;
            var item = business.Clone();
            item.PropertyName = field;
            item.Value = getValueBy(row, field);
            this.transaction.QueueInTransaction.Enqueue(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static object getValueBy(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty
                (propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (pi != null)
            {
                if (pi.GetValue(obj) == null)
                    return null;
                else if (pi.GetValue(obj).GetType() == typeof(bool))
                    return (bool)pi.GetValue(obj) == true ? 1 : 0;
                else if (pi.GetValue(obj).GetType().IsEnum)
                    return (int)pi.GetValue(obj);
                else if (typeof(IBaseRecord).IsAssignableFrom(pi.GetValue(obj).GetType()))
                    return ((IBaseRecord)pi.GetValue(obj)).Id;
                else
                    return pi.GetValue(obj);
            }
            return null;
        }

        internal static T PropertiesFromType<T>(Type type)
        {
            return (T)propertiesFromType(typeof(T), type);
        }

        private static object propertiesFromType(Type result, Type type)
        {
            FieldInfo[] props = type.GetFields
                   (BindingFlags.NonPublic | BindingFlags.Instance);
            if (result == typeof(List<Type>))
            {
                List<Type> arr = new List<Type>();
                foreach (FieldInfo prp in props)
                {
                    if (!prp.Name.Equals("PropertyChanged"))
                        arr.Add(prp.FieldType);
                }
                return arr;
            }
            else if (result == typeof(IDictionary<string, Type>))
            {
                Dictionary<string, Type> dict = new Dictionary<string, Type>();
                foreach (FieldInfo prp in props)
                {
                    if (!prp.Name.Equals("PropertyChanged")
                        && !dict.ContainsKey(prp.Name))
                    {
                        dict.Add(prp.Name, prp.FieldType);
                    }
                }
                return dict;
            }
            return null;
        }


        internal static string getTableName(Type type)
        {
            FieldInfo fi = type.GetField
                ("tableName", BindingFlags.NonPublic | BindingFlags.Static);
            return ((string)fi.GetValue(null)).ToLower();
        }

        internal static string getTableNameByField(Type type, string property, out Type returnedType)
        {
            PropertyDescriptor pd = BusinessValidator.GetPropertyDescriptor(type, property.Trim());
            returnedType = pd.PropertyType;
            return typeof(IBaseRecord).IsAssignableFrom(pd.PropertyType)
                ? getTableName(pd.PropertyType)
                : string.Empty;
        }

        internal static IEnumerable<object> getValues(IBaseRecord row, string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                yield return getValueBy(row, fields[i].Replace("`", ""));
            }
        }

        internal static string[] getPropertiesNames(Type type)
        {
            PropertyInfo[] props = type.GetProperties
                   (BindingFlags.Public | BindingFlags.Instance);

            List<string> arr = new List<string>();
            foreach (PropertyInfo pi in props)
            {
                if (!reserved.Contains(pi.Name) && pi.CanWrite)
                    arr.Add(String.Format("`{0}`", pi.Name));
            }
            return arr.ToArray();
        }

        internal static string[] getFieldsNames(Type type)
        {
            string table = getTableName(type);
            List<string> fields = new List<string>();
            var _fields = PropertiesFromType<IDictionary<string, Type>>(type);
            _fields.Keys.ToList().ForEach(delegate(string s)
            {
                if (s.StartsWith("_"))
                {
                    s = s.Remove(0, 1);
                    fields.Add(String.Format("{0}.{1}", table, s));
                }
                else
                    fields.Add(String.Format("{0}.{1}", table, s));
            });
            return fields.ToArray();
        }

        [Obsolete]
        public static IEnumerable<Type> GetAllEntities()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                foreach (Type t in asm.GetTypes().Where
                (type => type.IsSubclassOf(typeof(EntityProvider))))
                {
                    yield return t;
                }
            }
        }

        #region Transactions

        public ISqlTransaction BeginTransaction()
        {
            return this.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }

        public ISqlTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (this.Connection.State == ConnectionState.Closed)
                this.Connection.Open();
            SQLTransaction transaction = new SQLTransaction(this.Connection, this);
            transaction.OnCommit += new CommitEventHandler(transaction_OnCommit);
            transaction.BeginTransaction(isolationLevel);
            return transaction;
        }

        void transaction_OnCommit(object sender, CommitEventArgs e)
        {
            var arr = e.Collection;
            arr.ToList().ForEach(delegate(IGrouping<IBaseRecord, BusinessObject> g)
            {
                IEnumerable<string> err = BusinessValidator.Validate(g.Key);
                string result = err.Count() == 0 ? string.Empty : err.FirstOrDefault();
                if (!String.IsNullOrEmpty(result))
                    throw new EntityValidationException(result);
            });
        }
        #endregion

        #region IDisposable Impl

        void IDisposable.Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                instance = null;
                GC.SuppressFinalize(this);
            }
        }

        #endregion



    }
}
