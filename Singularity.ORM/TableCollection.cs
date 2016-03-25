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
