using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class RepeaterUntilFailEditor 
                : DecoratorEditor
            {
                public RepeaterUntilFailEditor (string title, Vector2 pos) : base (title, pos)
                {
                }

                public RepeaterUntilFailEditor (string title, Rect rect) : base (title, rect)
                {
                }

                public RepeaterUntilFailEditor (string title, Rect windows, Sockets sockets) : base (title, windows, sockets)
                {
                }


                public override ENodeState OnBehaviour (Context data)
                {
                    ENodeState result = child.Behaviour (data);

                    if (result != ENodeState.RUNNING)
                    {
                        ResetHandle ();
                    }

                    return HandleDebug (result == ENodeState.FAILURE ? ENodeState.FAILURE : ENodeState.RUNNING);
                }
            }

        }
    }
}
