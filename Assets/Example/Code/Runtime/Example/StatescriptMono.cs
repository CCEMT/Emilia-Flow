using Emilia.Flow;
using UnityEngine;

namespace Emilia.Statescript
{
    public class StatescriptMono : MonoBehaviour
    {
        public const string EditorPath = "Assets/Example/Resource/Asset";

        public string fileName;

        private IFlowRunner runner;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(fileName)) return;

            FlowLoader flowLoader = new();
            flowLoader.editorFilePath = EditorPath;

            runner = FlowRunnerUtility.CreateRunner();
            runner.Init(this.fileName, flowLoader, gameObject);
            this.runner.Start();
        }

        private void Update()
        {
            this.runner?.Update();
        }

        private void OnDisable()
        {
            this.runner?.Dispose();
            runner = null;
        }
    }
}