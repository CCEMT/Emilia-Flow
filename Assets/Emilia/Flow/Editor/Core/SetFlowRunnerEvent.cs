using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    public class SetFlowRunnerEvent : EventBase<SetFlowRunnerEvent>
    {
        public EditorFlowRunner runner;

        protected override void Init()
        {
            base.Init();
            this.runner = null;
        }

        public static SetFlowRunnerEvent Create(EditorFlowRunner runner)
        {
            SetFlowRunnerEvent e = GetPooled();
            e.runner = runner;
            return e;
        }
    }
}