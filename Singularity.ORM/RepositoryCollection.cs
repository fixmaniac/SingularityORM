using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Singularity.ORM.SQL;

namespace Singularity.ORM
{
   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="TRep"></typeparam>
    public class RepositoryCollection<TRep> : ICollection<TRep> where TRep : RepositoryItem
    {
        private Dictionary<string, TRep> collection = new Dictionary<string, TRep>();

        /// <summary>
        /// 
        /// </summary>
        public RepositoryCollection() {

            this.collection = new Dictionary<string, TRep>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(TRep item)
        {         
                if (!this.Any(i => i.Name == item.Name))
                    this.collection.Add(item.Name, item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TRep item)
        {
            return collection.ContainsValue(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TRep[] array, int arrayIndex)
        {
            collection.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return collection.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TRep item)
        {
            return collection.Remove(item.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TRep> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }        
    }  
}
