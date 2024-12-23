namespace Emilia.Flow
{
    public interface IFlowRunner
    {
        int uid { get; }

        FlowGraphAsset asset { get; }

        FlowGraph graph { get; }

        bool isActive { get; }

        void Init(string fileName, IFlowLoader loader, object owner = null);

        void Start();

        void Update();

        void Dispose();
    }
}