using Emilia.Kit;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    [EditorHandle(typeof(EditorFlowAsset))]
    public class FlowGraphPanelHandle : GraphPanelHandle
    {
        public override void LoadPanel(EditorGraphView graphView, GraphPanelSystem system)
        {
            base.LoadPanel(graphView, system);
            system.OpenDockPanel<FlowToolbarView>(20, GraphDockPosition.Top);
        }
    }
}