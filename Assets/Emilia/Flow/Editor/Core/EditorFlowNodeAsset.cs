using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emilia.Flow.Attributes;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Emilia.Flow.Editor
{
    [HideMonoScript]
    public class EditorFlowNodeAsset : UniversalNodeAsset
    {
        [ShowInInspector, HideLabel, HideReferenceObjectPicker]
        public object displayData
        {
            get => userData;
            set => userData = value;
        }

        protected override string defaultDisplayName
        {
            get
            {
                if (userData == null) return base.defaultDisplayName;
                Type nodeAssetType = userData.GetType();
                FlowNodeMenuAttribute createNodeMenu = nodeAssetType.GetCustomAttribute<FlowNodeMenuAttribute>();
                if (createNodeMenu == null) return base.defaultDisplayName;
                OperateMenuUtility.PathToNameAndCategory(createNodeMenu.path, out string titleText, out string _);
                return titleText;
            }
        }
    }

    [EditorNode(typeof(EditorFlowNodeAsset))]
    public class EditorFlowNodeView : UniversalEditorNodeView, ISpecifyOpenScriptObject
    {
        protected EditorFlowNodeAsset flowNodeAsset;
        protected EditorFlowAsset editorFlowAsset;

        private Label descriptionLabel;

        private bool _editInNode;
        protected override bool editInNode => this._editInNode;

        public override bool canExpanded => false;

        public object openScriptObject => this.flowNodeAsset.userData;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            this.flowNodeAsset = asset as EditorFlowNodeAsset;
            this.editorFlowAsset = graphView.graphAsset as EditorFlowAsset;

            this._editInNode = this.editorFlowAsset.capacity.HasFlag(FlowCapacity.EditToNode);

            base.Initialize(graphView, asset);
        }

        protected override void InitializeNodeView()
        {
            base.InitializeNodeView();
            AddDescription();
            SetNodeColor(this.flowNodeAsset.userData);
        }

        private void AddDescription()
        {
            string description = ObjectDescriptionUtility.GetDescription(this.flowNodeAsset.userData, graphView);

            this.descriptionLabel = new Label(description);
            this.descriptionLabel.enableRichText = true;
            this.descriptionLabel.pickingMode = PickingMode.Ignore;
            this.descriptionLabel.style.position = Position.Absolute;
            this.descriptionLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            topLayerContainer.Add(this.descriptionLabel);
        }

        public override void OnValueChanged(bool isSilent = false)
        {
            base.OnValueChanged(isSilent);
            if (this.descriptionLabel != null) this.descriptionLabel.text = ObjectDescriptionUtility.GetDescription(this.flowNodeAsset.userData, graphView);
            RebuildPortView();
        }

        public override List<EditorPortInfo> CollectStaticPortAssets()
        {
            List<EditorPortInfo> portAssets = new();

            IUniversalFlowNodeAsset universalFlowNodeAsset = this.flowNodeAsset.userData as IUniversalFlowNodeAsset;
            if (universalFlowNodeAsset == default) return portAssets;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] fieldInfos = universalFlowNodeAsset.GetType().GetFields(bindingFlags);
            int fieldAmount = fieldInfos.Length;
            for (int i = 0; i < fieldAmount; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];

                FlowInputValuePort flowInputVarPort = fieldInfo.GetCustomAttribute<FlowInputValuePort>(true);
                if (flowInputVarPort == null) continue;

                EditorPortInfo editorPortInfo = ToEditorPortInfo(fieldInfo.Name, EditorPortDirection.Input, fieldInfo.FieldType, flowInputVarPort);
                editorPortInfo.order = -1 + i * 0.001f;

                FlowPortOrderAttribute flowPortOrderAttribute = fieldInfo.GetCustomAttribute<FlowPortOrderAttribute>(true);
                if (flowPortOrderAttribute != null) editorPortInfo.order = flowPortOrderAttribute.order;

                if (flowInputVarPort.capacity.HasFlag(FlowPortCapacity.Expand))
                {
                    bool forceImGUI = flowInputVarPort.capacity.HasFlag(FlowPortCapacity.ExpandInImGUI);
                    string path = $"{nameof(EditorFlowNodeAsset.displayData)}.{fieldInfo.Name}";
                    this.inputEditInfos.Add(fieldInfo.Name, new EditorNodeInputPortEditInfo(fieldInfo.Name, path, forceImGUI));
                }

                FlowPortColorAttribute flowPortColorAttribute = fieldInfo.GetCustomAttribute<FlowPortColorAttribute>(true);
                if (flowPortColorAttribute != null) editorPortInfo.color = new Color(flowPortColorAttribute.r, flowPortColorAttribute.g, flowPortColorAttribute.b);

                FlowInsertOrderAttribute flowInsertOrderAttribute = fieldInfo.GetCustomAttribute<FlowInsertOrderAttribute>(true);
                if (flowInsertOrderAttribute != null) editorPortInfo.priority = flowInsertOrderAttribute.insertOrder;

                portAssets.Add(editorPortInfo);
            }

            Type nodeType = universalFlowNodeAsset.nodeType;

            MethodInfo[] methods = nodeType.GetMethods(bindingFlags);
            int methodAmount = methods.Length;
            for (int i = 0; i < methodAmount; i++)
            {
                MethodInfo methodInfo = methods[i];

                FlowOutputValuePort flowOutputVarPort = methodInfo.GetCustomAttribute<FlowOutputValuePort>(true);
                if (flowOutputVarPort != null)
                {
                    EditorPortInfo valueEditorPortInfo = ToEditorPortInfo(methodInfo.Name, EditorPortDirection.Output, methodInfo.ReturnType, flowOutputVarPort);
                    valueEditorPortInfo.order = -1 + i * 0.001f;

                    FlowPortOrderAttribute valueFlowPortOrderAttribute = methodInfo.GetCustomAttribute<FlowPortOrderAttribute>(true);
                    if (valueFlowPortOrderAttribute != null) valueEditorPortInfo.order = valueFlowPortOrderAttribute.order;

                    FlowPortColorAttribute valueFlowPortColorAttribute = methodInfo.GetCustomAttribute<FlowPortColorAttribute>(true);
                    if (valueFlowPortColorAttribute != null) valueEditorPortInfo.color = new Color(valueFlowPortColorAttribute.r, valueFlowPortColorAttribute.g, valueFlowPortColorAttribute.b);

                    FlowInsertOrderAttribute valueFlowInsertOrderAttribute = methodInfo.GetCustomAttribute<FlowInsertOrderAttribute>(true);
                    if (valueFlowInsertOrderAttribute != null) valueEditorPortInfo.priority = valueFlowInsertOrderAttribute.insertOrder;

                    portAssets.Add(valueEditorPortInfo);
                    continue;
                }

                EditorPortDirection? isInputOrOutput = null;
                FlowPortGenerator flowPortGenerator = null;
                FlowInputMethodPort flowInputMethodPort = methodInfo.GetCustomAttribute<FlowInputMethodPort>(true);
                if (flowInputMethodPort != null)
                {
                    isInputOrOutput = EditorPortDirection.Input;
                    flowPortGenerator = flowInputMethodPort;
                }

                FlowOutputMethodPort flowOutputMethodPort = methodInfo.GetCustomAttribute<FlowOutputMethodPort>(true);
                if (flowOutputMethodPort != null)
                {
                    isInputOrOutput = EditorPortDirection.Output;
                    flowPortGenerator = flowOutputMethodPort;
                }

                if (flowPortGenerator == null) continue;

                ParameterInfo firstParameter = methodInfo.GetParameters().FirstOrDefault();

                EditorPortInfo editorPortInfo = ToEditorPortInfo(methodInfo.Name, isInputOrOutput.Value, firstParameter?.ParameterType, flowPortGenerator);
                editorPortInfo.order = i * 0.001f;

                FlowPortOrderAttribute flowPortOrderAttribute = methodInfo.GetCustomAttribute<FlowPortOrderAttribute>(true);
                if (flowPortOrderAttribute != null) editorPortInfo.order = flowPortOrderAttribute.order;

                FlowPortColorAttribute flowPortColorAttribute = methodInfo.GetCustomAttribute<FlowPortColorAttribute>(true);
                if (flowPortColorAttribute != null) editorPortInfo.color = new Color(flowPortColorAttribute.r, flowPortColorAttribute.g, flowPortColorAttribute.b);

                FlowInsertOrderAttribute flowInsertOrderAttribute = methodInfo.GetCustomAttribute<FlowInsertOrderAttribute>(true);
                if (flowInsertOrderAttribute != null) editorPortInfo.priority = flowInsertOrderAttribute.insertOrder;

                portAssets.Add(editorPortInfo);
            }

            List<string> portIds = portAssets.Select((x) => x.id).ToList();
            FlowShowOrHideUtility.FilterPort(universalFlowNodeAsset, portIds);
            portAssets = portAssets.Where((x) => portIds.Contains(x.id)).ToList();

            return portAssets;
        }

        private EditorPortInfo ToEditorPortInfo(string id, EditorPortDirection direction, Type portType, FlowPortGenerator flowPortGenerator)
        {
            EditorPortInfo editorPortInfo = new();
            editorPortInfo.id = id;
            editorPortInfo.nodePortViewType = typeof(FlowPortView);
            editorPortInfo.displayName = flowPortGenerator.displayName;
            editorPortInfo.direction = direction;
            editorPortInfo.orientation = flowPortGenerator.capacity.HasFlag(FlowPortCapacity.Vertical) ? EditorOrientation.Vertical : EditorOrientation.Horizontal;
            editorPortInfo.portType = portType;
            editorPortInfo.canMultiConnect = flowPortGenerator.capacity.HasFlag(FlowPortCapacity.MultiConnect);
            return editorPortInfo;
        }
    }
}