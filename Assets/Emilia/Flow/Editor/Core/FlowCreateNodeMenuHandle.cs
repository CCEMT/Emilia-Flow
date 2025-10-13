using System;
using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;

namespace Emilia.Flow.Editor
{
    [EditorHandle(typeof(EditorFlowAsset))]
    public class FlowCreateNodeMenuHandle : UniversalCreateNodeMenuHandle
    {
        private EditorFlowAsset flowAsset;

        public override void InitializeCache(EditorGraphView graphView, List<ICreateNodeHandle> createNodeHandles)
        {
            base.InitializeCache(graphView, createNodeHandles);
            flowAsset = graphView.graphAsset as EditorFlowAsset;
            FilterCreateNodeHandles(graphView,createNodeHandles);
        }

        private void FilterCreateNodeHandles(EditorGraphView graphView,List<ICreateNodeHandle> createNodeHandles)
        {
            int cacheAmount = createNodeHandles.Count;
            for (int i = cacheAmount - 1; i >= 0; i--)
            {
                ICreateNodeHandle createNodeHandle = createNodeHandles[i];

                object nodeData = createNodeHandle.nodeData;
                if (nodeData == null) continue;
                if (IsContain(nodeData.GetType())) continue;
                createNodeHandles.RemoveAt(i);
            }
        }

        private bool IsContain(Type nodeType)
        {
            int subNodeTypeCount = flowAsset.subNodeTypes.Length;
            for (int i = 0; i < subNodeTypeCount; i++)
            {
                Type subNodeType = flowAsset.subNodeTypes[i];
                if (subNodeType.IsAssignableFrom(nodeType)) return true;
            }

            return false;
        }
    }
}