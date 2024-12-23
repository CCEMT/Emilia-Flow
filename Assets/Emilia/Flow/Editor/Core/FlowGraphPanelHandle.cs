using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.Flow.Editor
{
    public class FlowGraphPanelHandle : GraphPanelHandle<EditorFlowAsset>
    {
        public override void LoadPanel(GraphPanelSystem system)
        {
            system.OpenDockPanel<FlowToolbarView>(20, GraphDockPosition.Top);
        }
    }
}