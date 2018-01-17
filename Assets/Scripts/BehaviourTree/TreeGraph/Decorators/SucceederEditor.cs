using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class SucceederEditor 
                : DecoratorEditor
            {
                public SucceederEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public SucceederEditor(string title, Rect rect) : base(title, rect)
                {
                }

                public SucceederEditor(string title, Rect windows, Sockets sockets) : base(title, windows, sockets)
                {
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    return HandleDebug(child.Behaviour(data) == ENodeState.RUNNING ? ENodeState.RUNNING : ENodeState.SUCCESS);
                }
            }
        }

    }
}
