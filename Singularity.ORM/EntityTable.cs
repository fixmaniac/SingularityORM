using System;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;
using Singularity.ORM.Conditions;

namespace Singularity.ORM
{
    public abstract class EntityTable
    {
        protected ISqlTransaction Transaction { get; set; }
        protected SQLprovider Context
        {
            get
            {
                return ((SQLTransaction)this.Transaction).Provider;
            }
        }
        protected virtual T FindByID<T>(int id) where T : EntityProvider
        {
            EntityProvider result = ((EntityProvider)this.Context.FindById[typeof(T), id]);
            if (result != null)
                SetTRansaction(result);
            return (T)result;
        }

        protected virtual T FindBy<T>(string field, object value) where T : EntityProvider
        {
            EntityProvider result = ((EntityProvider)this.Context.FindBy[typeof(T), field, value]);
            if (result != null)
                SetTRansaction(result);
            return (T)result;

        }

        protected virtual IEnumerable<T> GetRows<T>(SQLCondition condition) where T : EntityProvider
        {
            IEnumerable<EntityProvider> result =
                this.Context.GetRows[typeof(T), condition].Cast<EntityProvider>();
            result.ToList().ForEach(delegate(EntityProvider entity)
            {
                SetTRansaction(entity);
            });
            return result.Cast<T>();
        }

        protected virtual T GetFirst<T>(SQLCondition condition) where T : EntityProvider
        {
            if (condition == SQLCondition.Empty) {
                condition &= new RecordCondition.NotEqual("Id", 0);
            }
                condition &= new RecordCondition.Sort(SortOrder.ASC);
                condition &= new RecordCondition.Limit(1);
            EntityProvider result = ((EntityProvider)this.Context.GetRows[typeof(T), condition].FirstOrDefault());
            if (result != null)
                SetTRansaction(result);
            return (T)result;
        }

        protected virtual T GetLast<T>(SQLCondition condition) where T : EntityProvider
        {
            if (condition == SQLCondition.Empty) {
                condition &= new RecordCondition.NotEqual("Id", 0);
            }
                condition &= new RecordCondition.Sort(SortOrder.DESC);
                condition &= new RecordCondition.Limit(1);
            EntityProvider result = ((EntityProvider)this.Context.GetRows[typeof(T), condition].FirstOrDefault());
            if (result != null)
                SetTRansaction(result);
            return (T)result;
        }

        protected virtual IEnumerable<T> GetLimited<T>(SQLCondition condition, int limit) where T : EntityProvider
        {
            condition &= new RecordCondition.Limit(limit);
            IEnumerable<EntityProvider> result =
                   this.Context.GetRows[typeof(T), condition].Cast<EntityProvider>();
            result.ToList().ForEach(delegate(EntityProvider entity)
            {
                SetTRansaction(entity);
            });
            return result.Cast<T>();
        }

        protected virtual void Add<T>(EntityProvider row) where T : EntityProvider, new()
        {
            this.Context.AddNew((T)row, this.Transaction);
        }


        private void SetTRansaction(EntityProvider obj)
        {
            obj["CurrentTransaction"] = this.Transaction;
        }

        protected EntityTable(ISqlTransaction transaction)
        {
            this.Transaction = transaction;
        }
    }
}
