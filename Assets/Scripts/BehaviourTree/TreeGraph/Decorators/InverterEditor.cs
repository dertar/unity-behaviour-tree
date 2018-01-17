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
            public class InverterEditor 
                : DecoratorEditor
            {
                public InverterEditor (string title, Vector2 pos) : base (title, pos)
                {
                }

                public InverterEditor (string title, Rect rect) : base (title, rect)
                {
                }

                public InverterEditor (string title, Rect windows, Sockets sockets) : base (title, windows, sockets)
                {
                }

                public override ENodeState OnBehaviour (Context data)
                {
                    return HandleDebug (Inverter.Invert (child.OnBehaviour (data)));
                }
            }
        }
    }
}
