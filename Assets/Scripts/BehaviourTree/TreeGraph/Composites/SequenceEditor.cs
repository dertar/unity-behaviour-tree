using Ai.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class SequenceEditor 
                : CompositeEditor
            {
                public SequenceEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public SequenceEditor(string title, Rect window) : base(title, window)
                {
                }

                public SequenceEditor(string title, Rect windows, Sockets sockets) : base(title, windows, sockets)
                {
                }

                public SequenceEditor(string title, Rect window, string name) : base(title, window, name)
                {
                }

                protected override void Init()
                {
                    
                }
                
                public override ENodeState OnBehaviour(Context data)
                {
                    if (IsAllChildrenExecuted())
                    {
                        return ENodeState.SUCCESS;
                    }

                    ENodeState current = GetCurrentChildState(data);
                    HandleDebug(current);
                    Monitor();

                    if (current == ENodeState.SUCCESS)
                    {
                        NextChild();
                        return OnBehaviour(data);
                    }
                    else if (current == ENodeState.FAILURE)
                    {
                        return ENodeState.FAILURE;
                    }

                    return ENodeState.RUNNING;
                }

                public override void OnReset()
                {
                    SequenceNodes();
                    ResetAllChildren();
                }
            }
        }
    }
}
