using System;
using Emilia.Flow.Attributes;

namespace Emilia.Statescript
{
    [FlowNodeMenu("入口/主入口"), Serializable]
    public class MainStatescriptEntryAsset : StatescriptEntryAsset<MainStatescriptEntry> { }

    public class MainStatescriptEntry : StatescriptEntry<MainStatescriptEntryAsset>
    {
        protected override void OnInit()
        {
            base.OnInit();
            graph.onStart += OnOutput;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            graph.onStart -= OnOutput;
        }
    }
}