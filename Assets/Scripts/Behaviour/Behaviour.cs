using Ai.BehaviourTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Ai.BehaviourTree.TreeGraph;

namespace Ai
{
    [CustomEditor(typeof(Behaviour))]
    public class BehaviourEditor 
        : Editor
    {
        private int selectedBehaviour = -1;

        private string[] behaviours;

        private SerializedProperty behaviourName;

        public void OnEnable()
        {
            behaviourName = serializedObject.FindProperty("behaviourName");
            UpdateListBehaviours();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var obj = target as Behaviour;

            var name = behaviourName.stringValue;

            selectedBehaviour = Array.FindIndex<string>(behaviours, x => x == name);

            selectedBehaviour = EditorGUILayout.Popup(selectedBehaviour, behaviours);

            if (selectedBehaviour >= 0 && selectedBehaviour < behaviours.Length
                && name != behaviours[selectedBehaviour])
            {
                behaviourName.stringValue = behaviours[selectedBehaviour];
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateListBehaviours()
        {
            List<string> pathes = new List<string>(Directory.GetFiles(Application.dataPath + BehaviourTree.TreeGraph.TreeEditor.CATALOG, "*.AiBT"));
            behaviours = pathes.ConvertAll<string>((x) =>
            {
                var lems = x.Split('/');

                return lems[lems.Length - 1];
            }).ToArray();
        }
    }
    [System.Serializable]
    public class Behaviour : MonoBehaviour
    {
        private EBehaviour type;

        private NodeEditor tree;
        private List<NodeEditor> nodes;

        private Context context;
        
        [HideInInspector]
        [SerializeField]
        private string behaviourName;

        private void Start()
        {
            context = new Context();
            
            nodes = BehaviourTree.TreeGraph.TreeEditorIO.LoadFileToTree(GetPathToBehaviour());
            tree = nodes[0];

        }

        private void Update()
        {
            
            //if (context.GetSubject().IsAlive())
            {
                tree.Behaviour(context);
            }

        }

        private bool IsState(EBehaviour state)
        {
            return state == type;
        }

        public void SetState(EBehaviour state)
        {
			type = state;
        }

        public INode GetTree()
        {
            return tree;
        }

        public void SetBehaviour(string fileName)
        {
            behaviourName = fileName;
        }

        public string GetBehaviourName()
        {
            return behaviourName;
        }

        public List<NodeEditor> GetNodes()
        {
            return nodes;
        }

        public string GetPathToBehaviour()
        {
            return Application.dataPath + BehaviourTree.TreeGraph.TreeEditor.CATALOG + behaviourName;
        }
    }
}
