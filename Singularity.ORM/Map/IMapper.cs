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

namespace Singularity.ORM.Map
{
    /// <summary>
    /// Exposes methods that map miscallenoeus field types
    /// </summary>
    public interface IEntityMapper
    {
        SQLprovider Context { get; set; }
        Object Row { get; set; }
        object Map();
    }

    /// <summary>
    /// Exposes a method that convert IDataReader type to .NET OOP type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The element type of the returned value</typeparam>
    public interface IMapper<T>
    {
        T Convert();
    }
}
