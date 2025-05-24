using System;
using System.Collections.Generic;
using Emilia.Kit;
using UnityEditor;
using UnityEngine;

namespace Emilia.Flow.Editor
{
    public class EditorFlowRunner : IFlowRunner
    {
        private static Dictionary<string, List<EditorFlowRunner>> _runnerByAssetId = new Dictionary<string, List<EditorFlowRunner>>();
        private static Dictionary<int, EditorFlowRunner> _runnerByUid = new Dictionary<int, EditorFlowRunner>();

        public static readonly Dictionary<int, List<int>> nodeStates = new Dictionary<int, List<int>>();
        public static readonly Dictionary<int, Queue<EditorFlowDebugPingMessage>> nodeMessage = new Dictionary<int, Queue<EditorFlowDebugPingMessage>>();

        public static IReadOnlyDictionary<string, List<EditorFlowRunner>> runnerByAssetId => _runnerByAssetId;
        public static IReadOnlyDictionary<int, EditorFlowRunner> runnerByUid => _runnerByUid;

        private object owner;

        private string assetId;
        private FlowGraph _flowGraph;

        public int uid { get; private set; } = -1;
        public string fileName { get; private set; }
        public FlowGraphAsset asset => _flowGraph.graphAsset;
        public FlowGraph graph => _flowGraph;
        public bool isActive => _flowGraph?.isActive ?? false;

        public void Init(string fileName, IFlowLoader loader, object owner = null)
        {
            try
            {
                this.fileName = fileName;

                string fullPath = $"{loader.editorFilePath}/{fileName}.asset";
                EditorFlowAsset loadAsset = AssetDatabase.LoadAssetAtPath<EditorFlowAsset>(fullPath);
                if (loadAsset == null) return;
                Init(loadAsset.cache, owner);
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Init 时出现错误：\n{e.ToUnityLogString()}");
                _flowGraph?.Dispose();
            }
        }

        public void Init(FlowGraphAsset graphAsset, object owner = null)
        {
            uid = FlowRunnerUtility.GetId();
            this.owner = owner;

            _flowGraph = new FlowGraph();
            _flowGraph.Init(uid, graphAsset, owner);

            assetId = graphAsset.id;
            if (_runnerByAssetId.ContainsKey(graphAsset.id) == false) _runnerByAssetId[graphAsset.id] = new List<EditorFlowRunner>();
            _runnerByAssetId[graphAsset.id].Add(this);

            _runnerByUid[uid] = this;
        }

        public void Reload(FlowGraphAsset flowGraphAsset)
        {
            bool isStart = isActive;
            if (this._flowGraph != null) this._flowGraph.Dispose();
            _flowGraph = new FlowGraph();
            _flowGraph.Init(uid, flowGraphAsset, owner);
            if (isStart) Start();
        }

        public void Start()
        {
            if (this._flowGraph == null) return;

            try
            {
                this._flowGraph.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Start 时出现错误：\n{e.ToUnityLogString()}");
                _flowGraph.Dispose();
            }

        }

        public void Update()
        {
            if (this._flowGraph == null) return;
            if (isActive == false) return;

            try
            {
                this._flowGraph.Tick();
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Update 时出现错误：\n{e.ToUnityLogString()}");
                _flowGraph.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                if (_runnerByUid.ContainsKey(uid)) _runnerByUid.Remove(uid);
                if (nodeStates.ContainsKey(uid)) nodeStates.Remove(uid);
                if (nodeMessage.ContainsKey(uid)) nodeMessage.Remove(uid);

                fileName = null;

                if (uid != -1) FlowRunnerUtility.RecycleId(uid);
                uid = -1;

                if (this._flowGraph == null) return;

                if (_runnerByAssetId.ContainsKey(assetId)) _runnerByAssetId[assetId].Remove(this);

                if (isActive) this._flowGraph.Dispose();
                this._flowGraph = null;

                owner = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"{owner} Dispose 时出现错误：\n{e.ToUnityLogString()}");
            }
        }
    }
}