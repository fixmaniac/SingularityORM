using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Exceptions
{
    public class EntityValidationException : FormatException
    {
        public EntityValidationException(string message)
            : base(message)
        {

        }
    }
}
