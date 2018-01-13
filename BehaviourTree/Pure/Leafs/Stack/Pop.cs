using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace Leafs
        {
            public abstract class Pop<T>
              : Leaf
            {

                public Pop ()
                {
                    Init ("", "");
                }

                public Pop (string keyStack, string keyPop)
                {
                    Init (keyStack, keyPop);
                }

                private void Init (string keyStack, string keyPop)
                {
                    RegisterDynamicValue ("Stack", keyStack, typeof (List<T>), EPermissionLeaf.READ);
                    RegisterDynamicValue ("Val", keyPop, typeof (T), EPermissionLeaf.WRITE);
                }

                public override ENodeState OnBehaviour (Context data)
                {
                    var stack = Read<List<T>> (data, "Stack");

                    if (stack.Count == 0)
                        return ENodeState.FAILURE;

                    var val = stack[0];
                    stack.RemoveAt (0);

                    Write<T> (data, "Val", val);

                    return ENodeState.SUCCESS;
                }

                public override void OnReset ()
                {

                }
            }



        }
    }
}
