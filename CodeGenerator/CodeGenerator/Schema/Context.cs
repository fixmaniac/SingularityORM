using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeGenerator.Schema
{
    public class Context
    {
        public List<Entity> Entities = new List<Entity>();
        public Hashtable ByName = new Hashtable();

        public void AddEntity(Entity entity)
        {
            this.Entities.Add(entity);
        }

        public void Load()
        {
            indexByName();
        }

        private void indexByName()
        {
            foreach (Entity entity in this.Entities)
            {
                foreach (WithName v in entity.Fields)
                    this.addByName(v);
                foreach (WithName v in entity.Enums)
                    this.addByName(v);
            }
        }

        private void addByName(WithName v)
        {
            if (this.ByName.ContainsKey((object)v.Name))
                throw new InvalidOperationException("Field or property '" + v.Name + "' is declared more than once.");
            this.ByName[(object)v.Name] = (object)v;
        }        
    }
}
