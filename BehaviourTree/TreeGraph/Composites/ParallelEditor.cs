using System;
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
            public class ParallelEditor
                : CompositeEditor
            {
                protected int reqFail;
                protected int reqSuccess;

                protected int failed = 0;
                protected int successed = 0;
               

                public ParallelEditor (string title, Vector2 pos) : base (title, pos)
                {
                }

                public ParallelEditor (string title, Rect window, int reqFail, int reqSuccess) : base (title, window)
                {
                    ParallelInit (reqFail, reqSuccess);
                }

                public ParallelEditor (string title, Rect window, string name, int reqFail, int reqSuccess) : base (title, window)
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

                    ENodeState current = GetCurrentChildState (data);
                    HandleDebug (current);
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


                public override void OnReset ()
                {

                    failed = successed = 0;
                    SequenceNodes ();
                    ResetAllChildren ();

                }
                protected override void Init ()
                {
                    failed = successed = 0;
                    SequenceNodes ();
                }

                public int GetReqFail ()
                {
                    return reqFail;
                }

                public int GetReqSuccess ()
                {
                    return reqSuccess;
                }

                public void SetReqFail (int v)
                {
                    reqFail = v;
                }

                public void SetReqSuccess (int v)
                {
                    reqSuccess = v;
                }
            }
        }
    }
}
