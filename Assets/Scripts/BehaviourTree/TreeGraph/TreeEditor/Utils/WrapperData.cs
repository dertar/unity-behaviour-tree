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
            [Serializable]
            public struct NodeData
            {
                public Rect window;
                public string title;
                public SocketsData sockets;
                public int id;
                public string type;

                public List<StringArgument> argsStr;
                public List<SingleArgument> argsNum;
                public List<BoolArgument> argsBool;

                public string handledLeaf;
                public int reqFail;
                public int reqSucc;

                public string name;
            }
            [Serializable]
            public class SimpleTypeArgument
            {
                public int index;
                public SimpleTypeArgument (int index)
                {
                    this.index = index;
                }

                public object Get ()
                {
                    return null;
                }
            }
            [Serializable]
            public class BoolArgument
                : SimpleTypeArgument
            {
                public bool val;

                public BoolArgument (int index, bool val)
                    : base (index)
                {
                    this.val = val;
                }
            }
            [Serializable]
            public class SingleArgument
                : SimpleTypeArgument
            {
                public float val;

                public SingleArgument (int index, float val)
                    : base (index)
                {
                    this.val = val;
                }
            }
            [Serializable]
            public class StringArgument
                : SimpleTypeArgument
            {
                public string val;

                public StringArgument (int index, string val) 
                    : base(index)
                {
                    this.val = val;
                }
            }



            [Serializable]
            public struct SocketsData
            {
                public List<SocketData> input;
                public List<SocketData> output;
            }

            [Serializable]
            public struct SocketData
            {
                public Rect dimension;
                public int connectedTo;
            }

            [Serializable]
            public class PackedNodeData
            {
                public List<NodeData> nodeData;
            }



            static public class WrapperData
            {
                
                static private NodeData ToData(NodeEditor node, Dictionary<NodeEditor, int> instances)
                {
                    NodeData ret = new NodeData();
                    var n = node as NodeEditor;

                    ret.window = n.GetWindow();
                    ret.title = n.GetTitle();
                    ret.sockets = ToData(n.GetSockets(), instances);
                    ret.id = instances[n];
                    ret.type = node.GetType().ToString();

                    if (typeof(LeafEditor).Equals(node.GetType()))
                    {
                        var leaf = node as LeafEditor;
                        var keys = leaf.GetArgs ();

                        Convert (keys, out ret.argsBool, out ret.argsNum, out ret.argsStr);

                       
                        //ret.argsStr = Select<string> (keys);
                        // ret.argsNum = Select<float> (keys);
                        //ret.argsBool = Select<bool> (keys);



                        ret.handledLeaf = leaf.GetNode().GetType().ToString();
                    }else if(typeof(ParallelEditor).Equals(node.GetType())
                         || typeof (ParallelRunningEditor).Equals (node.GetType ()))
                    {
                        var parallel = node as ParallelEditor;
                        ret.reqFail = parallel.GetReqFail();
                        ret.reqSucc = parallel.GetReqSuccess();
                        ret.name = parallel.GetName();
                    }
                    else if (typeof(CompositeEditor).Equals(node.GetType().BaseType))
                    {
                        var composite = node as CompositeEditor;

                        ret.name = composite.GetName();
                    }

                    return ret;
                }

                static void Convert
                    (List<object> objs
                    , out List<BoolArgument> bools
                    , out List<SingleArgument> singles
                    , out List<StringArgument> strings)
                {
                    bools = new List<BoolArgument> ();
                    singles = new List<SingleArgument> ();
                    strings = new List<StringArgument> ();

                    int index = 0;
                    foreach (var x in objs)
                    {
                        if (x.GetType ().Equals (typeof (string)))
                            strings.Add (new StringArgument (index, (string)x));
                        else if (x.GetType ().Equals (typeof (float)))
                            singles.Add (new SingleArgument (index, (float)x));
                        else if (x.GetType ().Equals (typeof (bool)))
                            bools.Add (new BoolArgument (index, (bool)x));

                        index++;
                    }
                }

                static List<object> Convert
                    (List<BoolArgument> bools
                    , List<SingleArgument> singles
                    , List<StringArgument> strings)
                {
                    int count = bools.Count + singles.Count + strings.Count;

                    List<object> ret = new List<object> ();

                    for (int i = 0; i < count; i++)
                    {

                        var b = bools.SingleOrDefault (x => x.index == i);

                        if (b != null)
                        {
                            ret.Add (b.val);
                            continue;
                        }

                        var s = singles.SingleOrDefault (x => x.index == i);
                        if (s != null)
                        {
                            ret.Add (s.val);
                            continue;
                        }
                        var st = strings.SingleOrDefault (x => x.index == i);
                        if (st != null)
                        {
                            ret.Add (st.val);
                            continue;
                        }
                    }
                    return ret;
                }


                private static void Pack<T> 
                    (Dictionary<string, T> dictStr
                    , out List<string> keyStr
                    , out List<T> valStr)
                {
                    keyStr = dictStr.Keys.ToList ();
                    valStr = dictStr.Values.ToList ();
                }


                static private List<T> Select<T>
                    (Dictionary<string, Triple<object, Type, EPermissionLeaf>> dict)    
                {
                    return dict.Where ((x) =>
                    {
                        return x.Value.First.GetType ().Equals (typeof (T));
                    }).ToList().ConvertAll(x => (T)x.Value.First);
                }


                static private List<T> SelectAndConvert<T>(List<object> objs)
                {
                    return objs.Where((x) =>
                    {
                        return x.GetType().Equals(typeof(T));
                    }).ToList().ConvertAll<T>((x) =>
                    {
                        return (T)x;
                    });
                }

                static private SocketsData ToData(Sockets sockets, Dictionary<NodeEditor, int> instances)
                {
                    SocketsData ret = new SocketsData();

                    ret.input = ToData(sockets.GetSockets(SocketIO.INPUT), instances);
                    ret.output = ToData(sockets.GetSockets(SocketIO.OUTPUT), instances);

                    return ret;
                }

                static private List<SocketData> ToData(List<Socket> sockets, Dictionary<NodeEditor, int> instances)
                {
                    List<SocketData> ret = new List<SocketData>();

                    foreach (var socket in sockets)
                    {
                        var socketData = new SocketData();
                        socketData.dimension = socket.dimension;

                        if(socket.connectedTo != null && instances.ContainsKey(socket.connectedTo))
                        {
                            socketData.connectedTo = socket.connectedTo == null ? -1 : instances[socket.connectedTo];
                        }else
                        {
                            socketData.connectedTo = -1;
                        }

                        ret.Add(socketData);
                    }

                    return ret;
                }

                static public PackedNodeData PackData(List<NodeEditor> nodes)
                {
                    PackedNodeData ret = new PackedNodeData();
                    ret.nodeData = new List<NodeData>();

                    Dictionary<NodeEditor, int> instances = GenerateInstance(nodes);

                    foreach (var node in nodes)
                    {
                        ret.nodeData.Add(ToData(node, instances));
                    }

                    return ret;
                }

                private static Dictionary<NodeEditor, int> GenerateInstance(List<NodeEditor> nodes)
                {
                    Dictionary<NodeEditor, int> ret = new Dictionary<NodeEditor, int>();

                    int i = 0;
                    foreach (var node in nodes)
                    {
                        ret.Add(node, i++);
                    }

                    return ret;
                }


                public static NodeEditor CreateLeafNode (string title, Vector2 pos, Type typeLeaf)
                {
                    return CreateLeafNode (title
                        , new Rect (pos.x, pos.y, NodeEditor.CONST_NODE_SIZE_WIDTH, NodeEditor.CONST_NODE_SIZE_HEIGHT)
                        , typeLeaf
                        , null);
                }


                public static NodeEditor CreateLeafNode (string title, Rect window, Type typeLeaf, List<object> keys)
                {
                    Leaf handleLeaf = keys != null 
                        ? (Leaf)Activator.CreateInstance (typeLeaf, keys.ToArray ())
                        : (Leaf)Activator.CreateInstance (typeLeaf);

                    return (NodeEditor)Activator.CreateInstance (typeof (LeafEditor), new object[] { title, window, handleLeaf });
                }

                public static List<NodeEditor> UnpackData(PackedNodeData data)
                {
                    //List<Node> ret = new List<Node>();
                    Dictionary<NodeEditor, int> instances = new Dictionary<NodeEditor, int>();
                    // Dictionary<int, SocketsData> socketInstances = new Dictionary<int, SocketsData>();

                    int i = 0;
                    foreach (var x in data.nodeData)
                    {

                        NodeEditor node;

                        if(typeof(LeafEditor).ToString().Equals(x.type))
                        {

                            var lst = Convert (x.argsBool, x.argsNum, x.argsStr);
                            //lst.AddRange (x.argsStr.ConvertAll<object>(y => (object)y));
                            //lst.AddRange (x.argsNum.ConvertAll<object> (y => (object)y));
                            //lst.AddRange (x.argsBool.ConvertAll<object> (y => (object)y));
                            

                            node = CreateLeafNode(x.title, x.window, Type.GetType(x.handledLeaf)
                                , lst);
                        }
                        else if (typeof(ParallelEditor).ToString().Equals(x.type) 
                            || typeof (ParallelRunningEditor).ToString ().Equals (x.type))
                        {
                            node = (NodeEditor)Activator.CreateInstance(Type.GetType(x.type), new object[] { x.title, x.window, x.name,x.reqFail, x.reqSucc });
                        }
                        else if (typeof(SequenceEditor).ToString().Equals(x.type) || typeof(SelectorEditor).ToString().Equals(x.type)
                            || typeof(SequenceRandomEditor).ToString().Equals(x.type) || typeof(SelectorRandomEditor).ToString().Equals(x.type))
                        {
                            node = (NodeEditor)Activator.CreateInstance(Type.GetType(x.type), new object[] { x.title, x.window, x.name });
                        }
                        else
                        {
                            node = (NodeEditor)Activator.CreateInstance(Type.GetType(x.type), new object[] { x.title, x.window });
                        }                        

                        instances.Add(node, i++);
                        
                        //socketInstances.Add(i, x.sockets);
                    }
                    i = 0;
                    foreach (var x in instances.Keys)
                    {
                        var node = x as NodeEditor;
                        node.SetSockets(FromData(data.nodeData[i++].sockets, instances));
                    }

                    foreach (var node in instances.Keys.ToList())
                    {
                        node.UpdateNodesFromSockets();
                    }

                    return new List<NodeEditor>(instances.Keys);
                }

                private static Dictionary<string, object> AssembleDictionary<T> (List<string> keyStr, List<T> valStr)
                {
                    return keyStr.Select ((k, j) => new { k, v = valStr[j] }).ToDictionary (y => y.k, y => (object)y.v);
                }

                private static Sockets FromData(SocketsData data, Dictionary<NodeEditor, int> instances)
                {
                    Dictionary<SocketIO, List<Socket>> sockets = new Dictionary<SocketIO, List<Socket>>();
                    sockets.Add(SocketIO.INPUT, CreateSocketWithInstances(data.input, instances));
                    sockets.Add(SocketIO.OUTPUT, CreateSocketWithInstances(data.output, instances));

                    return new Sockets(sockets);
                }

                private static List<Socket> CreateSocketWithInstances(List<SocketData> data, Dictionary<NodeEditor, int> instances)
                {
                    return data.ConvertAll<Socket>((x) =>
                    {
                        Socket socket = new Socket();
                        socket.dimension = x.dimension;
                        socket.connectedTo = instances.FirstOrDefault(y => y.Value == x.connectedTo).Key;

                        return socket;
                    });
                }

            }
        }

    }
}
