using System;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public class Selector : Composite
        {
            public Selector(string name, params Node[] nodes) : base(name, nodes)
            {
            }
                
            public override ENodeState OnBehaviour(Context data)
            {
                if (IsAllChildrenExecuted ())
                {
                    return ENodeState.FAILURE;
                }

                ENodeState current = GetCurrentChildState (data);

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

            public override void OnInit (Context data)
            {
                SequenceNodes ();
            }

            public override void OnReset ()
            {
                SequenceNodes();
                ResetAllChildren();
            }
        }
    }
}
