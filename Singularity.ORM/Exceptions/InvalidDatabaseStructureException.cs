using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Exceptions
{
    public class InvalidDatabaseStructureException : Exception
    {
        public readonly string Table;
        public InvalidDatabaseStructureException(string tabela) :
            base("Błędna struktura tabeli '" + tabela + "'")
        {
            this.Table = tabela;
        }
    }
}
