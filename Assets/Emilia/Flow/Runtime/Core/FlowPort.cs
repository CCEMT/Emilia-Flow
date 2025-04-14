using System;
using System.Collections.Generic;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.Flow
{
    [Serializable]
    public class FlowPortAsset
    {
        [SerializeField]
        private string _portName;

        [SerializeField]
        private List<int> _edgeIds;

        public string portName => this._portName;
        public IReadOnlyList<int> edgeIds => this._edgeIds;

        public FlowPortAsset(string portName, List<int> edgeIds)
        {
            this._portName = portName;
            this._edgeIds = edgeIds;
        }
    }

    public class FlowPort : IReference
    {
        private FlowPortAsset _asset;
        private FlowNode _node;

        private List<FlowEdge> _edges = new List<FlowEdge>();
        private Dictionary<int, FlowEdge> _edgeById = new Dictionary<int, FlowEdge>();

        public FlowPortAsset asset => _asset;
        public FlowNode node => _node;
        public IReadOnlyList<FlowEdge> edges => _edges;
        public IReadOnlyDictionary<int, FlowEdge> edgeById => _edgeById;

        public void OnInit(FlowPortAsset portAsset, FlowNode node)
        {
            this._asset = portAsset;
            this._node = node;

            InitEdges();
        }

        private void InitEdges()
        {
            int count = _asset.edgeIds.Count;
            for (int i = 0; i < count; i++)
            {
                int edgeId = _asset.edgeIds[i];
                FlowEdge edge = _node.graph.edgeById.GetValueOrDefault(edgeId);

                this._edges.Add(edge);
                this._edgeById.Add(edgeId, edge);
            }
        }

        /// <summary>
        /// 调用该端口所连接的函数
        /// </summary>
        public void Invoke(object arg)
        {
            Action<object> action = this._node.GetPortAction(this._asset.portName);
            action?.Invoke(arg);
        }

        /// <summary>
        /// 获取该端口所连接的值
        /// </summary>
        public T GetValue<T>(object arg) => this._node.GetPortValue<T>(this._asset.portName, arg);
        
        public void OnDispose()
        {
            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            _edges.Clear();
            _edgeById.Clear();
            _asset = null;
            _node = null;
        }
    }
}