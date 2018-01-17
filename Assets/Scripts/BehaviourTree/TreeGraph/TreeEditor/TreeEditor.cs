using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public class TreeEditor : EditorWindow
            {
                static public string CATALOG = "/Data/Ai/";
                static public string CATALOG_LEAFS = "/BehaviourTree/Pure/Leafs/";

                private enum MOUSE_CLICK
                {
                    LEFT_BUTTON = 0,
                    RIGHT_BUTTON = 1,
                    MIDDLE_BUTTON = 2
                }

                private enum Mode
                {
                    EDIT,
                    LINK,
                    DEBUG
                }

                private Mode mode = Mode.EDIT;
                private float zoom = 1f;

                const float MAX_PAN_WIDTH = 10000f;
                const float MAX_PAN_HEIGHT = 10000f;
                const float WIDTH_PANEL = 200f;
                private static float WIDTH_PANEL_SECONDARY_NUMFIELD = 60f;

                private Vector2 mousePos;
                private Vector2 scrollPos = Vector2.zero;
                private Vector2 grabPos;

                private Rect pan = new Rect (0, 0, MAX_PAN_WIDTH, MAX_PAN_HEIGHT);
                private float OFFSET_HEIGHT_TOOLBAR = 16f;

                private List<NodeEditor> nodes = new List<NodeEditor> ();
                private GameObject debugGameObject;

                private Socket selectedSocket;

                private NodeEditor fromNode;

                private int selectedToolbar = 0;
                private string[] TOOLBAR_BUTTONS = new string[] { "New Ai", "Open Ai", "Save AI as", "Save" };

                public static Vector2 ROOT_START_POSITION = new Vector2 (MAX_PAN_WIDTH / 2 - NodeEditor.CONST_NODE_SIZE_WIDTH / 2, 20f);

                private Texture2D window;

                private KeyValuePair<Texture2D, Texture2D> texturePairSocket;
                private Dictionary<Type, Texture2D> textureIcons;
                private Dictionary<Type, Texture2D> textureIconsRun;
                private Dictionary<Type, Texture2D> textureIconsFailed;

                private NodeMonitor monitor = new NodeMonitor ();

                private GUISkin skin;
                private const float SPEED_GRAB = 5f;

                private string bufferCopy = string.Empty;
                private string behaviourPath = string.Empty;

                [MenuItem ("Window/TreeBehaviour")]
                static void ShowEditor ()
                {
                    TreeEditor editor = EditorWindow.GetWindow<TreeEditor> ();
                    editor.titleContent = new GUIContent ("Tree Behaviour");
                    editor.LoadStyle ();
                }

                private void Awake ()
                {
                    NewBehaviour ();
                }

                private void ConnectMonitorToNodes ()
                {
                    foreach (var node in nodes)
                    {
                        node.SetMonitor (monitor);
                    }
                }

                public void LoadStyle ()
                {
                    //window = AssetDatabase.LoadAssetAtPath("Assets/Data/Texture/window.png", typeof(Texture2D)) as Texture2D;
                    texturePairSocket = new KeyValuePair<Texture2D, Texture2D>
                        (AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/socket_green.png")
                        , (AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/socket_red.png")));

                    skin = AssetDatabase.LoadAssetAtPath ("Assets/Data/NodeSkins/NodeWindow.guiskin", typeof (GUISkin)) as GUISkin;
                    LoadIcons ();
                    CheckIsAllTextureLoaded ();
                }

                private void LoadScripts ()
                {
                    var fullPath = Application.dataPath + CATALOG_LEAFS;
                    var dirs = Directory.GetDirectories (fullPath);

                    foreach (var d in dirs)
                    {
                        var files = Directory.GetFiles (d);
                    }
                }

                private void LoadIcons ()
                {

                    textureIcons = new Dictionary<Type, Texture2D> ();

                    textureIcons.Add (typeof (NodeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/leaf_black.png"));
                    textureIcons.Add (typeof (CompositeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/composite_black.png"));
                    textureIcons.Add (typeof (DecoratorEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/decorator_black.png"));

                    textureIconsRun = new Dictionary<Type, Texture2D> ();
                    textureIconsRun.Add (typeof (NodeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/leaf_green.png"));
                    textureIconsRun.Add (typeof (CompositeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/composite_green.png"));
                    textureIconsRun.Add (typeof (DecoratorEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/decorator_green.png"));

                    textureIconsFailed = new Dictionary<Type, Texture2D> ();
                    textureIconsFailed.Add (typeof (NodeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/leaf_red.png"));
                    textureIconsFailed.Add (typeof (CompositeEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/composite_red.png"));
                    textureIconsFailed.Add (typeof (DecoratorEditor), AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/Data/NodeSkins/decorator_red.png"));

                }

                private void CheckIsAllTextureLoaded ()
                {
                    CheckIsAllTextureLoaded (textureIcons);
                    CheckIsAllTextureLoaded (textureIconsRun);
                    CheckIsAllTextureLoaded (textureIconsFailed);
                }

                private void CheckIsAllTextureLoaded (Dictionary<Type, Texture2D> textures)
                {
                    if (textures == null)
                    {
                        LoadStyle ();
                    }

                    foreach (var pair in textures)
                    {
                        if (pair.Value == null)
                        {
                            throw new NullReferenceException ("Some texture not loaded");
                        }
                    }


                }

                private void NewBehaviour ()
                {
                    scrollPos.x = MAX_PAN_HEIGHT / 2 - position.width / 2 - WIDTH_PANEL;
                    scrollPos.y = 0f;
                    nodes = new List<NodeEditor> ();
                    nodes.Add (new RootEditor ("Root", ROOT_START_POSITION));
                    behaviourPath = string.Empty;
                }

                private void LoadAiFromFile (string behaviourPath)
                {
                    if (behaviourPath != string.Empty)
                    {
                        nodes = TreeEditorIO.LoadFileToTree (behaviourPath);
                        this.behaviourPath = behaviourPath;
                        RecalcWindowNodes ();
                    }
                }


                private void HandlePanel ()
                {
                    switch (selectedToolbar)
                    {
                        case -1:
                            return;
                        case 0:
                            {
                                if (EditorUtility.DisplayDialog ("New Ai", "Are u sure about this?", "Plus", "Minus"))
                                {
                                    NewBehaviour ();
                                    Repaint ();
                                }
                            }
                            break;
                        case 1:
                            {
                                var path = EditorUtility.OpenFilePanel (
                                                    "Open Ai file",
                                                    Application.dataPath + CATALOG,
                                                    "AiBT");

                                LoadAiFromFile (path);

                            }
                            break;

                        case 2:
                            {
                                SaveFilePanel ();

                            }
                            break;
                        case 3:
                            {
                                ;
                                if (behaviourPath != string.Empty)
                                {
                                    TreeEditorIO.SaveTreeToFile (behaviourPath, nodes);
                                }
                                else
                                {
                                    SaveFilePanel ();
                                }
                            }
                            break;

                    }
                    selectedToolbar = -1;
                }

                private void SaveFilePanel ()
                {
                    var path = EditorUtility.SaveFilePanel (
                                                    "Save Ai",
                                                    Application.dataPath + CATALOG,
                                                    "",
                                                    "AiBT");
                    behaviourPath = path;
                    TreeEditorIO.SaveTreeToFile (path, nodes);
                }

                private void RecalcWindowNodes ()
                {
                    foreach (var node in nodes)
                    {
                        node.RecalcWindowSize ();
                    }
                }

                private void HandleDebugMode (Event e)
                {
                    if (e.button == (int)MOUSE_CLICK.LEFT_BUTTON)
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            GetSelectNewNode ();
                        }
                    }
                }

                private void GetSelectNewNode ()
                {
                    var selectedNode = GetSelectedNode ();
                    if (selectedNode != null && fromNode != selectedNode)
                        fromNode = selectedNode;
                }

                private void HandleEditMode (Event e)
                {
                    if (e.button == (int)MOUSE_CLICK.RIGHT_BUTTON)
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            var selectedNode = GetSelectedNode ();


                            if (selectedNode != null)
                            {
                                selectedSocket = selectedNode.GetSockets ().GetSockets (SocketIO.OUTPUT).SingleOrDefault (
                                    (x) =>
                                    {
                                        return x.GetGlobalDimension (selectedNode.GetWindow ()).Contains (mousePos + scrollPos);
                                    }
                                    );

                                ShowContextMenu (selectedSocket != null ? selectedSocket.GetType () : selectedNode.GetType (), selectedNode);
                            }
                            else
                            {
                                ShowContextMenu (null, null);
                            }
                            e.Use ();
                        }

                    }
                    else if (e.button == (int)MOUSE_CLICK.LEFT_BUTTON)
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            GetSelectNewNode ();
                        }

                    }
                }


                private void HandleLinkMode (Event e)
                {
                    if (e.button == (int)MOUSE_CLICK.LEFT_BUTTON)
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            var selectedNode = GetSelectedNode ();
                            if (selectedNode != null)
                            {
                                var inputSelectedSocket = selectedNode.GetSockets ().GetSocket (SocketIO.INPUT, 0);
                                if (inputSelectedSocket != null)
                                {
                                    if (selectedSocket.connectedTo != null)
                                    {
                                        DisconnectInputSocket (selectedSocket.connectedTo);
                                    }

                                    DisconnectInputSocket (selectedNode);
                                    DisconnectOutputSocket (inputSelectedSocket);

                                    selectedSocket.connectedTo = selectedNode;
                                    var selectedInputSocket = selectedNode.GetSockets ().GetSocket (SocketIO.INPUT, 0);
                                    selectedInputSocket.connectedTo = fromNode;

                                    mode = Mode.EDIT;
                                    fromNode = null;
                                }
                            }
                        }
                    }
                }

                private void DrawLink (Event e)
                {
                    if (e.keyCode == KeyCode.Escape)
                    {
                        mode = Mode.EDIT;
                        fromNode = null;
                    }
                    else
                    {
                        Handles.color = Sockets.COLOR_LINKING;
                        var output = selectedSocket.dimension.center + fromNode.GetPosition ();

                        Handles.DrawLine (output - scrollPos, Event.current.mousePosition);
                    }
                }

                private Rect ZoomWindow (Rect rect, float zoom)
                {

                    return new Rect (rect.x, rect.y, rect.width * zoom, rect.height * zoom);
                }

                private void DrawGUI ()
                {
                    GUILayout.BeginHorizontal ();
                    scrollPos = EditorGUILayout.BeginScrollView (scrollPos, true, true);

                    GUILayout.BeginHorizontal ();
                    GUILayout.Space (pan.width);
                    GUILayout.EndHorizontal ();

                    GUILayout.BeginVertical ();
                    GUILayout.Space (pan.height);
                    GUILayout.EndVertical ();

                    DrawGrid (20, 0.2f, Color.gray);
                    DrawGrid (100, 0.5f, Color.gray);
                    BeginWindows ();

                    //CheckIsAllTextureLoaded();
                    if (nodes == null)
                    {
                        LoadAiFromFile (behaviourPath);
                        Debug.Log ("Reload nodes");
                    }

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = (nodes[i] as NodeEditor);
                        if (node != null)
                        {
                            Texture2D texture;
                            try
                            {


                                texture = textureIcons[GetBaseType (node.GetType ())];
                            }
                            finally
                            {

                                LoadStyle ();
                                texture = textureIcons[GetBaseType (node.GetType ())];
                            }

                            var textureRun = textureIconsRun[GetBaseType (node.GetType ())];
                            var textureFailed = textureIconsFailed[GetBaseType (node.GetType ())];

                            if (mode.Equals (Mode.EDIT) && grabPos == Vector2.zero)
                            {
                                node.SetPosition (GUI.Window (i, ZoomWindow (node.GetWindow (), zoom), Draw, texture /*node.GetTitle()*/, skin.window).position);
                            }
                            else
                            {
                                /*var last = node.GetLastReturn();
                                if (last.Equals(ENodeState.FAILURE))
                                {
                                    GUI.Window(i, ZoomWindow(node.GetWindow(), zoom), Draw, textureFailed, skin.window);
                                }
                                else
                                {*/
                                if (monitor.InSequence (node))
                                {
                                    GUI.Window (i, ZoomWindow (node.GetWindow (), zoom), Draw, textureRun, skin.window);
                                }
                                else
                                {
                                    GUI.Window (i, ZoomWindow (node.GetWindow (), zoom), Draw, texture, skin.window);
                                }
                                //}
                            }
                            node.DrawLinks (scrollPos, monitor);
                        }
                    }
                    EndWindows ();
                    EditorGUILayout.EndScrollView ();

                    GUILayout.BeginVertical ();
                    selectedToolbar = GUILayout.SelectionGrid (-1, TOOLBAR_BUTTONS, 1, GUILayout.MinWidth (WIDTH_PANEL), GUILayout.MaxWidth (WIDTH_PANEL));

                    if (behaviourPath != string.Empty)
                    {
                        GUILayout.Label ("Ai: " + behaviourPath.Split ('/').ToList ().Last (), GUILayout.MaxWidth (WIDTH_PANEL));
                    }
                    GUILayout.Space (8f);

                    DrawZoomComponent ();

                    if (fromNode != null)
                    {
                        GUILayout.Label ("Node: " + fromNode.GetTitle (), GUILayout.MaxWidth (WIDTH_PANEL));

                        if (typeof (LeafEditor).Equals (fromNode.GetType ()))
                        {
                            var leaf = fromNode as LeafEditor;

                            DrawPanelForLeaf (leaf);
                        }
                        else if (typeof (RootEditor).Equals (fromNode.GetType ()))
                        {
                            var context = (fromNode as RootEditor).GetContext ();
                            if (context != null)
                            {

                                foreach (var pair in context.Get ())
                                {
                                    GUILayout.Label ("Key: " + pair.Key, GUILayout.MaxWidth (WIDTH_PANEL));
                                    GUILayout.Label ("Type: " + pair.GetType(), GUILayout.MaxWidth (WIDTH_PANEL));

                                    GUILayout.TextField (pair.Value.ToString (), GUILayout.MaxWidth (WIDTH_PANEL));
                                }
                            }
                        }
                        else if (typeof (ParallelEditor).Equals (fromNode.GetType ())
                             || typeof (ParallelRunningEditor).Equals (fromNode.GetType ()))
                        {
                            var composite = fromNode as ParallelEditor;

                            GUILayout.Label ("Name: ", GUILayout.MaxWidth (WIDTH_PANEL));
                            composite.SetName (GUILayout.TextField (composite.GetName (), GUILayout.MaxWidth (WIDTH_PANEL)));

                            GUILayout.Label ("Request to Fail", GUILayout.MaxWidth (WIDTH_PANEL));
                            composite.SetReqFail (EditorGUILayout.IntField (composite.GetReqFail (), GUILayout.MaxWidth (WIDTH_PANEL_SECONDARY_NUMFIELD)));

                            GUILayout.Label ("Request to Success", GUILayout.MaxWidth (WIDTH_PANEL));
                            composite.SetReqSuccess (EditorGUILayout.IntField (composite.GetReqSuccess (), GUILayout.MaxWidth (WIDTH_PANEL_SECONDARY_NUMFIELD)));

                        }
                        else if (typeof (CompositeEditor).Equals (fromNode.GetType ().BaseType))
                        {
                            var composite = fromNode as CompositeEditor;

                            GUILayout.Label ("Name: ", GUILayout.MaxWidth (WIDTH_PANEL));
                            composite.SetName (GUILayout.TextField (composite.GetName (), GUILayout.MaxWidth (WIDTH_PANEL)));
                        }



                        if (mode.Equals (Mode.DEBUG))
                        {
                            GUILayout.Space (8f);

                            GUILayout.Label ("---------------DEBUG----------------", GUILayout.MaxWidth (WIDTH_PANEL));
                            var lastReturn = fromNode.GetLastReturn ();
                            var returnCount = fromNode.GetCountReturn ();

                            foreach (var pair in returnCount)
                            {
                                GUILayout.Label (pair.Key.ToString () + "", GUILayout.MaxWidth (WIDTH_PANEL));

                                GUILayout.TextField (pair.Value + "", GUILayout.MaxWidth (WIDTH_PANEL));
                            }

                            GUILayout.Label ("Last return:", GUILayout.MaxWidth (WIDTH_PANEL));
                            GUILayout.TextField (lastReturn.ToString (), GUILayout.MaxWidth (WIDTH_PANEL));
                            GUILayout.Label ("Ticks:", GUILayout.MaxWidth (WIDTH_PANEL));
                            GUILayout.TextField (fromNode.GetTicks () + "", GUILayout.MaxWidth (WIDTH_PANEL));
                        }
                    }

                    GUILayout.EndVertical ();

                    GUILayout.EndHorizontal ();
                }

                private Type GetBaseType (Type type)
                {

                    if (type.Equals (typeof (ParallelRunningEditor))
                        || type.Equals (typeof (SequenceRandomEditor))
                        || type.Equals (typeof (SelectorRandomEditor)))
                        return type.BaseType.BaseType;

                    return type.BaseType;
                }

                private KeyValuePair<float, float> boundZoom = new KeyValuePair<float, float> (0.5f, 2f);

                private void DrawZoomComponent ()
                {
                    GUILayout.Label ("Zoom", GUILayout.MaxWidth (WIDTH_PANEL));
                    GUILayout.BeginHorizontal ();
                    zoom = GUILayout.HorizontalSlider (zoom, boundZoom.Key, boundZoom.Value, GUILayout.MaxWidth (WIDTH_PANEL - WIDTH_PANEL_SECONDARY_NUMFIELD));
                    zoom = EditorGUILayout.FloatField (zoom, GUILayout.MaxWidth (WIDTH_PANEL_SECONDARY_NUMFIELD));
                    zoom = Mathf.Clamp (zoom, boundZoom.Key, boundZoom.Value);


                    GUILayout.EndHorizontal ();
                }

                private void DrawPanelForLeafDynamicValues (LeafEditor leaf)
                {
                    var dynamic = leaf.GetDynamicKeys ();

                    foreach (var key in dynamic.Keys.ToList ())
                    {
                        GUILayout.Label ("Key: " + key, GUILayout.MaxWidth (WIDTH_PANEL));
                        GUILayout.Label (dynamic[key].type + "," + dynamic[key].permission, GUILayout.MaxWidth (WIDTH_PANEL));
                        dynamic[key].keyToContext = GUILayout.TextField (dynamic[key].keyToContext, GUILayout.MaxWidth (WIDTH_PANEL));
                    }
                }

                private void DrawPanelForLeafStaticValues (LeafEditor leaf)
                {
                    var staticVals = leaf.GetStaticKeys ();

                    foreach (var key in staticVals.Keys.ToList ())
                    {
                        if (staticVals[key].staticValue.GetType ().Equals (typeof (float)))
                        {
                            GUILayout.Label ("Key: " + key, GUILayout.MaxWidth (WIDTH_PANEL));
                            GUILayout.Label (staticVals[key].staticValue.GetType() + ", Read Only", GUILayout.MaxWidth (WIDTH_PANEL));
                            float num = (float)staticVals[key].staticValue;
                            GUILayout.BeginHorizontal ();
                            num = GUILayout.HorizontalSlider (num, 0f, 100f, GUILayout.MaxWidth (WIDTH_PANEL - WIDTH_PANEL_SECONDARY_NUMFIELD));
                            num = EditorGUILayout.FloatField (num, GUILayout.MaxWidth (WIDTH_PANEL_SECONDARY_NUMFIELD));

                            staticVals[key].staticValue = num;
                            GUILayout.EndHorizontal ();
                        }
                        else if (staticVals[key].staticValue.GetType ().Equals (typeof (bool)))
                        {
                            GUILayout.Label (staticVals[key].staticValue.GetType() + ", Read Only", GUILayout.MaxWidth (WIDTH_PANEL));
                            staticVals[key].staticValue = GUILayout.Toggle ((bool)staticVals[key].staticValue, key, GUILayout.MaxWidth (WIDTH_PANEL));
                        }
                        else if (staticVals[key].staticValue.GetType ().Equals (typeof (string)))
                        {
                            GUILayout.Label (staticVals[key].staticValue.GetType () + ", Read Only", GUILayout.MaxWidth (WIDTH_PANEL));
                            staticVals[key].staticValue = GUILayout.TextField ((string)staticVals[key].staticValue, GUILayout.MaxWidth (WIDTH_PANEL));
                        }
                    }
                }

                private void DrawPanelForLeaf (LeafEditor leaf)
                {
                    DrawPanelForLeafDynamicValues (leaf);
                    DrawPanelForLeafStaticValues (leaf);

                }

                private void OnGUI ()
                {
                    Event e = Event.current;
                    mousePos = e.mousePosition;

                    HandleGrab (e);

                    if (mode.Equals (Mode.EDIT))
                    {
                        HandleEditMode (e);
                    }
                    else if (mode.Equals (Mode.LINK))
                    {
                        DrawLink (e);
                        HandleLinkMode (e);
                    }
                    else if (mode.Equals (Mode.DEBUG))
                    {
                        HandleDebugMode (e);
                    }
                    DrawGUI ();

                    HandlePanel ();
                    Repaint ();

                }

                private void HandleGrab (Event e)
                {

                    if (e.button == (int)MOUSE_CLICK.MIDDLE_BUTTON)
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            grabPos = mousePos;
                        }
                        else if (e.type == EventType.MouseDrag)
                        {
                            var vec = grabPos - mousePos;
                            vec.Normalize ();
                            scrollPos += vec * SPEED_GRAB;
                            grabPos = mousePos;
                        }
                    }
                    if (e.type == EventType.MouseUp)
                    {
                        grabPos = Vector2.zero;
                    }
                }

                public static void DrawLinkBetweenDots (Vector2 start, Vector2 end, Color color)
                {
                    Handles.color = color;

                    Handles.DrawLine (start, end);
                }


                void Update ()
                {
                    if (nodes == null)
                    {
                        NewBehaviour ();
                    }

                    if (Application.isPlaying && debugGameObject != Selection.activeGameObject)
                    {
                        var behaviour = Selection.activeGameObject.GetComponent<Behaviour> ();
                        if (behaviour)
                        {
                            nodes = behaviour.GetNodes ();
                            ConnectMonitorToNodes ();
                            RecalcWindowNodes ();
                            mode = Mode.DEBUG;
                            behaviourPath = behaviour.GetPathToBehaviour ();
                            Repaint ();
                        }
                    }
                    else if (!Application.isPlaying && mode.Equals (Mode.DEBUG))
                    {
                        mode = Mode.EDIT;
                        LoadAiFromFile (behaviourPath);
                        Repaint ();
                    }

                    if (mode.Equals (Mode.LINK))
                    {
                        Repaint ();
                    }
                }



                private void ShowContextMenu (Type type, NodeEditor node)
                {
                    GenericMenu menu = new GenericMenu ();
                    if (type == null)
                    {
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (SequenceEditor).ToString ())), false, ContextCallback, typeof (SequenceEditor));
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (SelectorEditor).ToString ())), false, ContextCallback, typeof (SelectorEditor));
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (SequenceRandomEditor).ToString ())), false, ContextCallback, typeof (SequenceRandomEditor));
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (SelectorRandomEditor).ToString ())), false, ContextCallback, typeof (SelectorRandomEditor));
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (ParallelEditor).ToString ())), false, ContextCallback, typeof (ParallelEditor));
                        menu.AddItem (new GUIContent ("Compositers/" + PrettyTitle (typeof (ParallelRunningEditor).ToString ())), false, ContextCallback, typeof (ParallelRunningEditor));


                        menu.AddItem (new GUIContent ("Decorators/" + PrettyTitle (typeof (RepeaterEditor).ToString ())), false, ContextCallback, typeof (RepeaterEditor));
                        menu.AddItem (new GUIContent ("Decorators/" + PrettyTitle (typeof (RepeaterUntilFailEditor).ToString ())), false, ContextCallback, typeof (RepeaterUntilFailEditor));
                        menu.AddItem (new GUIContent ("Decorators/" + PrettyTitle (typeof (InverterEditor).ToString ())), false, ContextCallback, typeof (InverterEditor));
                        menu.AddItem (new GUIContent ("Decorators/" + PrettyTitle (typeof (SucceederEditor).ToString ())), false, ContextCallback, typeof (SucceederEditor));

                        menu.AddItem (new GUIContent ("Leafs/Debug/" + PrettyTitle (typeof (Leafs.Log).ToString ())), false, ContextCallback, typeof (Leafs.Log));
                        menu.AddItem (new GUIContent ("Leafs/Debug/" + PrettyTitle (typeof (Leafs.Failed).ToString ())), false, ContextCallback, typeof (Leafs.Failed));
                        menu.AddItem (new GUIContent ("Leafs/Debug/" + PrettyTitle (typeof (Leafs.Successed).ToString ())), false, ContextCallback, typeof (Leafs.Successed));

                        menu.AddItem (new GUIContent ("Leafs/Time/" + PrettyTitle (typeof (Leafs.Cap).ToString ())), false, ContextCallback, typeof (Leafs.Cap));

                 
                        menu.AddItem (new GUIContent ("Leafs/Statements/" + PrettyTitle (typeof (Leafs.Bigger).ToString ())), false, ContextCallback, typeof (Leafs.Bigger));



                        if (bufferCopy != string.Empty)
                        {
                            menu.AddItem (new GUIContent ("Insert node(s)"), false, ContextInsert, mousePos);
                        }
                    }
                    else if (typeof (CompositeEditor).IsAssignableFrom (type))
                    {
                        menu.AddItem (new GUIContent ("Add output socket"), false, ContextAddOutputSocket, node);
                        menu.AddItem (new GUIContent ("Remove node"), false, ContextRemoveNode, node);
                        menu.AddItem (new GUIContent ("Copy bunch"), false, ContextCopy, node);
                    }
                    else if (typeof (DecoratorEditor).IsAssignableFrom (type) && !typeof (RootEditor).Equals (type))
                    {
                        menu.AddItem (new GUIContent ("Remove node"), false, ContextRemoveNode, node);
                        menu.AddItem (new GUIContent ("Copy bunch"), false, ContextCopy, node);
                    }
                    else if (typeof (Socket).Equals (type))
                    {
                        menu.AddItem (new GUIContent ("Connect to"), false, ContextConnectSocket, node);
                        menu.AddItem (new GUIContent ("Disconnect"), false, ContextDisconnectSocket, node);
                        menu.AddItem (new GUIContent ("Remove socket"), false, ContextRemoveSocket, node);
                    }
                    else if (typeof (LeafEditor).Equals (type))
                    {
                        menu.AddItem (new GUIContent ("Remove node"), false, ContextRemoveNode, node);
                        menu.AddItem (new GUIContent ("Copy node"), false, ContextCopy, node);
                    }

                    menu.ShowAsContext ();
                }

                private void ContextInsert (object obj)
                {
                    var pos = (Vector2)obj;

                    var nodes = TreeEditorIO.GetNodes (bufferCopy);

                    ShiftWindowPositionNodes (nodes, scrollPos + pos - nodes[0].GetPosition ());

                    this.nodes.AddRange (nodes);
                }


                private void ShiftWindowPositionNodes (List<NodeEditor> nodes, Vector2 pos)
                {
                    foreach (var node in nodes)
                    {
                        var rect = node.GetWindow ();
                        rect.x += pos.x;
                        rect.y += pos.y;
                        node.SetWindow (rect);
                    }
                }

                private NodeEditor GetSelectedNode ()
                {

                    return nodes != null
                        ? nodes.SingleOrDefault (x => x.IsInside (mousePos + scrollPos))
                        : null;
                }

                private void DrawSockets (NodeEditor node, SocketIO io)
                {
                    var sockets = node.GetSockets ().GetSockets (io);

                    foreach (var socket in sockets)
                    {
                        GUI.DrawTexture (socket.dimension, socket.connectedTo == null ? texturePairSocket.Value : texturePairSocket.Key);
                    }
                }

                private void DrawSockets (NodeEditor node, SocketIO io, float zoom)
                {
                    var sockets = node.GetSockets ().GetSockets (io);

                    foreach (var socket in sockets)
                    {

                        var pos = socket.dimension;
                        pos.x *= zoom;
                        pos.y *= zoom;
                        GUI.DrawTexture (pos, socket.connectedTo == null ? texturePairSocket.Value : texturePairSocket.Key);
                    }
                }

                void Draw (int id)
                {
                    var node = nodes[id] as NodeEditor;

                    /*if(mode.Equals(Mode.DEBUG))
                    {
                        node.DrawWindowDebug();
                    }else
                    {
                        node.DrawWindow(skin);
                    }*/



                    node.DrawWindow (skin);

                    DrawSockets (node, SocketIO.INPUT, zoom);
                    DrawSockets (node, SocketIO.OUTPUT, zoom);

                    GUI.DragWindow ();
                }

                private string PrettyTitle (string className)
                {
                    return DelimByHighRegisterSymbols (className.ToString ().Split ('.').Last ());
                }

                private string DelimByHighRegisterSymbols (string name)
                {
                    if (name.Length == 0)
                        return name;
                    string ret = "" + name[0];
                    for (int i = 1; i < name.Length; i++)
                    {
                        if (Char.IsUpper (name[i]))
                        {
                            ret += " ";
                        }
                        ret += name[i];

                    }
                    return ret;
                }

                void ContextCallback (object obj)
                {
                    Type type = obj as Type;
                    Vector2 pos = mousePos + scrollPos;


                    NodeEditor node = typeof (DecoratorEditor).IsAssignableFrom (type)
                        || typeof (CompositeEditor).IsAssignableFrom (type)
                        ? (NodeEditor)Activator.CreateInstance (type, new object[] { PrettyTitle (type.ToString ()), pos })
                        : WrapperData.CreateLeafNode (PrettyTitle (type.ToString ()), pos, type);

                    if (node != null)
                    {
                        nodes.Add (node);
                    }

                }

                private void ContextAddOutputSocket (object obj)
                {
                    (obj as NodeEditor).AddSocket (SocketIO.OUTPUT);
                }

                private void ContextConnectSocket (object obj)
                {
                    mode = Mode.LINK;

                    fromNode = obj as NodeEditor;

                    fromNode.UpdateNodesFromSockets ();
                }

                private void ContextDisconnectSocket (object obj)
                {
                    DisconnectOutputSocket (selectedSocket);
                }

                private void ContextRemoveNode (object obj)
                {
                    var selectedNode = (obj as NodeEditor);
                    var outputSockets = selectedNode.GetSockets ().GetSockets (SocketIO.OUTPUT);
                    foreach (var socket in outputSockets)
                    {
                        DisconnectOutputSocket (socket);
                    }
                    DisconnectInputSocket (selectedNode);
                    nodes.Remove (selectedNode);
                    selectedNode = null;
                }

                private void ContextCopy (object obj)
                {
                    var node = obj as NodeEditor;
                    List<NodeEditor> nodes = new List<NodeEditor> ();
                    nodes.Add (node);
                    GetChildren (node, nodes);
                    Debug.Log ("nodes copied " + nodes.Count);

                    bufferCopy = TreeEditorIO.GetJsonData (nodes);
                    Debug.Log (bufferCopy);
                }

                private void GetChildren (NodeEditor node, List<NodeEditor> nodes)
                {
                    if (node != null)
                    {

                        var sockets = node.GetSockets ().GetSockets (SocketIO.OUTPUT);
                        if (sockets != null)
                        {
                            var children = sockets.ConvertAll<NodeEditor> (x => x.connectedTo).Where (x => x != null);

                            nodes.AddRange (children);

                            foreach (var child in children)
                            {
                                GetChildren (child, nodes);
                            }
                        }
                    }
                }


                private void DisconnectOutputSocket (Socket outputSocket)
                {
                    if (outputSocket != null && outputSocket.connectedTo != null)
                    {
                        var inputSocket = (outputSocket.connectedTo as NodeEditor).GetSockets ().GetSocket (SocketIO.INPUT, 0);

                        inputSocket.connectedTo = null;
                    }

                    outputSocket.connectedTo = null;
                }

                private void DisconnectInputSocket (NodeEditor node)
                {
                    var inputSocket = node.GetSockets ().GetSocket (SocketIO.INPUT, 0);

                    if (inputSocket != null && inputSocket.connectedTo != null)
                    {
                        var outputSocket = (inputSocket.connectedTo as NodeEditor).GetSockets ().FindSocketConnectedTo (SocketIO.OUTPUT, node);
                        if (outputSocket != null)
                        {
                            outputSocket.connectedTo = null;
                        }
                    }

                    inputSocket.connectedTo = null;
                }

                private void ContextRemoveSocket (object obj)
                {
                    DisconnectOutputSocket (selectedSocket);
                    (obj as NodeEditor).RemoveSocket (selectedSocket);
                }

                private void DrawGrid (float grid, float opacity, Color color)
                {
                    int widthCells = Mathf.CeilToInt (pan.width / grid);
                    int heightCells = Mathf.CeilToInt (pan.height / grid);

                    Handles.BeginGUI ();
                    Handles.color = new Color (color.r, color.g, color.b, opacity);

                    for (int i = 0; i < widthCells; i++)
                    {
                        Handles.DrawLine (
                            new Vector3 (i * grid, 0, 0),
                            new Vector3 (i * grid, pan.width, 0)
                            );
                    }

                    for (int i = 0; i < heightCells; i++)
                    {
                        Handles.DrawLine (
                           new Vector3 (0, i * grid, 0),
                           new Vector3 (pan.height, i * grid, 0)
                           );
                    }


                    Handles.color = Color.white;
                    Handles.EndGUI ();
                }
            }
        }
    }

}
