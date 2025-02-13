using System.Collections.Generic;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Emilia.Node.Attributes;
using Emilia.Node.Editor;
using Emilia.Node.Universal.Editor;
using Emilia.Reflection.Editor;
using UnityEditor;
using UnityEngine;

namespace Emilia.Flow.Editor
{
    public class FlowToolbarView : ToolbarView
    {
        protected override void InitControls()
        {
            AddControl(new ButtonToolbarViewControl("参数", OnEditorParameter));

            if (EditorApplication.isPlaying)
            {
                AddControl(new ButtonToolbarViewControl("运行参数", OnEditorRuntimeParameter));

                AddControl(new DropdownButtonToolbarViewControl("运行实例", BuildRunnerMenu));
            }

            AddControl(new ButtonToolbarViewControl("保存", OnSave), ToolbarViewControlPosition.RightOrBottom);
        }

        private void OnEditorParameter()
        {
            EditorFlowAsset flowAsset = graphView.graphAsset as EditorFlowAsset;
            EditorParametersManage editorParametersManage = flowAsset.editorParametersManage;
            if (editorParametersManage == null)
            {
                editorParametersManage = flowAsset.editorParametersManage = ScriptableObject.CreateInstance<EditorParametersManage>();
                EditorAssetKit.SaveAssetIntoObject(editorParametersManage, flowAsset);
            }

            Selection.activeObject = editorParametersManage;
        }

        private void OnEditorRuntimeParameter()
        {
            GetFlowRunnerEvent getFlowRunnerEvent = GetFlowRunnerEvent.GetPooled();
            getFlowRunnerEvent.target = graphView;

            graphView.SendEvent_Internal(getFlowRunnerEvent, DispatchMode_Internals.Immediate);
            FlowRuntimeParameter flowRuntimeParameter = new FlowRuntimeParameter(getFlowRunnerEvent.runner);
            EditorKit.SetSelection(flowRuntimeParameter, "运行参数");
        }

        private OdinMenu BuildRunnerMenu()
        {
            EditorFlowAsset flowAsset = graphView.graphAsset as EditorFlowAsset;

            OdinMenu odinMenu = new OdinMenu();
            odinMenu.defaultWidth = 300;

            if (EditorFlowRunner.runnerByAssetId == null) return odinMenu;
            List<EditorFlowRunner> runners = EditorFlowRunner.runnerByAssetId.GetValueOrDefault(flowAsset.id);
            if (runners == null) return odinMenu;

            int count = runners.Count;
            for (int i = 0; i < count; i++)
            {
                EditorFlowRunner runner = runners[i];
                string itemName = runner.graph.owner.ToString();
                if (string.IsNullOrEmpty(runner.editorFlowAsset.description) == false) itemName = $"{runner.editorFlowAsset.description}({runner.editorFlowAsset.name})";
                odinMenu.AddItem(itemName, () => {
                    SetFlowRunnerEvent e = SetFlowRunnerEvent.Create(runner);
                    e.target = graphView;

                    graphView.SendEvent(e);
                });
            }

            return odinMenu;
        }

        private void OnSave()
        {
            EditorFlowAsset flowAsset = graphView.graphAsset as EditorFlowAsset;
            EditorFlowAsset rootFlowAsset = flowAsset.GetRootAsset() as EditorFlowAsset;

            EditorFlowUtility.DataBuild(rootFlowAsset, (report) => {
                if (report.result == BuildResult.Succeeded) graphView.window.ShowNotification(new GUIContent("保存成功"), 1.5f);
            });
        }
    }
}