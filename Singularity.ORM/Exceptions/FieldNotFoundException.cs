using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.SQL;

namespace Singularity.ORM.Exceptions
{
    public class FieldNotFoundException : KeyNotFoundException
    {
        public readonly string FieldName;
        public FieldNotFoundException(string field, Type type) :
            base(String.Format("Pole o nazwie {0} nieznalezione w tabeli {1}", field, SQLprovider.getTableName(type)))
        {
            this.FieldName = field;
        }
    }
}
