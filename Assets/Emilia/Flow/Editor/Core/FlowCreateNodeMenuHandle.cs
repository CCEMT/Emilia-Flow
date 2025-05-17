using System;
using System.Collections.Generic;
using Emilia.Kit;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    [EditorHandle(typeof(EditorFlowAsset))]
    public class FlowCreateNodeMenuHandle : CreateNodeMenuHandle
    {
        private EditorFlowAsset flowAsset;

        public override void InitializeCache(EditorGraphView graphView, List<ICreateNodeHandle> createNodeHandles)
        {
            base.InitializeCache(graphView, createNodeHandles);
            flowAsset = graphView.graphAsset as EditorFlowAsset;
            FilterCreateNodeHandles(graphView);
        }

        private void FilterCreateNodeHandles(EditorGraphView graphView)
        {
            int cacheAmount = graphView.createNodeMenu.createNodeHandleCacheList.Count;
            for (int i = cacheAmount - 1; i >= 0; i--)
            {
                ICreateNodeHandle createNodeHandle = graphView.createNodeMenu.createNodeHandleCacheList[i];

                object nodeData = createNodeHandle.nodeData;
                if (nodeData == null) continue;
                if (IsContain(nodeData.GetType())) continue;
                graphView.createNodeMenu.createNodeHandleCacheList.RemoveAt(i);
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