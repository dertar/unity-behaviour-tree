using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class SequenceRandomEditor
                : SequenceEditor
            {
                public SequenceRandomEditor(string title, Rect window) : base(title, window)
                {
                }

                public SequenceRandomEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public SequenceRandomEditor(string title, Rect window, string name) : base(title, window, name)
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
