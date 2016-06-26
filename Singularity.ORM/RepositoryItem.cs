using System;
using System.Linq;
using System.Reflection;


namespace Singularity.ORM
{
    /// <summary>
    /// 
    /// </summary>
    public class RepositoryItem
    {
        /// <summary>
        ///  Repository name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tables count
        /// </summary>
        public int NumbersOfTables { get; set; }        
        /// <summary>
        /// Repository object instance
        /// </summary>
        public EntityRepository Repository { get; set; }
        /// <summary>
        /// Properties collection
        /// </summary>
        public PropertyInfo[] TablesCollection { get; set; }

        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="repository"></param>
        public RepositoryItem(EntityRepository repository)
        {
            PropertyInfo[] entityTables = repository.GetType().GetProperties
                (BindingFlags.Instance | BindingFlags.Public).
                    Where(p => !p.CanWrite).ToArray();

            this.Repository = repository;
            this.Name = repository.GetType().Name.Replace("Repository", "");
            this.NumbersOfTables  = entityTables.Length;
            this.TablesCollection = entityTables;
        }
    }
}
