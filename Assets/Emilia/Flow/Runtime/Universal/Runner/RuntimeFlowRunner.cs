using Emilia.Reference;
using UnityEngine;

namespace Emilia.Flow
{
    public class RuntimeFlowRunner : IFlowRunner
    {
        private IFlowLoader flowLoader;
        private FlowGraph _flowGraph;

        public int uid { get; private set; }
        public FlowGraphAsset asset => this._flowGraph.graphAsset;
        public FlowGraph graph => this._flowGraph;
        public bool isActive => graph.isActive;

        public void Init(string fileName, IFlowLoader loader, object owner = null)
        {
            this.uid = FlowRunnerUtility.GetId();

            flowLoader = loader;

            string fullPath = $"{this.flowLoader.runtimeFilePath}/{fileName}.bytes";
            TextAsset textAsset = this.flowLoader.LoadAsset(fullPath) as TextAsset;
            FlowGraphAsset flowGraphAsset = this.flowLoader.LoadFlowGraphAsset(textAsset.bytes);

            this._flowGraph = ReferencePool.Acquire<FlowGraph>();
            this._flowGraph.Init(uid, flowGraphAsset, owner);
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
            FlowRunnerUtility.RecycleId(uid);
            uid = -1;

            if (this._flowGraph == null) return;
            if (isActive == false) return;

            this._flowGraph.Dispose();
            this._flowGraph = null;
        }
    }
}