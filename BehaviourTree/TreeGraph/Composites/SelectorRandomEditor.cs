using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class SelectorRandomEditor
                : SelectorEditor
            {
                public SelectorRandomEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public SelectorRandomEditor(string title, Rect window) : base(title, window)
                {
                }

                public SelectorRandomEditor(string title, Rect window, string name) : base(title, window, name)
                {
                }

                public override void OnReset()
                {
                    MixNodes();
                    ResetAllChildren();
                }

                protected override void Init()
                {
                    base.Init();
                    MixNodes();
                }
            }
        }
    }
}

