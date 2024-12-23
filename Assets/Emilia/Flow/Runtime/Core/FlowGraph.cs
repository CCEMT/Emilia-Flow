﻿using System;
using System.Collections.Generic;
using Emilia.Reference;
using Emilia.Variables;
using UnityEngine;

namespace Emilia.Flow
{
    [Serializable]
    public class FlowGraphAsset
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private string _description;

        [SerializeField]
        private List<FlowNodeAsset> _nodes;

        [SerializeField]
        private List<FlowEdgeAsset> _edges;

        [SerializeField]
        private VariablesManage _variablesManage;

        public string id => this._id;
        public string description => this._description;
        public IReadOnlyList<FlowNodeAsset> nodes => this._nodes;
        public IReadOnlyList<FlowEdgeAsset> edges => this._edges;

        public VariablesManage variablesManage => this._variablesManage;

        public FlowGraphAsset(string id, string description, List<FlowNodeAsset> nodes, List<FlowEdgeAsset> edges, VariablesManage variablesManage)
        {
            this._id = id;
            this._description = description;
            this._nodes = nodes;
            this._edges = edges;
            this._variablesManage = variablesManage;
        }
    }

    public class FlowGraph : IReference
    {
        private List<FlowNode> _nodes = new List<FlowNode>();
        private List<FlowEdge> _edges = new List<FlowEdge>();

        private Dictionary<int, FlowNode> _nodeById = new Dictionary<int, FlowNode>();
        private Dictionary<int, FlowEdge> _edgeById = new Dictionary<int, FlowEdge>();

        public IReadOnlyList<FlowNode> nodes => this._nodes;
        public IReadOnlyList<FlowEdge> edges => this._edges;
        public IReadOnlyDictionary<int, FlowNode> nodeById => this._nodeById;
        public IReadOnlyDictionary<int, FlowEdge> edgeById => this._edgeById;

        public int uid { get; private set; }
        public FlowGraphAsset graphAsset { get; private set; }
        public VariablesManage variablesManage { get; private set; }
        public object owner { get; private set; }
        public FlowGraph parent { get; private set; }
        public List<FlowGraph> children { get; private set; } = new List<FlowGraph>();
        public bool isActive { get; private set; }

        public event Action onStart;
        public event Action onTick;

        public void Init(int uid, FlowGraphAsset graphAsset, object owner = null, FlowGraph parent = null)
        {
            this.uid = uid;
            this.graphAsset = graphAsset;
            variablesManage = graphAsset.variablesManage.Clone();
            this.owner = owner;
            this.parent = parent;

            InitEdge();
            InitNode();

            InitConnection();
        }

        private void InitNode()
        {
            int count = graphAsset.nodes.Count;
            for (int i = 0; i < count; i++)
            {
                FlowNodeAsset nodeAsset = graphAsset.nodes[i];
                AddNode(nodeAsset);
            }
        }

        private void AddNode(FlowNodeAsset nodeAsset)
        {
            FlowNode node = nodeAsset.CreateNode();
            node.OnInit(nodeAsset, this);
            this._nodes.Add(node);
            this._nodeById.Add(nodeAsset.id, node);
        }

        private void InitEdge()
        {
            int count = graphAsset.edges.Count;
            for (int i = 0; i < count; i++)
            {
                FlowEdgeAsset edgeAsset = graphAsset.edges[i];
                AddEdge(edgeAsset);
            }
        }

        private void AddEdge(FlowEdgeAsset edgeAsset)
        {
            FlowEdge edge = ReferencePool.Acquire<FlowEdge>();
            edge.OnInit(edgeAsset, this);
            this._edges.Add(edge);
            this._edgeById.Add(edgeAsset.id, edge);
        }

        private void InitConnection()
        {
            int count = _edges.Count;
            for (int i = 0; i < count; i++)
            {
                FlowEdge edge = _edges[i];
                edge.OnInitConnection();
            }
        }

        public void Start()
        {
            isActive = true;
            onStart?.Invoke();
        }

        public void Tick()
        {
            onTick?.Invoke();
        }

        public void Dispose()
        {
            isActive = false;
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            int nodesCount = this._nodes.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                FlowNode node = this._nodes[i];
                node.Dispose();
            }

            int edgesCount = this._edges.Count;
            for (int i = 0; i < edgesCount; i++)
            {
                FlowEdge edge = this._edges[i];
                edge.OnDispose();
            }

            uid = -1;

            onStart = null;
            onTick = null;

            this._nodes.Clear();
            this._edges.Clear();

            this._nodeById.Clear();
            this._edgeById.Clear();

            variablesManage.Clear();
        }
    }
}