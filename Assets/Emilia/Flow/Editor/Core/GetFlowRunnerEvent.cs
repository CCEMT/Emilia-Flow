using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    public class GetFlowRunnerEvent : EventBase<GetFlowRunnerEvent>
    {
        public EditorFlowRunner runner;

        protected override void Init()
        {
            base.Init();
            this.runner = null;
        }
    }
}