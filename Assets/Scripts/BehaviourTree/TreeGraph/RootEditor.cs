using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class RootEditor 
                : DecoratorEditor
            {
                private Context context;

                public RootEditor(string title, Vector2 pos) : base(title, pos)
                {
                    
                }

                public RootEditor(string title, Rect window) : base(title, window)
                {

                }

                protected override void Init()
                {
                    base.Init();
                    sockets.RemoveSocket(sockets.GetSocket(SocketIO.INPUT, 0), window);
                }

                public Context GetContext()
                {
                    return context;
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    Monitor();

                    return HandleDebug(child.OnBehaviour(data));
                }

                public override void OnInit (Context data)
                {
                    base.OnInit (data);
                    context = data;
                }
            }
        }
    }
}
