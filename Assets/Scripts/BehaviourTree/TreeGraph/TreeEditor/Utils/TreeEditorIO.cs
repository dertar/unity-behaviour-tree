using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ai
{
    namespace BehaviourTree
    {
        namespace TreeGraph
        {
            public static class TreeEditorIO
            {
                public static List<NodeEditor> LoadFileToTree(string path)
                {
                    if (path == string.Empty)
                        return null;

                    var json = File.ReadAllText(path);
                    
                    return GetNodes(json);
                }

                public static void SaveTreeToFile(string path, List<NodeEditor> nodes)
                {
                    if (path == string.Empty)
                        return;

                    string json = GetJsonData(nodes);

                    File.WriteAllText(path, json);
                }

                public static string GetJsonData(List<NodeEditor> nodes)
                {
                    return JsonUtility.ToJson(WrapperData.PackData(nodes));
                }

                public static List<NodeEditor> GetNodes(string json)
                {
                    var data = JsonUtility.FromJson<PackedNodeData>(json);

                    return WrapperData.UnpackData(data);
                }
            }

        }
    }
}
