using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class LeafEditor 
                : NodeEditor
            {
                protected Leaf leaf;

                public LeafEditor(string title, Vector2 pos,Leaf leaf) : base(title, pos)
                {
                    this.leaf = leaf;
                }

                public LeafEditor(string title, Rect window, Leaf leaf) : base(title, window)
                {
                    this.leaf = leaf;
                }

                //public LeafEditor(string title, Rect window, Leaf leaf, Dictionary<string, Triple<object, Type, EPermissionLeaf>> keys) : base(title, window)
                //{
                //    this.leaf = leaf;

                //    if(keys != null)
                //        leaf.SetKeys(keys);
                //}


                //public override void DrawWindowDebug()
                //{
                //    var keys = leaf.GetKeys();

                //    foreach(var key in keys)
                //    {
                //        GUILayout.Label(key.Key + ":" + key.Value);
                //    }

                //    GUILayout.Label("returned " + lastReturn);
                //    GUILayout.Label("ticks " + ticks);
                //}


                public Dictionary<string, LeafDataStatic> GetStaticKeys ()
                {
                    return leaf.GetStatic();
                }

                public Dictionary<string, LeafDataDynamic> GetDynamicKeys ()
                {
                    return leaf.GetDynamic ();
                }


                public Node GetNode()
                {
                    return leaf;
                }

                public override void OnInit (Context data)
                {
                    leaf.OnInit(data);
                }

                public List<object> GetArgs ()
                {
                    return leaf.GetArgs ();
                }

                public override ENodeState OnBehaviour(Context data)
                {
                    Monitor();
                    return HandleDebug(leaf.OnBehaviour(data));
                }

                public override void OnReset()
                {
                    leaf.OnReset();
                }

                protected override void Init()
                {
                    
                }

                public override void UpdateNodesFromSockets()
                {

                }
            }

        }

    }
}
