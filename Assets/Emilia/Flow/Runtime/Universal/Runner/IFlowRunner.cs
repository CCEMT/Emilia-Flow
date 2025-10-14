namespace Emilia.Flow
{
    public interface IFlowRunner
    {
        int uid { get; }

        string fileName { get; }

        FlowGraphAsset asset { get; }

        FlowGraph graph { get; }

        bool isActive { get; }

        void Init(string fileName, IFlowLoader loader, object owner = null);

        void Init(FlowGraphAsset graphAsset, object owner = null);

        void Start();

        void Update();

        void Dispose();
    }
}