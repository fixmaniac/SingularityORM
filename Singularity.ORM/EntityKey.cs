using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.Conditions;

namespace Singularity.ORM
{
    public abstract class EntityKey : EntityTable
    {
        public EntityKey(ISqlTransaction transaction)
            : base(transaction)
        {

        }
        public new IEnumerable<T> GetRows<T>(SQLCondition condition) where T : EntityProvider
        {
            return base.GetRows<T>(condition);
        }

        public new T GetFirst<T>(SQLCondition condition) where T : EntityProvider
        {
            return base.GetFirst<T>(condition);
        }
    }
}
