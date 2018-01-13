using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class NodeMonitor 
            {
                private NodeEditor currentNode;
                private List<NodeEditor> sequences = new List<NodeEditor>();

                public NodeMonitor()
                {

                }

                public void Record(NodeEditor node)
                {
                    if(typeof(RootEditor).Equals(node.GetType()))
                    {
                        sequences.Clear();
                    }
                    sequences.Add(node);

                    this.currentNode = node;
                }

                public bool IsNode(NodeEditor node)
                {
                    return currentNode.Equals(node);
                }

                public bool InSequence(NodeEditor node)
                {
                    return sequences.Contains(node);
                }
            }

        }
    }
}
