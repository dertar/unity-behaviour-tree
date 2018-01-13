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
            public class Failed : Leaf
            {
                public int resets = 0;
                public int inits = 0;
                public int behaviours = 0;

                public override void OnInit(Context data)
                {
                    inits++;
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    data.Add<int>("resets", resets);
                    data.Add<int>("inits", inits);
                    data.Add<int>("behavoiurs", behaviours++);

                    return ENodeState.FAILURE;
                }

                public override void OnReset()
                {
                    resets++;
                }
            }
        }
    }
}
