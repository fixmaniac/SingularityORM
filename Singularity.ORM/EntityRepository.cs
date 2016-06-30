using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM
{
    /// <summary>
    /// Repository base class
    /// </summary>
    public abstract class EntityRepository
    {
        /// <summary>
        /// Transaction
        /// </summary>
        public ISqlTransaction Transaction 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// (...ctor)
        /// </summary>
        public EntityRepository()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static T GetInstance<T>(ISqlTransaction transaction) where T : EntityRepository, new()
        {
            T result = Activator.CreateInstance<T>();
            result.Transaction = transaction;
            transaction.Provider.Repositories.Add(new RepositoryItem(result));
            return result;
        }
    }
}
