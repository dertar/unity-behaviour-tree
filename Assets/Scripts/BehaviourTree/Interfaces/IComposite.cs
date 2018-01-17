using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public interface IComposite
        {
            void NextChild();
            bool IsAllChildrenExecuted();

            INode GetCurrentChild();

            void ResetAllChildren();
            ENodeState GetCurrentChildState(Context data);
            List<int> GenSequence(int to);
            void MixNodes();
            void SequenceNodes();
        }
    }
}
