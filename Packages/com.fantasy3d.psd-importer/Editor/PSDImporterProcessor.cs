using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PSDImporter
{
    public class PSDImporterProcessor
    {
        private PSDImporterConfig config;
        private JObject jsonData;

        public PSDImporterProcessor(PSDImporterConfig config)
        {
            this.config = config;
        }

        public bool ImportResources()
        {
            try
            {
                ValidatePrefabName();

                string functionFolderPath = GetFunctionFolderPath();
                string unityImagesPath = Path.Combine(functionFolderPath, "PSDSlices").Replace('\\', '/');
                string unityJsonPath = Path.Combine(functionFolderPath, "Json").Replace('\\', '/');

                PSDImporterUtils.EnsureDirectoryExists(config.PrefabSavePath);
                PSDImporterUtils.EnsureDirectoryExists(functionFolderPath);
                PSDImporterUtils.EnsureDirectoryExists(unityImagesPath);
                PSDImporterUtils.EnsureDirectoryExists(unityJsonPath);

                if (!ImportJsonFile(unityJsonPath))
                    return false;

                if (!ImportImageFiles(unityImagesPath))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"导入资源出错: {e.Message}\n{e.StackTrace}");
                EditorUtility.DisplayDialog("错误", $"导入资源失败: {e.Message}", "确定");
                return false;
            }
        }

        private void ValidatePrefabName()
        {
            if (string.IsNullOrEmpty(config.CustomPrefabName))
            {
                config.CustomPrefabName = Path.GetFileNameWithoutExtension(config.JsonPath);
            }
            else
            {
                config.CustomPrefabName = string.Join("_", config.CustomPrefabName.Split(Path.GetInvalidFileNameChars()));
            }
        }

        private string GetFunctionFolderPath()
        {
            string prefabBasePath = config.PrefabSavePath.TrimEnd('/');
            return Path.Combine(prefabBasePath, config.FunctionName).Replace('\\', '/');
        }

        private bool ImportJsonFile(string unityJsonPath)
        {
            string unityJsonFile = Path.Combine(unityJsonPath, Path.GetFileName(config.JsonPath));
            string normalizedJsonPath = config.JsonPath.Replace('\\', '/');

            try
            {
                File.Copy(normalizedJsonPath, unityJsonFile, true);
                string jsonContent = File.ReadAllText(unityJsonFile);
                jsonData = JObject.Parse(jsonContent);

                if (jsonData["CanvasSize"] != null)
                {
                    config.DesignWidth = jsonData["CanvasSize"]["Width"].ToObject<float>();
                    config.DesignHeight = jsonData["CanvasSize"]["Height"].ToObject<float>();
                    Debug.Log($"从JSON更新画布尺寸: {config.DesignWidth}x{config.DesignHeight}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"导入JSON文件出错: {e.Message}");
                return false;
            }
        }

        private bool ImportImageFiles(string unityImagesPath)
        {
            if (!Directory.Exists(config.ImageFolderPath))
                return false;

            string[] imageFiles = Directory.GetFiles(config.ImageFolderPath, "*.png");
            int totalImages = imageFiles.Length;
            int currentImage = 0;
            List<string> failedFiles = new List<string>();
            List<string> importedAssetPaths = new List<string>();

            foreach (string imageFile in imageFiles)
            {
                currentImage++;
                string fileName = Path.GetFileName(imageFile);
                string destPath = Path.Combine(unityImagesPath, fileName).Replace('\\', '/');
                string normalizedImageFile = imageFile.Replace('\\', '/');

                EditorUtility.DisplayProgressBar("复制图片",
                    $"正在复制 {fileName} ({currentImage}/{totalImages})",
                    (float)currentImage / totalImages);

                try
                {
                    File.Copy(normalizedImageFile, destPath, true);
                    string assetPath = PSDImporterUtils.GetAssetPath(destPath);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        importedAssetPaths.Add(assetPath);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"复制 {fileName} 出错: {e.Message}");
                    failedFiles.Add(fileName);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            int successCount = 0;
            currentImage = 0;

            foreach (string assetPath in importedAssetPaths)
            {
                currentImage++;
                EditorUtility.DisplayProgressBar("设置导入参数",
                    $"正在处理 {Path.GetFileName(assetPath)} ({currentImage}/{importedAssetPaths.Count})",
                    (float)currentImage / importedAssetPaths.Count);

                try
                {
                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null)
                    {
                        // 九宫格边框
                        if (jsonData != null)
                        {
                            string fileName = Path.GetFileName(assetPath);
                            var imageNode = ((JArray)jsonData["InfoList"])
                                ?.FirstOrDefault(n => Path.GetFileName(n["FilePath"].ToString()) == fileName) as JObject;

                            if (imageNode != null && imageNode["IsNineSlice"]?.ToObject<bool>() == true)
                            {
                                float left = imageNode["Left"].ToObject<float>();
                                float top = imageNode["Top"].ToObject<float>();
                                float right = imageNode["Right"].ToObject<float>();
                                float bottom = imageNode["Bottom"].ToObject<float>();
                                importer.spriteBorder = new Vector4(left, bottom, right, top);
                            }
                        }

                        PSDImporterUtils.SetTextureImportSettings(importer, config);
                        importer.SaveAndReimport();
                        successCount++;
                    }
                    else
                    {
                        failedFiles.Add(Path.GetFileName(assetPath));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"设置 {assetPath} 导入参数出错: {e.Message}");
                    failedFiles.Add(Path.GetFileName(assetPath));
                }
            }

            EditorUtility.ClearProgressBar();

            string resultMessage = $"导入完成:\n成功: {successCount}\n失败: {failedFiles.Count}";
            if (failedFiles.Count > 0)
            {
                resultMessage += "\n\n失败文件:\n" + string.Join("\n", failedFiles);
            }

            EditorUtility.DisplayDialog("导入结果", resultMessage, "确定");
            return failedFiles.Count == 0;
        }

        public JObject GetJsonData()
        {
            return jsonData;
        }
    }
}
