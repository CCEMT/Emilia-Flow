using System;
using System.IO;
using System.Threading.Tasks;
using Emilia.DataBuildPipeline.Editor;
using Emilia.Kit;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Emilia.Flow.Editor
{
    [BuildPipeline(typeof(FlowBuildArgs)), BuildSequence(2000)]
    public class FlowOutputFile : IDataOutput
    {
        public void Output(IBuildContainer buildContainer, IBuildArgs buildArgs, Action onFinished)
        {
            FlowBuildContainer container = buildContainer as FlowBuildContainer;
            FlowBuildArgs args = buildArgs as FlowBuildArgs;

            if (string.IsNullOrEmpty(args.outputPath))
            {
                onFinished.Invoke();
                return;
            }

            string dataPathNoAssets = Directory.GetParent(Application.dataPath).ToString();
            string path = $"{dataPathNoAssets}/{args.outputPath}/{container.editorFlowAsset.name}.bytes";

            Task.Run(() => {
                if (File.Exists(path)) File.Delete(path);
                byte[] bytes = TagSerializationUtility.IgnoreTagSerializeValue(container.flowGraphAsset, DataFormat.Binary, SerializeTagDefine.DefaultIgnoreTag);
                File.WriteAllBytes(path, bytes);
                EditorKit.UnityInvoke(RefreshAssetDatabase);
            });

            onFinished.Invoke();

            void RefreshAssetDatabase()
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }
    }
}