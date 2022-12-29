#if INCLUDE_POST_PROCESSING
using System.IO;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.PostProcessing;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors.PostProcessing
{
    [PostProcessEditor(typeof(Fog))]
    internal class FogEditor : PostProcessEffectEditor<Fog>
    {
        private SerializedParameterOverride _mode;
        private SerializedParameterOverride _color;
        private SerializedParameterOverride _param1;
        private SerializedParameterOverride _param2;
        private SerializedParameterOverride _shader;

        private int _fogMode = int.MinValue;

        public override void OnEnable()
        {
            _mode = FindParameterOverride(item => item.Mode);
            _color = FindParameterOverride(item => item.FogColor);
            _param1 = FindParameterOverride(item => item.Param1);
            _param2 = FindParameterOverride(item => item.Param2);
            _shader = FindParameterOverride(item => item.Shader);
        }

        public override void OnInspectorGUI()
        {
            PropertyField(_mode);
            HandleFogMode();

            if (!_mode.overrideState.boolValue)
                return;

            PropertyField(_color);

            if (_mode.value.intValue == (int)FogMode.Linear)
            {
                PropertyField(_param1, EditorGuiUtility.TempContent("Start"));
                PropertyField(_param2, EditorGuiUtility.TempContent("End"));
                _param2.value.floatValue = _param2.value.floatValue.ClampMin(_param1.value.floatValue);
            }
            else
            {
                PropertyField(_param1, EditorGuiUtility.TempContent("Density"));
                _param1.value.floatValue = _param1.value.floatValue.Clamp01();

                PropertyField(_param2, EditorGuiUtility.TempContent("Offset"));
                _param2.value.floatValue = _param2.value.floatValue.ClampMin(0f);
            }
        }

        private void HandleFogMode()
        {
            _shader.overrideState.boolValue = true;
            int mode = _mode.value.intValue;

            if (_fogMode != mode)
            {
                _fogMode = mode;

                string shaderName = Fog.GetFogShaderPath((FogMode)mode);

                Shader shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    _shader.value.objectReferenceValue = shader;
                    return;
                }

                CreateShaders();
                _shader.value.objectReferenceValue = Shader.Find(shaderName);
            }
        }

        private void CreateShaders()
        {
            const string extension = ".shader";
            string destFolder = $"{AssetDatabaseExt.ASSET_FOLDER}Shaders/{nameof(UnityUtility)}/{nameof(PostProcessing)}";
            Directory.CreateDirectory(destFolder);

            CreateAssetFromTemplate("610b257f6f84e644ab720a276f478350", destFolder, extension, "f7ed684984420634fbe1a903eb536700");
            CreateAssetFromTemplate("49adad62dedaf104f9ed5c71c1853bf5", destFolder, extension, "944841f2be5c3e14fa046b60baa9b66e");
            CreateAssetFromTemplate("38b8cc30e9d512b41b39c0e7b22e41f9", destFolder, extension, "0e185ad917445714696b3d339851a75c");

            AssetDatabase.Refresh();
        }

        private static void CreateAssetFromTemplate(string templateGuid, string destFolder, string assetExtension, string targetGuid)
        {
            string sourcePath = AssetDatabase.GUIDToAssetPath(templateGuid);
            string dest = $"{destFolder}/{Path.GetFileName(sourcePath)}{assetExtension}";
            File.Copy(sourcePath, dest);
            TemplatesUtility.CreateMetaFile(dest, false, targetGuid);
        }
    }
}
#endif
