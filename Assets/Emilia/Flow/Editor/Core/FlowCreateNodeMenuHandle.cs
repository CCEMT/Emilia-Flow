using System;
using Emilia.Node.Editor;

namespace Emilia.Flow.Editor
{
    public class FlowCreateNodeMenuHandle : CreateNodeMenuHandle<EditorFlowAsset>
    {
        private EditorFlowAsset flowAsset;

        public override void InitializeCache()
        {
            base.InitializeCache();
            flowAsset = smartValue.graphAsset as EditorFlowAsset;
            FilterCreateNodeHandles();
        }

        private void FilterCreateNodeHandles()
        {
            int cacheAmount = smartValue.createNodeMenu.createNodeHandleCacheList.Count;
            for (int i = cacheAmount - 1; i >= 0; i--)
            {
                ICreateNodeHandle createNodeHandle = smartValue.createNodeMenu.createNodeHandleCacheList[i];

                object nodeData = createNodeHandle.nodeData;
                if (nodeData == null) continue;
                if (IsContain(nodeData.GetType())) continue;
                smartValue.createNodeMenu.createNodeHandleCacheList.RemoveAt(i);
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