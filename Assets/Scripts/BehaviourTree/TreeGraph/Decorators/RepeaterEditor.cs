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
            public class RepeaterEditor 
                : DecoratorEditor
            {
                public RepeaterEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public RepeaterEditor(string title, Rect rect) : base(title, rect)
                {
                }

                public RepeaterEditor(string title, Rect windows, Sockets sockets) : base(title, windows, sockets)
                {
                }
            }
        }

    }
}
