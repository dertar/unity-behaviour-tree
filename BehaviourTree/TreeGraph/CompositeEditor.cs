using Ai.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public abstract class CompositeEditor 
                : NodeEditor
                , IComposite
            {
                protected List<NodeEditor> children = new List<NodeEditor>();
                protected int currentChild;

                protected int[] sequenceIndexes;
                protected string compositeName = string.Empty;

                public CompositeEditor(string title, Vector2 pos) : base(title, pos)
                {
                }

                public CompositeEditor(string title, Rect window) : base(title, window)
                {
                }

                public CompositeEditor(string title, Rect window,string name) : base(title, window)
                {
                    compositeName = name;
                }

                public CompositeEditor(string title, Rect windows, Sockets sockets) : base(title, windows, sockets)
                {
                }

                public override void OnInit (Context data)
                {
                    GetCurrentChild().OnInit (data);
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

                public virtual void ResetAllChildren ()
                {
                    currentChild = 0;
                    foreach (NodeEditor child in children)
                    {
                        child.ResetHandle();
                    }
                }

                public ENodeState GetCurrentChildState(Context data)
                {
                    ENodeState ret = (GetCurrentChild() as NodeEditor).Behaviour(data);

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
                    if (children.Count == 0) return;

                    sequenceIndexes = new int[children.Count];

                    List<int> sequence = GenSequence(sequenceIndexes.Length);

                    for (int i = 0; i < sequenceIndexes.Length; i++)
                    {
                        int rnd = sequence[UnityEngine.Random.Range(0, sequence.Count - 1)];
                        sequenceIndexes[i] = rnd;
                        sequence.Remove(rnd);
                    }
                }

                public void SequenceNodes()
                {
                    sequenceIndexes = GenSequence(children.Count).ToArray();
                }

                public override void UpdateNodesFromSockets()
                {
                    children = GetSockets().GetSockets(SocketIO.OUTPUT).ConvertAll(x => x.connectedTo);
                    SequenceNodes();
                }

                public string GetName()
                {
                    return compositeName;
                }

                public void SetName(string name)
                {
                    this.compositeName = name;
                }

                public override void DrawWindow(GUISkin skin)
                {
                    GUILayout.Space(8f);

                    GUILayout.Label(compositeName == string.Empty ? title : compositeName, skin.label);
                }
            }
        }

    }
}
