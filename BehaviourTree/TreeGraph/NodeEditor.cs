using UnityEngine;
using System.Collections.Generic;
using System;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public abstract class NodeEditor 
                : ScriptableObject
                , INode
            {
                public const float CONST_NODE_SIZE_WIDTH = 128f;
                public const float CONST_NODE_SIZE_HEIGHT = 128f;

                protected Rect window;

                protected string title;
                
                protected Sockets sockets = new Sockets();

                protected ENodeState lastReturn;
                protected long ticks = 0;
                protected Dictionary<ENodeState, int> returnCount = new Dictionary<ENodeState, int>();
                protected NodeMonitor monitor;

                private bool isFirstRun = true;

                public NodeEditor (string title, Vector2 pos)
                {
                    Init(title, new Rect(pos.x, pos.y, CONST_NODE_SIZE_WIDTH, CONST_NODE_SIZE_HEIGHT));
                }

                public NodeEditor(string title, Rect windows)
                {
                    Init(title, windows);
                }

                public NodeEditor(string title, Rect windows, Sockets sockets)
                {
                    Init(title, windows);
                    this.sockets = sockets;
                }

                private void Init(string title, Rect window)
                {
                    SetTitle(title);
                    SetWindow(window);
                    sockets.AddSocket(SocketIO.INPUT, window);
                    Init();
                }

                protected abstract void Init();

                public virtual void DrawWindow(GUISkin skin)
                {
                    GUILayout.Space(8f);

                    GUILayout.Label(title,skin.label);
                }

                public virtual void DrawWindowDebug()
                {

                }

                public void RecalcWindowSize()
                {
                    if (sockets != null)
                    {
                        window = sockets.RecalcSocketsXY(window);
                    }
                }

                public void SetSockets(Sockets sockets)
                {
                    this.sockets = sockets;
                }

                public virtual void DrawLinks(Vector2 scrollPos,NodeMonitor monitor)
                {
                    if (sockets != null)
                    {
                        foreach (var outputSocket in sockets.GetSockets(SocketIO.OUTPUT))
                        {
                            if (outputSocket.connectedTo != null)
                            {
                                var inputSocket = (outputSocket.connectedTo as NodeEditor).GetSocketDimension(SocketIO.INPUT, 0);
                                TreeEditor.DrawLinkBetweenDots(outputSocket.GetGlobalDimension(window).center, inputSocket.center,
                                    monitor != null && monitor.InSequence(outputSocket.connectedTo) ? Sockets.COLOR_RUNNING : Sockets.COLOR_LINK);
                            }
                        }
                    }
                }

                public bool IsInside(Vector2 pos)
                {
                    return window.Contains(pos);
                }
                public void SetTitle(string title)
                {
                    this.title = title;
                }
                public virtual void SetWindow(Rect window)
                {
                    this.window = window;

                    RecalcWindowSize();
                }
                public Rect GetWindow()
                {
                    return window;
                }
                public string GetTitle()
                {
                    return title;
                }
                public Vector2 GetPosition()
                {
                    return new Vector2(window.x, window.y);
                }

                public Vector2 GetDimension()
                {
                    return new Vector2(window.width, window.height);
                }

                public Sockets GetSockets()
                {
                    return sockets;
                }

                public Rect GetSocketDimension(SocketIO io, int index)
                {
                    return sockets.GetSocketGlobalDimension(io, index, window);
                }

                public void AddSocket(SocketIO io)
                {
                    window = sockets.AddSocket(io, window);
                }

                public void RemoveSocket(Socket socket)
                {
                    window = sockets.RemoveSocket(socket, window);
                }
              
                public ENodeState Behaviour(Context data)
                {
                    if (isFirstRun)
                    {
                        OnInit (data);
                        isFirstRun = false;
                    }

                    ENodeState state = OnBehaviour(data);

                    if (state != ENodeState.RUNNING)
                    {
                        ResetHandle();
                    }

                    return state;
                }

                public void ResetHandle()
                {
                    isFirstRun = true;
                    OnReset();
                }

                protected void CountReturn(ENodeState state)
                {
                    if (!returnCount.ContainsKey(state))
                        returnCount.Add(state, 0);
                    returnCount[state]++;
                }

                public Dictionary<ENodeState,int> GetCountReturn()
                {
                    return returnCount;
                }

                public long GetTicks()
                {
                    return ticks;
                }

                public ENodeState GetLastReturn()
                {
                    return lastReturn;
                }

                public bool IsRunningState()
                {
                    return lastReturn == ENodeState.RUNNING;
                }

                protected ENodeState HandleDebug(ENodeState state)
                {
                    lastReturn = state;
                    ticks++;
                    CountReturn(lastReturn);
                    return lastReturn;
                }

                public void SetMonitor(NodeMonitor monitor)
                {
                    this.monitor = monitor;
                }

                public void Monitor()
                {
                    if(monitor != null)
                    {
                        monitor.Record(this);
                    }
                }

                public void SetPosition (Vector2 position)
                {
                    window.x = position.x;
                    window.y = position.y;
                }

                public abstract void OnInit (Context data);
                public abstract ENodeState OnBehaviour(Context data);
                public abstract void OnReset();
                public abstract void UpdateNodesFromSockets();
            }
        }
    }
}

