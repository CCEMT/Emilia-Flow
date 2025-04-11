using System;
using Object = UnityEngine.Object;

namespace Emilia.Flow
{
    public class FlowLoader : IFlowLoader
    {
        public string runtimeFilePath { get; set; }
        public string editorFilePath { get; set; }
        public Func<string, Object> onLoadAsset { get; set; }
        public Action<string> onReleaseAsset { get; set; }
        public Func<byte[], FlowGraphAsset> onLoadFlowGraphAsset { get; set; }

        public Object LoadAsset(string path) => onLoadAsset?.Invoke(path);
        public void ReleaseAsset(string path) => onReleaseAsset?.Invoke(path);
        public FlowGraphAsset LoadFlowGraphAsset(byte[] bytes) => onLoadFlowGraphAsset?.Invoke(bytes);
    }
}