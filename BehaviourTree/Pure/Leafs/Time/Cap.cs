namespace Ai
{
    namespace BehaviourTree
    {
        namespace Leafs
        {
            public class Cap 
                : Leaf
            {
                public override ENodeState OnBehaviour(Context data)
                {
                    return ENodeState.RUNNING;
                }

                public override void OnInit (Context data)
                {
                    
                }

                public override void OnReset()
                {

                }
            }
        }
    }
}
