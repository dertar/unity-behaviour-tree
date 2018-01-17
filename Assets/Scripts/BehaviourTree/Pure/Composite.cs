using System.Collections.Generic;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        public abstract class Composite 
            : Node, IComposite
        {
            protected List<Node> children = new List<Node>();
            protected int currentChild;

            protected int[] sequenceIndexes;
            protected string name;

            public Composite(string name, params Node[] nodes)
            {
                this.name = name;
                children.AddRange(nodes);
            }

            public virtual void NextChild()
            {
                currentChild++;
            }

            public virtual bool IsAllChildrenExecuted()
            {
                return currentChild >= children.Count;
            }

            public virtual INode GetCurrentChild()
            {
                return children[sequenceIndexes[currentChild]];
            }

            public void ResetAllChildren()
            {
                currentChild = 0;
                foreach (Node child in children)
                {
                    child.ResetHandle();
                }
            }

            public ENodeState GetCurrentChildState(Context data)
            {
                ENodeState ret = (GetCurrentChild() as Node).Behaviour(data);

                return ret;
            }

            public List<int> GenSequence(int to)
            {
                List<int> sequence = new List<int>();

                for (int i = 0; i < to; i++)
                {
                    sequence.Add(i);
                }

                return sequence;
            }

            public void MixNodes()
            {

                sequenceIndexes = new int[children.Count];

                List<int> sequence = GenSequence(sequenceIndexes.Length);

                for (int i = 0; i < sequenceIndexes.Length; i++)
                {
                    int rnd = sequence[Random.Range(0, sequence.Count - 1)];
                    sequenceIndexes[i] = rnd;
                    sequence.Remove(rnd);
                }
            }

            public void SequenceNodes()
            {
                sequenceIndexes = GenSequence(children.Count).ToArray();
            }
        }
    }
}
