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
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityMapper<T> : IMapper<T>, IEntityMapper
    {
        // properties
        public SQLprovider Context { get; set; }
        public Object Row { get; set; }

        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public EntityMapper(SQLprovider context, Object obj)
        {
            this.Context = context;
            this.Row = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract T Convert();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Map()
        {
            return Convert();
        }
    }              
}
