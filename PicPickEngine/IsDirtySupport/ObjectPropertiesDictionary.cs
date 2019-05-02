using System;
using System.Collections.Generic;

namespace PicPick.Project.IsDirtySupport
{
    public class ObjectPropertiesDictionary<T, L> : Dictionary<T, L> where T : Type where L : List<string>, new()
    {
        public void Add(T obj, string prp)
        {
            if (!ContainsKey(obj))
                Add(obj, new L());
            if (!this[obj].Contains(prp))
                this[obj].Add(prp);
        }

        public bool Contains(T t, string prp)
        {
            return (ContainsKey(t) && this[t].Contains(prp));
        }


    }
}
