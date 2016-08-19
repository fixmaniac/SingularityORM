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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Singularity.ORM.SQL
{
    public static class SQLExtensions
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        internal static void Action(this IBaseRecord obj,
                                   ISqlTransaction trans, [CallerMemberName]string actionMethodName = "")
        {
            SQLTransaction _transaction = (SQLTransaction)trans;
            MethodInfo mi = typeof(SQLprovider).GetMethod
                (actionMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null)
            {
                mi.Invoke(_transaction.Provider, new object[] { obj, trans });
            }
        }

        public static void Delete(this IBaseRecord obj)
        {
            ISqlTransaction trans =
                ((EntityProvider)obj)["CurrentTransaction"] as ISqlTransaction;
            if (trans != null)
                Action(obj, trans);
        }

        public static void Delete(this IBaseRecord obj, ISqlTransaction trans)
        {
            Action(obj, trans);
        }

        public static void SetEdit(this IBaseRecord obj)
        {
            ISqlTransaction trans =
                ((EntityProvider)obj)["CurrentTransaction"] as ISqlTransaction;
            if (trans != null)
                Action(obj, trans);
        }

        public static void SetEdit(this IBaseRecord obj, ISqlTransaction trans)
        {
            Action(obj, trans);
        }



        public static string JoinFormat(this IEnumerable<KeyValuePair<string, object>> list, string separator,
                                   string formatString)
        {
            formatString = string.IsNullOrWhiteSpace(formatString) ? "{0}='{1}'" : formatString;
            return string.Join(separator,
                                 list.Select(item =>
                                     string.Format(formatString, item.Key,
                                     typeof(IBaseRecord).IsAssignableFrom(item.Value.GetType())
                                     ? ((IBaseRecord)item.Value).Id : item.Value)));
        }

    }
}
