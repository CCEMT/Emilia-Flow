using Emilia.Reference;
using UnityEngine;

namespace Emilia.Flow
{
    public class RuntimeFlowRunner : IFlowRunner, IReference
    {
        private FlowGraph _flowGraph;

        public int uid { get; private set; }
        public string fileName { get; private set; }
        public FlowGraphAsset asset => this._flowGraph.graphAsset;
        public FlowGraph graph => this._flowGraph;
        public bool isActive => graph?.isActive ?? false;

        public void Init(string fileName, IFlowLoader loader, object owner = null)
        {
            this.fileName = fileName;
            string fullPath = $"{loader.runtimeFilePath}/{fileName}.bytes";
            TextAsset textAsset = loader.LoadAsset(fullPath) as TextAsset;
            if (textAsset == null) return;

            FlowGraphAsset flowGraphAsset = loader.LoadFlowGraphAsset(textAsset.bytes);
            if (flowGraphAsset == null) return;

            loader.ReleaseAsset(fullPath);

            Init(flowGraphAsset, owner);
        }

        public void Init(FlowGraphAsset graphAsset, object owner = null)
        {
            uid = FlowRunnerUtility.GetId();

            this._flowGraph = ReferencePool.Acquire<FlowGraph>();
            this._flowGraph.Init(uid, graphAsset, owner);
        }

        public void Start()
        {
            if (this._flowGraph == null) return;
            this._flowGraph.Start();
        }

        public void Update()
        {
            if (this._flowGraph == null) return;
            if (isActive == false) return;

            this._flowGraph.Tick();
        }

        public void Dispose()
        {
            if (this._flowGraph != null && isActive) this._flowGraph.Dispose();
            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            fileName = null;
            if (uid != -1) FlowRunnerUtility.RecycleId(uid);
            uid = -1;

            this._flowGraph = null;
        }
    }
}