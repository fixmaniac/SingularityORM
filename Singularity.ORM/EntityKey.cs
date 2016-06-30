using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.Conditions;

namespace Singularity.ORM
{
    /// <summary>
    /// Key base class
    /// </summary>
    public abstract class EntityKey : EntityTable
    {
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="transaction"></param>
        public EntityKey(ISqlTransaction transaction)
            : base(transaction)
        {

        }

        /// <summary>
        /// Get collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public new IEnumerable<T> GetRows<T>(SQLCondition condition) where T : EntityProvider
        {
            return base.GetRows<T>(condition);
        }

        /// <summary>
        /// Get first row matched condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public new T GetFirst<T>(SQLCondition condition) where T : EntityProvider
        {
            return base.GetFirst<T>(condition);
        }
    }
}
