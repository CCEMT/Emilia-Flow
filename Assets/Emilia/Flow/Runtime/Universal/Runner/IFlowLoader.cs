using UnityEngine;

namespace Emilia.Flow
{
    public interface IFlowLoader
    {
        string runtimeFilePath { get; }
        string editorFilePath { get; }

        Object LoadAsset(string path);
        void ReleaseAsset(string path);
        FlowGraphAsset LoadFlowGraphAsset(byte[] bytes);
    }
}