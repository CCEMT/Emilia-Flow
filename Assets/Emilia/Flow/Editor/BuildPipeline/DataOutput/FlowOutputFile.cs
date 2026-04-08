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

            if (args.isGenerateFile == false || string.IsNullOrEmpty(args.outputPath))
            {
                onFinished.Invoke();
                return;
            }

            string dataPathNoAssets = Directory.GetParent(Application.dataPath).ToString();
            string path = $"{dataPathNoAssets}/{args.outputPath}/{args.flowAsset.name}.bytes";

            Task.Run(() => {

                try
                {
                    if (File.Exists(path)) File.Delete(path);
                    byte[] bytes = TagSerializationUtility.IgnoreTagSerializeValue(container.flowGraphAsset, DataFormat.Binary, SerializeTagDefine.DefaultIgnoreTag);
                    File.WriteAllBytes(path, bytes);
                    EditorApplication.delayCall += RefreshAssetDatabase;
                }
                catch (Exception e)
                {
                    EditorApplication.delayCall += () => Debug.LogError(e.ToUnityLogString());
                }

            });

            onFinished.Invoke();

            void RefreshAssetDatabase()
            {
                AssetDatabase.ImportAsset(path);
                args.generateFileCallback?.Invoke();
            }

        }
    }
}