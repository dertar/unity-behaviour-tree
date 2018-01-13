using System;

namespace Ai
{
    namespace BehaviourTree
    {
        public class Failurer : Decorator
        {
            public Failurer(Node node) : base(node)
            {
            }

            public override ENodeState OnBehaviour(Context data)
            {
                return child.Behaviour(data) == ENodeState.RUNNING ? ENodeState.RUNNING : ENodeState.FAILURE;
            }

            public override void OnInit (Context data)
            {
                
            }
        }
    }
}
