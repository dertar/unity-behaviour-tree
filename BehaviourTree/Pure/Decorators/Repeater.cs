using System;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public class Repeater : Decorator
        {
            public Repeater(Node node) : base(node)
            {
            }

            public override ENodeState OnBehaviour (Context data)
            {
                ENodeState result = child.Behaviour (data);

                if (result != ENodeState.RUNNING)
                {
                    ResetHandle ();
                }
                return result == ENodeState.RUNNING ? ENodeState.RUNNING : ENodeState.SUCCESS;
            }

            public override void OnInit (Context data)
            {
                
            }
        }
    }
}