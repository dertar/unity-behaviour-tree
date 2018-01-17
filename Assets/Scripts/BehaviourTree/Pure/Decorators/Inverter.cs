using System;

namespace Ai
{
    namespace BehaviourTree
    {
        public class Inverter : Decorator
        {
            public Inverter(Node node) : base(node)
            {
            }

            public override ENodeState OnBehaviour(Context data)
            {
                return Invert(child.Behaviour(data));
            }

            public static ENodeState Invert (ENodeState state)
            {
                if (state == ENodeState.FAILURE)
                {
                    return ENodeState.SUCCESS;
                }
                else if (state == ENodeState.SUCCESS)
                {
                    return ENodeState.FAILURE;
                }

                return ENodeState.RUNNING;
            }

            public override void OnInit (Context data)
            {
                
            }
        }
    }
}
