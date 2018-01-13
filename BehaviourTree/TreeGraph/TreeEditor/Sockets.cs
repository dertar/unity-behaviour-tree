using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public enum SocketIO
            {
                INPUT,
                OUTPUT
            }

            public class Socket
            {
                public Rect dimension;
                public NodeEditor connectedTo;

                public Rect GetGlobalDimension(Rect window)
                {
                    return new Rect(window.x + dimension.x,
                              window.y + dimension.y,
                              dimension.width,
                              dimension.height
                              );
                }
            }
            [Serializable]
            public class Sockets
            {
                public static Color COLOR_LINK = Color.black;
                public static Color COLOR_RUNNING = Color.yellow;
                public static Color COLOR_LINKING = Color.magenta;
                public static Color COLOR_SOCKET_DISCONNECTED = Color.red;
                public static Color COLOR_SOCKET_CONNECTED = Color.green;

                public static float GAP_BETWEEN_SOCKETS = 10f;

                private Dictionary<SocketIO, List<Socket>> sockets;
                private float gap = GAP_BETWEEN_SOCKETS;

                private Vector2 sizeInput = new Vector2(21f, 21f);
                private Vector2 sizeOutput = new Vector2(21f, 21f);

                public Sockets(Dictionary<SocketIO, List<Socket>> sockets)
                {
                    this.sockets = sockets;
                }

                public Sockets()
                {
                    sockets = new Dictionary<SocketIO, List<Socket>>();
                    sockets.Add(SocketIO.INPUT, new List<Socket>());
                    sockets.Add(SocketIO.OUTPUT, new List<Socket>());
                }

                public Rect RecalcSocketsXY(Rect windowSize)
                {
                    KeyValuePair<float[], float[]> pieces = new KeyValuePair<float[], float[]>
                        (GetPiecesXY(sockets[SocketIO.INPUT].Count, sizeInput.x),
                        GetPiecesXY(sockets[SocketIO.OUTPUT].Count, sizeOutput.x));

                    KeyValuePair<float, float> sizes = new KeyValuePair<float, float>
                        (pieces.Key[pieces.Key.Length - 1] + sizeInput.x + GAP_BETWEEN_SOCKETS,
                        pieces.Value[pieces.Value.Length - 1] + sizeOutput.x + GAP_BETWEEN_SOCKETS);

                    windowSize.width = NodeEditor.CONST_NODE_SIZE_WIDTH;

                    if (sizes.Key >= windowSize.width)
                        windowSize.width = sizes.Key;
                    else if (sizes.Value >= windowSize.width)
                        windowSize.width = sizes.Value;

                    RecalcSocketsXY(SocketIO.INPUT, windowSize, pieces.Key, sizes.Key);
                    RecalcSocketsXY(SocketIO.OUTPUT, windowSize, pieces.Value, sizes.Value);

                    return windowSize;
                }

                private void RecalcSocketsXY(SocketIO io, Rect windowSize, float[] pieces, float lenOfPieces)
                {
                    var sockets = this.sockets[io];
                    int len = sockets.Count;

                    float width = io.Equals(SocketIO.INPUT) ? sizeInput.x : sizeOutput.x,
                         height = io.Equals(SocketIO.INPUT) ? sizeInput.y : sizeOutput.y;

                    if (len == 0)
                        return;

                    float offset = (windowSize.width - lenOfPieces) / 2f;

                    for (int i = 0; i < len; i++)
                    {
                        sockets[i].dimension = new Rect(
                            offset + pieces[i],
                            io == SocketIO.INPUT ? 0 : windowSize.height - height,
                            width, height);
                    }
                }

                private float[] GetPiecesXY(int len, float width)
                {
                    if (len == 0)
                        return new float[] { 0f };

                    float[] pieces = new float[len];
                    pieces[0] = gap;

                    for (int i = 1; i < len; i++)
                    {
                        pieces[i] = pieces[i - 1] + width + gap;
                    }
                    return pieces;
                }

                public Socket FindSocketConnectedTo(SocketIO io, NodeEditor selectedInputNode)
                {
                    return sockets[io].Find((x) => x.connectedTo != null && x.connectedTo.Equals(selectedInputNode));
                }

                public Rect GetLocalDimension(SocketIO io, int index)
                {
                    return sockets[io][index].dimension;
                }

                public INode GetNode(SocketIO io, int index)
                {
                    return sockets[io][index].connectedTo;
                }

                public List<Socket> GetSockets(SocketIO io)
                {
                    return sockets[io];
                }

                public Socket GetSocket(SocketIO io, int index)
                {
                    return index >= 0 && index < sockets[io].Count ? sockets[io][index] : null;
                }

                public List<Rect> GetLocalDimensions(SocketIO io)
                {
                    return sockets[io].ConvertAll<Rect>(x => x.dimension);
                }

                public Rect AddSocket(SocketIO io, Rect windowSize)
                {
                    sockets[io].Add(new Socket());
                    return RecalcSocketsXY(windowSize);
                }

                public Rect GetSocketGlobalDimension(SocketIO io, int index, Rect window)
                {
                    return GetSocket(io, index).GetGlobalDimension(window);
                }

                public Rect RemoveSocket(Socket socket, Rect window)
                {
                    Remove(socket, SocketIO.INPUT);
                    Remove(socket, SocketIO.OUTPUT);
                    return RecalcSocketsXY(window);
                }

                private void Remove(Socket socket, SocketIO io)
                {
                    if (sockets[io].Contains(socket))
                        sockets[io].Remove(socket);
                }
            }
        }
    }

}
