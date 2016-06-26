using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM
{
    public abstract class EntityRepository
    {
        public ISqlTransaction Transaction 
        { 
            get; 
            set; 
        }

        public EntityRepository()
        {
        }

        public static T GetInstance<T>(ISqlTransaction transaction) where T : EntityRepository, new()
        {
            T result = Activator.CreateInstance<T>();
            result.Transaction = transaction;
            transaction.Provider.Repositories.Add(new RepositoryItem(result));
            return result;
        }
    }
}
