using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Singularity.ORM.Serialization._3rdParty.NewtonsoftJSON
{
    /// <summary>
    /// 
    /// </summary>
    public class SingularityJsonTextWriter : JsonTextWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public SingularityJsonTextWriter(TextWriter textWriter) : base(textWriter) { }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentDepth { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }
}
