using System;
using System.Collections.Generic;
using Emilia.Variables;
using Emilia.Variables.Editor;
using Sirenix.OdinInspector;

namespace Emilia.Flow.Editor
{
    [Serializable]
    public class FlowRuntimeParameter
    {
        private EditorFlowRunner runner;
        private EditorFlowAsset editorFlowAsset;

        private Dictionary<string, Variable> _runtimeUserVariables = new();

        [LabelText("参数"), ShowInInspector]
        public Dictionary<string, Variable> runtimeUserVariables
        {
            get
            {
                _runtimeUserVariables.Clear();
                if (this.runner == null) return this._runtimeUserVariables;

                foreach (var variablePair in this.runner.graph.variablesManage.variablesManager.variableMap)
                {
                    EditorParameter editorParameter = editorFlowAsset.editorParametersManage.parameters.Find((x) => x.key == variablePair.Key);
                    if (editorParameter == null) continue;
                    _runtimeUserVariables[editorParameter.description] = variablePair.Value;
                }

                return this._runtimeUserVariables;
            }

            set { }
        }

        public FlowRuntimeParameter(EditorFlowRunner runner, EditorFlowAsset editorFlowAsset)
        {
            this.editorFlowAsset = editorFlowAsset;
            this.runner = runner;
        }
    }
}