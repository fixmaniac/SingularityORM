﻿/*
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;
using Singularity.ORM.Conditions;

namespace Singularity.ORM.Reader
{
    /// <summary>
    /// Data reader returned all rows that match condition
    /// </summary>
    [DataObject]
    public class GelAllRowsAction : SQLActionBase
    {

        public GelAllRowsAction(SQLprovider provider)
        {
            base.Provider = provider;
            base.Connection = provider.Connection;
        }

        public virtual IEnumerable<object> this[Type type, SQLCondition cond]
        {
            get
            {
                return base.Result<IEnumerable<object>>(type, cond);
            }
        }
    }
}
