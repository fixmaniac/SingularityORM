using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Singularity.ORM.Serialization._3rdParty.NewtonsoftJSON
{
    /// <summary>
    /// 
    /// </summary>
    public class SingularityContractResolver : DefaultContractResolver
    {
        private readonly Func<bool> _includeProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeProperty"></param>
        public SingularityContractResolver(Func<bool> includeProperty)
        {
            _includeProperty = includeProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(
            MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => _includeProperty() &&
                                              (shouldSerialize == null ||
                                               shouldSerialize(obj));
            return property;
        }
    }
}
