using System;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public class Sequence : Composite
        {
            public Sequence(string name, params Node[] nodes) : base(name, nodes)
            {
                
            }

            public override ENodeState OnBehaviour (Context data)
            {
                if (IsAllChildrenExecuted ())
                {
                    return ENodeState.SUCCESS;
                }

                ENodeState current = GetCurrentChildState (data);

                if (current == ENodeState.SUCCESS)
                {
                    NextChild ();
                    return OnBehaviour (data);
                }
                else if (current == ENodeState.FAILURE)
                {
                    return ENodeState.FAILURE;
                }

                return ENodeState.RUNNING;
            }

            public override void OnInit (Context data)
            {
                SequenceNodes ();
            }

            public override void OnReset()
            {
                SequenceNodes();
                ResetAllChildren();
            }
        }
    }
}
