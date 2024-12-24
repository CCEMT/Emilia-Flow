using System;
using System.Collections.Generic;
using Emilia.Node.Universal.Editor;
using Emilia.Variables;
using Sirenix.OdinInspector;

namespace Emilia.Flow.Editor
{
    [Serializable]
    public class FlowRuntimeParameter
    {
        private EditorFlowRunner runner;

        private Dictionary<string, Variable> _runtimeUserVariables = new Dictionary<string, Variable>();

        [LabelText("参数"), ShowInInspector]
        public Dictionary<string, Variable> runtimeUserVariables
        {
            get
            {
                _runtimeUserVariables.Clear();
                if (this.runner == null) return this._runtimeUserVariables;

                foreach (var variablePair in this.runner.graph.variablesManage.variableMap)
                {
                    EditorParameter editorParameter = this.runner.editorFlowAsset.editorParametersManage.parameters.Find((x) => x.key == variablePair.Key);
                    if (editorParameter == null) continue;
                    _runtimeUserVariables[editorParameter.description] = variablePair.Value;
                }

                return this._runtimeUserVariables;
            }

            set { }
        }

        public FlowRuntimeParameter(EditorFlowRunner runner)
        {
            this.runner = runner;
        }
    }
}