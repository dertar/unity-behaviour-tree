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
            public abstract class DecoratorEditor 
                : NodeEditor
                , IDecorator
            {
                protected NodeEditor child;

                public DecoratorEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public DecoratorEditor(string title, Rect rect) : base(title, rect)
                {
                }


                public DecoratorEditor(string title, Rect windows, Sockets sockets) : base(title, windows, sockets)
                {
                }

                protected override void Init()
                {
                    sockets.AddSocket(SocketIO.OUTPUT, window);
                }

                public override void UpdateNodesFromSockets()
                {
                    child = GetSockets().GetSocket(SocketIO.OUTPUT, 0).connectedTo;
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    return HandleDebug(child.Behaviour(data));
                }

                public override void OnReset()
                {
                    child.OnReset();
                }

                public INode GetChild()
                {
                    return child;
                }

                public override void OnInit (Context data)
                {
                    child.OnInit (data);
                }
            }
        }

    }
}
