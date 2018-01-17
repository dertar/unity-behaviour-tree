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
            public class Bigger
                : Leaf
            {
                public Bigger ()
                {
                    Init ("", 0f);
                }

                public Bigger (string keyNum, float num)
                {
                    Init (keyNum, num);
                }


                private void Init (string keyNum, float num)
                {
                    RegisterDynamicValue ("Key num", keyNum, typeof (float), EPermissionLeaf.READ);
                    RegisterStaticValue ("num", "num", num);
                }

                public override ENodeState OnBehaviour (Context data)
                {
                    float bigger = Read<float> (data, "Key num");
                    float num = Read<float> (data, "num");

                    return bigger > num
                        ? ENodeState.SUCCESS
                        : ENodeState.FAILURE;
                }

                public override void OnReset ()
                {

                }
            }

        }
    }
}
