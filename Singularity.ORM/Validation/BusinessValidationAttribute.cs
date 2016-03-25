using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BusinessValidationAttribute : ValidationAttribute
    {
        public static bool Get(MemberInfo mi)
        {
            return IsDefined(mi, typeof(MendatoryAttribute));
        }
    }
}
