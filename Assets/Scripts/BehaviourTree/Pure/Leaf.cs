using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        public enum EPermissionLeaf
        {
            READ,
            WRITE,
            READ_WRITE
        }

        public class LeafData
        {
            public string keyToContext;

            public LeafData (string keyToContext)
            {
                this.keyToContext = keyToContext;
            }
        }

        public class LeafDataStatic
            : LeafData
        {
            public object staticValue;

            public LeafDataStatic (string keyToContext, object val) 
                : base (keyToContext)
            {
                staticValue = val;
            }
        }

        public class LeafDataDynamic
            : LeafData
        {
            public EPermissionLeaf permission;
            public Type type;

            public LeafDataDynamic (string keyToContext, Type type, EPermissionLeaf permission) 
                : base (keyToContext)
            {
                this.type = type;
                this.permission = permission;
            }
        }

        public abstract class Leaf 
            : Node
        {
            private Dictionary<string, LeafDataStatic> staticData;
            private Dictionary<string, LeafDataDynamic> dynamicData;

            private List<string> args;

            public Leaf()
            {
                staticData = new Dictionary<string, LeafDataStatic> ();
                dynamicData = new Dictionary<string, LeafDataDynamic> ();
                args = new List<string> ();
            }


            public void RegisterDynamicValue (string keyLeaf, string keyContext, Type type, EPermissionLeaf permission)
            {
                dynamicData.Add (keyLeaf, new LeafDataDynamic(keyContext, type, permission));
                args.Add (keyLeaf);
            }

            public void RegisterStaticValue (string keyLeaf, string keyContext, object defaultValue)
            {
                staticData.Add (keyLeaf, new LeafDataStatic (keyContext, defaultValue));
                args.Add (keyLeaf);
            }

            public List<object> GetArgs ()
            {
                var ret = new List<object> ();

                foreach (var x in args)
                {
                    string key = GetKeyToContext (x);
                    if (IsOnDynamic (x))
                    {
                        ret.Add (dynamicData[x].keyToContext);
                    }
                    else if (IsOnStatic (x))
                    {
                        ret.Add (staticData[x].staticValue);
                    }
                    else
                    {
                        throw new Exception ("Error parsing args!");
                    }
                    
                }

                return ret;
            }

            public Dictionary<string, LeafDataStatic> GetStatic ()
            {
                return staticData;
            }

            public Dictionary<string, LeafDataDynamic> GetDynamic ()
            {
                return dynamicData;
            }


            private string GetKeyToContext(string keyLeaf)
            {
                if (IsOnDynamic (keyLeaf))
                {
                    return GetKeyToContextFromDynamic (keyLeaf);
                }
                else if (IsOnStatic (keyLeaf))
                {
                    return GetKeyToContextFromStatic (keyLeaf);
                }

                throw new KeyNotFoundException ();
            }

            private string GetKeyToContextFromDynamic (string keyLeaf)
            {
                return dynamicData[keyLeaf].keyToContext;
            }

            private string GetKeyToContextFromStatic (string keyLeaf)
            {
                return staticData[keyLeaf].keyToContext;
            }

            private bool IsOnStatic (string keyLeaf)
            {
                return staticData.ContainsKey (keyLeaf);
            }

            private bool IsOnDynamic (string keyLeaf)
            {
                return dynamicData.ContainsKey (keyLeaf);
            }

            public void Write<T> (Context context, string keyLeaf, T val)
            {
                var data = GetDynamicData (keyLeaf);
                
                if (IsWritable (data.permission))
                {
                    if (data.type.Equals (typeof (T)))
                    {
                        context.Add<T> (GetKeyToContextFromDynamic (keyLeaf), val);
                    }
                    else
                    {
                        throw new InvalidCastException (this + "; key: " + keyLeaf + "; expected: " + data.type.ToString () + "; actual: " + typeof (T).ToString ());
                    }
                }
                else
                {
                    throw new Exception ("Error, data isn't writable");
                }

            }

            private LeafDataDynamic GetDynamicData (string keyLeaf)
            {
                return dynamicData[keyLeaf];
            }

            public T Read<T> (Context context, string keyLeaf)
            {
                if (IsOnDynamic (keyLeaf))
                {
                    return ReadFromDynamic<T> (context, keyLeaf);
                }
                else if (IsOnStatic (keyLeaf))
                {
                    return ReadFromStatic<T> (context, keyLeaf);
                }
                throw new KeyNotFoundException ();
            }

            private T ReadFromStatic<T> (Context context, string keyLeaf)
            {
                string keyToContext = GetKeyToContext (keyLeaf);
                return context.Get<T> (keyToContext);
            }

            private T ReadFromDynamic<T> (Context context, string keyLeaf)
            {
                var data = GetDynamicData (keyLeaf);
                if (IsReadable (data.permission))
                {
                    if (data.type.Equals (typeof (T)))
                    {
                        return context.Get<T> (data.keyToContext);
                    }
                    throw new InvalidCastException ();
                }
                throw new Exception ("Error, data isn't readable");
            }

            private LeafDataStatic GetStaticData (string keyLeaf)
            {
                return staticData[keyLeaf];
            }

            public override void OnInit (Context data)
            {
                foreach (var x in staticData)
                {
                    data.Add (x.Value.keyToContext, x.Value.staticValue);
                }
            }

            private bool IsReadable (EPermissionLeaf permission)
            {
                return permission.Equals (EPermissionLeaf.READ)
                    || permission.Equals (EPermissionLeaf.READ_WRITE);
            }

            private bool IsWritable (EPermissionLeaf permission)
            {
                return permission.Equals (EPermissionLeaf.WRITE)
                    || permission.Equals (EPermissionLeaf.READ_WRITE);
            }

        }
    }
}
