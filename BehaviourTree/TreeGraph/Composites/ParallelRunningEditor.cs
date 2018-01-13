using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class ParallelRunningEditor
                : ParallelEditor
            {
                
                private List<bool> childrenOver;

                public ParallelRunningEditor (string title, Vector2 pos) : base (title, pos)
                {
                }

                public ParallelRunningEditor (string title, Rect window, int reqFail, int reqSuccess) : base (title, window, reqFail, reqSuccess)
                {
                    ParallelInit (reqFail, reqSuccess);
                }

                public ParallelRunningEditor (string title, Rect window, string name, int reqFail, int reqSuccess) : base (title, window, name, reqFail, reqSuccess)
                {
                    ParallelInit (reqFail, reqSuccess);
                }


                private void ParallelInit (int reqFail, int reqSuccess)
                {
                    this.reqFail = reqFail;
                    this.reqSuccess = reqSuccess;
                }

                public override void DrawWindowDebug ()
                {

                }

                public override ENodeState OnBehaviour (Context data)
                {

                    if (IsAllChildrenExecuted ())
                    {
                        OnReset ();
                        return HandleDebug (ENodeState.RUNNING);
                    }

                    if (!childrenOver[currentChild])
                    {
                        ENodeState current = GetCurrentChildState (data);
                        HandleDebug (current);

                        if (current != ENodeState.RUNNING)
                            childrenOver[currentChild] = true;

                        if (current == ENodeState.SUCCESS)
                            successed++;
                        else if (current == ENodeState.FAILURE)
                            failed++;

                        if (successed >= reqSuccess)
                        {
                            return ENodeState.SUCCESS;
                        }
                        else if (failed >= reqFail)
                        {
                            return ENodeState.FAILURE;
                        }

                        NextChild ();
                        return OnBehaviour (data);
                    }

                    NextChild ();
                    return ENodeState.RUNNING;
                }

                public override void OnReset ()
                {

                    if (childrenOver == null 
                        || childrenOver.Count (x => x)
                        == childrenOver.Count)
                    {
                        failed = successed = 0;
                        SequenceNodes ();
                        ResetAllChildren ();
                        InitChildrenOver ();
                    }
                    else
                    {
                        currentChild = 0;
                    }
                }

                public override void OnInit (Context data)
                {
                    base.OnInit (data);
                    InitChildrenOver ();
                }

                protected override void Init ()
                {
                    failed = successed = 0;
                    SequenceNodes ();
                }


                private void InitChildrenOver ()
                {
                    childrenOver = new List<bool> ();
                    for (int i = 0; i < children.Count; i++)
                    {
                        childrenOver.Add (false);
                    }
                }

            }
        }
    }
}
