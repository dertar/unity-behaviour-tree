namespace Ai
{
    namespace BehaviourTree
    {
        public abstract class Node
            : INode
        {
            private bool isFirstRun = true;

            public virtual ENodeState Behaviour (Context data)
            {
                if (isFirstRun)
                {
                    OnInit (data);
                    isFirstRun = false;
                }

                ENodeState state = OnBehaviour (data);

                if (state != ENodeState.RUNNING)
                    ResetHandle ();

                return state;
            }

            public void ResetHandle ()
            {
                isFirstRun = true;
                OnReset();
            }

            public abstract void OnInit (Context data);
            public abstract void OnReset ();
            public abstract ENodeState OnBehaviour (Context data);
        }
    }
}
