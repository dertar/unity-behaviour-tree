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
            public class Log
                : Leaf
            {
                public Log(string msg)
                {
                    Init(msg);
                }

                public Log()
                {
                    Init("");
                }

                public void Init(string msg)
                {
                    RegisterStaticValue ("Message", "msg", msg);
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    Debug.Log(Read<string> (data, "Message"));

                    return ENodeState.SUCCESS;
                }

                public override void OnReset()
                {

                }

                public override void OnInit (Context data)
                {
                    base.OnInit (data);
                }
            }
        }
    }
}

