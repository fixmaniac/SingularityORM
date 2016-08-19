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
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// Exposes method responsible for convert Entity type fields
    /// </summary>
    public class BusinessMapper : EntityMapper<EntityProvider>
    {
        private Type type;
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="propType"></param>
        public BusinessMapper(SQLprovider context, Object obj, Type propType)
            : base(context, obj)
        {
            type = propType;
        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override EntityProvider Convert()
        {
            EntityProvider value = null;
            using (SQLprovider prv = new SQLprovider(Context.Credentials))
            {
                value = (EntityProvider)prv.FindById[type, (int)Row];
            }
            return value;
        }
    }
}
