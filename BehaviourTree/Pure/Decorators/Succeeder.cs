using System;

namespace Ai
{
    namespace BehaviourTree
    {

        public class Succeeder 
            : Decorator
        {
            public Succeeder (Node node) : base (node)
            {
            }

            public override ENodeState OnBehaviour (Context data)
            {
                return child.Behaviour (data) == ENodeState.RUNNING 
                    ? ENodeState.RUNNING : ENodeState.SUCCESS;
            }

            public override void OnInit (Context data)
            {

            }
        }
    }
}
