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
        /// <summary>
        ///  Provider
        /// </summary>
        protected SQLprovider Context
        {
            get
            {
                return ((SQLTransaction)this.Transaction).Provider;
            }
        }

        /// <summary>
        /// Find by Primary Key ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual T FindByID<T>(int id) where T : EntityProvider
        {
            EntityProvider result = ((EntityProvider)this.Context.FindById[typeof(T), id]);
            if (result != null)
                SetTRansaction(result);
            return (T)result;
        }

        /// <summary>
        /// Find by indexer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual T FindBy<T>(string field, object value) where T : EntityProvider
        {
            EntityProvider result = ((EntityProvider)this.Context.FindBy[typeof(T), field, value]);
            if (result != null)
                SetTRansaction(result);
            return (T)result;

        }

        /// <summary>
        /// Get collection using multiple condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get first matched record using multiple condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get last matched record using multiple condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get collection with limit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add new instance of entity object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        protected virtual void Add<T>(EntityProvider row) where T : EntityProvider, new()
        {
            this.Context.AddNew((T)row, this.Transaction);
        }

        /// <summary>
        /// Return type of entity
        /// </summary>
        public virtual Type RowType
        {
            get
            {
                return typeof(EntityProvider);
            }
        }

        /// <summary>
        /// Set transaction
        /// </summary>
        /// <param name="obj"></param>
        private void SetTRansaction(EntityProvider obj)
        {
            obj["CurrentTransaction"] = this.Transaction;
        }

        /// <summary>
        /// (..ctor)
        /// </summary>
        /// <param name="transaction"></param>
        protected EntityTable(ISqlTransaction transaction)
        {
            this.Transaction = transaction;
        }
    }
}
