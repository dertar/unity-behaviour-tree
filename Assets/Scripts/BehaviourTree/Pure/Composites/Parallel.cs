using System;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public class Parallel : Composite
        {
            private int reqFail;
            private int reqSuccess;

            private int failed = 0;
            private int successed = 0;

            public Parallel(string name, int reqFail, int reqSuccess, params Node[] nodes)
                : base(name, nodes)
            {
                this.reqFail = reqFail;
                this.reqSuccess = reqSuccess; 
            }


            /*public override ENodeState OnBehaviour(Context data, bool handled)
            {
                int failed = 0,
                    successed = 0;

                foreach (var child in children)
                {
                    ENodeState status = child.Behaviour(data);
                    if (status == ENodeState.FAILURE)
                        failed++;
                    else if (status == ENodeState.SUCCESS)
                        successed++;
                }

                if (successed >= reqSuccess)
                    return ENodeState.SUCCESS;
                else if (failed >= reqFail)
                    return ENodeState.FAILURE;

                return ENodeState.RUNNING;
            }*/
            public override ENodeState OnBehaviour(Context data)
            {
                if (IsAllChildrenExecuted())
                {
                    return ENodeState.FAILURE;
                }

                ENodeState current = GetCurrentChildState(data);

                if (current == ENodeState.SUCCESS)
                    successed++;
                else if (current == ENodeState.FAILURE)
                    failed++;

                if (successed >= reqSuccess)
                    return ENodeState.SUCCESS;
                else if (failed >= reqFail)
                    return ENodeState.FAILURE;

                NextChild();
                return OnBehaviour(data);
            }

            public override void OnInit (Context data)
            {
                SequenceNodes ();
            }

            public override void OnReset()
            {
                failed = successed = 0;
                SequenceNodes();
                ResetAllChildren();
            }
        }
    }
}
