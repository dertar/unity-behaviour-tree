using System;

namespace Ai
{
    namespace BehaviourTree
    {

        public class RepeatUntilFail : Decorator
        {
            public RepeatUntilFail(Node node) : base(node)
            {
            }

            public override ENodeState OnBehaviour (Context data)
            {
                ENodeState result = child.Behaviour (data);

                if (result != ENodeState.RUNNING)
                {
                    //child.Reset();
                    ResetHandle ();
                }
                return result == ENodeState.FAILURE ? ENodeState.FAILURE : ENodeState.RUNNING;
            }

            public override void OnInit (Context data)
            {
                
            }
        }
    }
}
