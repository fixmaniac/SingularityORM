using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Singularity.ORM.Serialization._3rdParty.NewtonsoftJSON;


namespace Singularity.ORM.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public static class Serializer
    {
        private static readonly int opctimalDepth = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NewtonsoftSerializeObject(this object obj)
        {
            return NewtonsoftSerializeObject(obj, opctimalDepth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static string NewtonsoftSerializeObject(this object obj, int maxDepth)
        {
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new SingularityJsonTextWriter(strWriter))
                {
                    Func<bool> include = () => jsonWriter.CurrentDepth <= maxDepth;
                    var resolver = new SingularityContractResolver(include);
                    var serializer = new JsonSerializer { ContractResolver = resolver };
                    serializer.Serialize(jsonWriter, obj);
                }
                return strWriter.ToString();
            }
        }
    }
}
