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
