using System;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class SelectorEditor
                : CompositeEditor
            {
                public SelectorEditor (string title, Vector2 pos) : base (title, pos)
                {
                }

                public SelectorEditor (string title, Rect window) : base (title, window)
                {
                }

                public SelectorEditor (string title, Rect window, string name) : base (title, window, name)
                {
                }

                public override ENodeState OnBehaviour (Context data)
                {
                    if (IsAllChildrenExecuted ())
                    {
                        return ENodeState.FAILURE;
                    }

                    ENodeState current = GetCurrentChildState (data);
                    HandleDebug (current);

                    if (current == ENodeState.SUCCESS)
                    {
                        return ENodeState.SUCCESS;
                    }
                    else if (current == ENodeState.FAILURE)
                    {
                        NextChild ();
                        return OnBehaviour (data);
                    }

                    return ENodeState.RUNNING;
                }

                public override void OnReset ()
                {
                    SequenceNodes ();
                    ResetAllChildren ();
                }

                protected override void Init ()
                {
                    
                }
            }

        }
    }
}
