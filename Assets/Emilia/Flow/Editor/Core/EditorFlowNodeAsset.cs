using System;
using System.Collections.Generic;
using System.Reflection;
using Emilia.Flow.Attributes;
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

        private IFlowNodeDescription flowNodeDescription;
        private Label descriptionLabel;

        private bool _editInNode;
        protected override bool editInNode => _editInNode;

        public override bool canExpanded => false;

        public object openScriptObject => flowNodeAsset.userData;

        public override void Initialize(EditorGraphView graphView, EditorNodeAsset asset)
        {
            flowNodeAsset = asset as EditorFlowNodeAsset;
            editorFlowAsset = graphView.graphAsset as EditorFlowAsset;

            _editInNode = editorFlowAsset.capacity.HasFlag(FlowCapacity.EditToNode);

            base.Initialize(graphView, asset);
        }

        protected override void InitializeNodeView()
        {
            base.InitializeNodeView();
            AddDescription();
            SetNodeColor(flowNodeAsset.userData);
        }

        private void AddDescription()
        {
            flowNodeDescription = flowNodeAsset.userData as IFlowNodeDescription;
            if (flowNodeDescription == null) return;

            descriptionLabel = new Label(flowNodeDescription.description);
            descriptionLabel.enableRichText = true;
            descriptionLabel.pickingMode = PickingMode.Ignore;
            this.descriptionLabel.style.position = Position.Absolute;
            topLayerContainer.Add(descriptionLabel);
        }

        public override void OnValueChanged()
        {
            base.OnValueChanged();
            if (descriptionLabel != null) descriptionLabel.text = flowNodeDescription.description;
        }

        public override List<EditorPortInfo> CollectStaticPortAssets()
        {
            List<EditorPortInfo> portAssets = new List<EditorPortInfo>();

            IUniversalFlowNodeAsset universalFlowNodeAsset = flowNodeAsset.userData as IUniversalFlowNodeAsset;
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

                portAssets.Add(editorPortInfo);
            }

            Type nodeType = universalFlowNodeAsset.nodeType;

            PropertyInfo[] propertyInfos = nodeType.GetProperties(bindingFlags);
            int propertyAmount = propertyInfos.Length;
            for (int i = 0; i < propertyAmount; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
                if (getMethodInfo == null) continue;

                FlowOutputValuePort flowOutputVarPort = propertyInfo.GetCustomAttribute<FlowOutputValuePort>(true);
                if (flowOutputVarPort == null) continue;

                EditorPortInfo editorPortInfo = ToEditorPortInfo(propertyInfo.Name, EditorPortDirection.Output, propertyInfo.PropertyType, flowOutputVarPort);
                editorPortInfo.order = -1 + i * 0.001f;

                FlowPortOrderAttribute flowPortOrderAttribute = propertyInfo.GetCustomAttribute<FlowPortOrderAttribute>(true);
                if (flowPortOrderAttribute != null) editorPortInfo.order = flowPortOrderAttribute.order;

                FlowPortColorAttribute flowPortColorAttribute = propertyInfo.GetCustomAttribute<FlowPortColorAttribute>(true);
                if (flowPortColorAttribute != null) editorPortInfo.color = new Color(flowPortColorAttribute.r, flowPortColorAttribute.g, flowPortColorAttribute.b);

                portAssets.Add(editorPortInfo);
            }

            MethodInfo[] methods = nodeType.GetMethods(bindingFlags);
            int methodAmount = methods.Length;
            for (int i = 0; i < methodAmount; i++)
            {
                MethodInfo methodInfo = methods[i];

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

                EditorPortInfo editorPortInfo = ToEditorPortInfo(methodInfo.Name, isInputOrOutput.Value, null, flowPortGenerator);
                editorPortInfo.order = i * 0.001f;

                FlowPortOrderAttribute flowPortOrderAttribute = methodInfo.GetCustomAttribute<FlowPortOrderAttribute>(true);
                if (flowPortOrderAttribute != null) editorPortInfo.order = flowPortOrderAttribute.order;

                FlowPortColorAttribute flowPortColorAttribute = methodInfo.GetCustomAttribute<FlowPortColorAttribute>(true);
                if (flowPortColorAttribute != null) editorPortInfo.color = new Color(flowPortColorAttribute.r, flowPortColorAttribute.g, flowPortColorAttribute.b);

                portAssets.Add(editorPortInfo);
            }

            return portAssets;
        }

        private EditorPortInfo ToEditorPortInfo(string id, EditorPortDirection direction, Type portType, FlowPortGenerator flowPortGenerator)
        {
            EditorPortInfo editorPortInfo = new EditorPortInfo();
            editorPortInfo.id = id;
            editorPortInfo.displayName = flowPortGenerator.displayName;
            editorPortInfo.direction = direction;
            editorPortInfo.orientation = flowPortGenerator.capacity.HasFlag(FlowPortCapacity.Vertical) ? EditorOrientation.Vertical : EditorOrientation.Horizontal;
            editorPortInfo.portType = portType;
            editorPortInfo.canMultiConnect = flowPortGenerator.capacity.HasFlag(FlowPortCapacity.MultiConnect);
            return editorPortInfo;
        }
    }
}