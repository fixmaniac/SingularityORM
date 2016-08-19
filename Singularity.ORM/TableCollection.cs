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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM
{
    public sealed class TableCollection : IEnumerable, IEnumerable<EntityTable>
    {
        private readonly Dictionary<Type, EntityTable> byType = new Dictionary<Type, EntityTable>();
        private readonly ISqlTransaction transaction;
        public EntityTable this[Type type]
        {
            get
            {
                return this.loadByType(type);
            }
        }

        internal TableCollection(ISqlTransaction transaction)
        {
            this.transaction = transaction;
        }

        private EntityTable NewTable(Type type)
        {
            return (EntityTable)type.GetConstructor(new Type[]
			{
				typeof(ISqlTransaction)
			}).Invoke(new object[]
			{
				this.transaction
			});
        }

        internal void add(EntityTable table)
        {
            Type type = table.GetType();
            if (this.byType.ContainsKey(type))
            {
                throw new Exception(string.Concat(new string[]
				{
					"Ponowna rejestracja tabeli '",
					table.GetType().ToString(),
					"' w tej samej transakcji. Prawdopodobnie użyto błędnie instrukcji new ",
					table.GetType().ToString(),
					"EntityTable(tansaation)."
				}));
            }
            this.byType[type] = table;
        }

        private EntityTable loadByType(Type type)
        {
            EntityTable result;
            if (!this.byType.TryGetValue(type, out result))
            {
                //if (!SQLprovider.byType.Contains(type))
                //{
                //    throw new ArgumentException("Niezarejestrowany moduł '" + type.Name + "'");
                //}
                result = this.NewTable(type);
            }
            return result;
        }

        public IEnumerator<EntityTable> GetEnumerator()
        {
            foreach (EntityTable table in this.byType.Values)
            {
                yield return table;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.byType.Values.GetEnumerator();
        }
    }
}
