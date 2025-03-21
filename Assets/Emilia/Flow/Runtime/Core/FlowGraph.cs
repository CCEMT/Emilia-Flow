using System;
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

        /// <summary>
        /// 所有Node
        /// </summary>
        public IReadOnlyList<FlowNode> nodes => this._nodes;

        /// <summary>
        /// 所有Edge
        /// </summary>
        public IReadOnlyList<FlowEdge> edges => this._edges;

        /// <summary>
        /// Node通过id索引`
        /// </summary>
        public IReadOnlyDictionary<int, FlowNode> nodeById => this._nodeById;

        /// <summary>
        /// Edge通过id索引
        /// </summary>
        public IReadOnlyDictionary<int, FlowEdge> edgeById => this._edgeById;

        /// <summary>
        /// 实例id
        /// </summary>
        public int uid { get; private set; }

        /// <summary>
        /// 资产
        /// </summary>
        public FlowGraphAsset graphAsset { get; private set; }

        /// <summary>
        /// 变量管理
        /// </summary>
        public VariablesManage variablesManage { get; private set; }

        /// <summary>
        /// 调用者
        /// </summary>
        public object owner { get; private set; }

        /// <summary>
        /// 父级
        /// </summary>
        public FlowGraph parent { get; private set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<FlowGraph> children { get; private set; } = new List<FlowGraph>();

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActive { get; private set; }

        /// <summary>
        /// 开始事件
        /// </summary>
        public event Action onStart;

        /// <summary>
        /// 轮询事件
        /// </summary>
        public event Action onTick;

        /// <summary>
        /// 初始化
        /// </summary>
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

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            isActive = true;
            onStart?.Invoke();
        }

        /// <summary>
        /// 轮询
        /// </summary>
        public void Tick()
        {
            onTick?.Invoke();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            isActive = false;
            ReferencePool.Release(this);
        }

        void IReference.Clear()
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
            graphAsset = null;
            owner = null;
            parent = null;
            children.Clear();

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