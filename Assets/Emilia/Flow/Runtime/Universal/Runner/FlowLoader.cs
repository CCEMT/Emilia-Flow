using System;
using Object = UnityEngine.Object;

namespace Emilia.Flow
{
    public class FlowLoader : IFlowLoader
    {
        public string runtimeFilePath { get; set; }
        public string editorFilePath { get; set; }
        public Func<string, Object> onLoadAsset { get; set; }
        public Func<byte[], FlowGraphAsset> onLoadFlowGraphAsset { get; set; }

        public Object LoadAsset(string path)
        {
            return onLoadAsset?.Invoke(path);
        }

        public FlowGraphAsset LoadFlowGraphAsset(byte[] bytes)
        {
            return onLoadFlowGraphAsset?.Invoke(bytes);
        }
    }
}