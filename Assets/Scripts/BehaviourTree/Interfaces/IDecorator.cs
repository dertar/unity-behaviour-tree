using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public interface IDecorator
        {
            INode GetChild();
        }
    }
}

