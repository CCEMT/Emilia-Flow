using System;
using System.Collections.Generic;
using Emilia.Reference;
using UnityEngine;

namespace Emilia.Flow
{
    [Serializable]
    public class FlowEdgeAsset
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private int _inputNodeId;

        [SerializeField]
        private int _outputNodeId;

        [SerializeField]
        private string _inputPortName;

        [SerializeField]
        private string _outputPortName;

        public int id => this._id;

        public int inputNodeId => this._inputNodeId;
        public int outputNodeId => this._outputNodeId;

        public string inputPortName => this._inputPortName;
        public string outputPortName => this._outputPortName;

        public FlowEdgeAsset(int id, int inputNodeId, int outputNodeId, string inputPortName, string outputPortName)
        {
            this._id = id;
            this._inputNodeId = inputNodeId;
            this._outputNodeId = outputNodeId;
            this._inputPortName = inputPortName;
            this._outputPortName = outputPortName;
        }
    }

    public class FlowEdge : IReference
    {
        private FlowEdgeAsset _asset;
        private FlowGraph _graph;

        private FlowNode _inputNode;
        private FlowNode _outputNode;

        private FlowPort _inputPort;
        private FlowPort _outputPort;

        /// <summary>
        /// 资产
        /// </summary>
        public FlowEdgeAsset asset => this._asset;

        public FlowGraph graph => this._graph;

        /// <summary>
        /// Input节点
        /// </summary>
        public FlowNode inputNode => this._inputNode;

        /// <summary>
        /// Output节点
        /// </summary>
        public FlowNode outputNode => this._outputNode;

        /// <summary>
        /// Input端口
        /// </summary>
        public FlowPort inputPort => this._inputPort;

        /// <summary>
        /// Output端口
        /// </summary>
        public FlowPort outputPort => this._outputPort;

        public void OnInit(FlowEdgeAsset edgeAsset, FlowGraph graph)
        {
            this._asset = edgeAsset;
            this._graph = graph;
        }

        public void OnInitConnection()
        {
            this._inputNode = this._graph.nodeById.GetValueOrDefault(this._asset.inputNodeId);
            this._outputNode = this._graph.nodeById.GetValueOrDefault(this._asset.outputNodeId);

            this._inputPort = this._inputNode.inputPorts.GetValueOrDefault(this._asset.inputPortName);
            this._outputPort = this._outputNode.outputPorts.GetValueOrDefault(this._asset.outputPortName);
        }

        public void OnDispose()
        {
            ReferencePool.Release(this);
        }

        void IReference.Clear()
        {
            this._asset = null;
            this._graph = null;
            this._inputNode = null;
            this._outputNode = null;
            this._inputPort = null;
            this._outputPort = null;
        }
    }
}