
using System.Collections;
using System.Collections.Generic;
namespace Ai
{
    namespace BehaviourTree
    {
        public class SequenceRandom : Sequence
        {
            public SequenceRandom(string name, params Node[] nodes) : base(name, nodes)
            {
                OnReset();
            }

            public override void OnReset()
            {
                MixNodes();
                ResetAllChildren();
            }
        }
    }
}
