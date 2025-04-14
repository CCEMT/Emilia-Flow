using System.Linq;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    public class FlowHotKeysHandle : GraphHotKeysHandle<EditorFlowAsset>
    {
        public override void OnKeyDown(KeyDownEvent evt)
        {
            base.OnKeyDown(evt);
            if (evt.ctrlKey && evt.keyCode == KeyCode.Q)
            {
                const float Interval = 50;
                
                GraphLayoutUtility.AlignmentType alignmentType = GraphLayoutUtility.AlignmentType.Horizontal | GraphLayoutUtility.AlignmentType.TopOrLeft;
                GraphLayoutUtility.Start(Interval, alignmentType, smartValue.graphSelected.selected.OfType<IEditorNodeView>().ToList());
            }
        }
    }
}