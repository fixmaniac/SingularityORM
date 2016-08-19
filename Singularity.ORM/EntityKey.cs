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
