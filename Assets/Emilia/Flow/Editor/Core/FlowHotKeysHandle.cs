using System.Linq;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    [EditorHandle(typeof(EditorFlowAsset))]
    public class FlowHotKeysHandle : UniversalGraphHotKeysHandle
    {
        public override void OnGraphKeyDown(EditorGraphView graphView, KeyDownEvent evt)
        {
            base.OnGraphKeyDown(graphView, evt);
            if (evt.ctrlKey && evt.keyCode == KeyCode.Q)
            {
                const float Interval = 50;

                GraphLayoutUtility.AlignmentType alignmentType = GraphLayoutUtility.AlignmentType.Horizontal | GraphLayoutUtility.AlignmentType.TopOrLeft;
                GraphLayoutUtility.Start(Interval, alignmentType, graphView.graphSelected.selected.OfType<IEditorNodeView>().ToList());
            }
        }
    }
}