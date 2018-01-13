using Ai.Combat;
using Character;
using System;
using System.Collections.Generic;

namespace Ai
{
    namespace BehaviourTree
    {
        public class Context
        {

            private Dictionary<string, object> data;

            public Context(Mob mob)
            {
                data = new Dictionary<string, object> ();
            }

            public T Get<T>(string key)
            {
                if (Contains(key))
                {
                    return (T)data[key];
                }

                throw new KeyNotFoundException("Key not found");
            }

            public void Add<T>(string key, T obj)
            {
                if (data.ContainsKey(key))
                {
                    ChangeWithOutVerify(key, obj);
                } else
                {
                    data.Add(key, obj);
                }
            }

            public void Change<T>(string key, T obj)
            {
                if (data.ContainsKey(key))
                {
                    ChangeWithOutVerify(key, obj);
                }
            }

            private void ChangeWithOutVerify<T>(string key, T obj)
            {
                data[key] = obj;
            }


            public bool Remove(string key)
            {
                return data.Remove(key);
            }


            public bool Contains(string key)
            {
                return data.ContainsKey(key);
            }

            public void Clear()
            {
                data.Clear();
            }

            public Dictionary<string, object> Get()
            {
                return data;
            }

        }
    }
}
