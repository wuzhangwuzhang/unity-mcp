using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace PSDImporter
{
    public class PSDImporterWindow : EditorWindow
    {
        private PSDImporterConfig config;
        private Vector2 scrollPosition;
        private Font defaultFont;

        private bool showPathSettings = true;
        private bool showImportSettings = false;
        private bool showUISettings = false;

        [MenuItem("Tools/PSD Importer")]
        static void Init()
        {
            PSDImporterWindow window = (PSDImporterWindow)EditorWindow.GetWindow(typeof(PSDImporterWindow));
            window.titleContent = new GUIContent("PSD Importer");
            window.minSize = new Vector2(450, 600);
            window.Show();
        }

        void OnEnable()
        {
            config = new PSDImporterConfig();
            config.LoadConfig();
            if (!string.IsNullOrEmpty(config.DefaultFontPath))
                defaultFont = AssetDatabase.LoadAssetAtPath<Font>(config.DefaultFontPath);
        }

        void OnDisable()
        {
            config.SaveConfig();
        }

        void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("PSD Importer", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 主要操作按钮
            GUI.enabled = !string.IsNullOrEmpty(config.JsonPath) && !string.IsNullOrEmpty(config.ImageFolderPath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("1. 导入资源", GUILayout.Height(30)))
            {
                ImportResources();
            }
            if (GUILayout.Button("2. 生成UGUI Prefab", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(config.JsonPath) || string.IsNullOrEmpty(config.ImageFolderPath))
                {
                    EditorUtility.DisplayDialog("错误", "请先选择JSON文件和图片文件夹", "确定");
                    return;
                }
                GeneratePrefab();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("3. 生成图集", GUILayout.Height(30)))
            {
                string prefabPath = Path.Combine(config.PrefabSavePath, config.FunctionName, "UIPrefab",
                    $"{config.CustomPrefabName}.prefab").Replace('\\', '/');
                if (!File.Exists(prefabPath))
                {
                    EditorUtility.DisplayDialog("错误", "请先生成预制体!", "确定");
                    return;
                }

                if (EditorUtility.DisplayDialog("确认",
                    "此操作将:\n1. 将小于512x512的图片打包成图集\n2. 更新预制体中的图片引用\n3. 删除已打包的散图\n是否继续?",
                    "确定", "取消"))
                {
                    GenerateTextureAtlas();
                }
            }

            if (GUILayout.Button("4. 更新图集", GUILayout.Height(30)))
            {
                string atlasDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "UIAtlas").Replace('\\', '/');
                string atlasName = $"{config.CustomPrefabName}_Atlas.png";
                string atlasPath = Path.Combine(atlasDir, atlasName).Replace('\\', '/');
                if (!File.Exists(atlasPath))
                {
                    EditorUtility.DisplayDialog("错误", "找不到现有图集，请先生成图集!", "确定");
                    return;
                }

                if (EditorUtility.DisplayDialog("确认",
                    "此操作将:\n1. 更新现有图集\n2. 保持预制体中的图片引用\n是否继续?",
                    "确定", "取消"))
                {
                    UpdateTextureAtlas();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            GUI.enabled = true;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            showPathSettings = EditorGUILayout.Foldout(showPathSettings, "路径设置", true);
            if (showPathSettings)
            {
                EditorGUI.indentLevel++;
                DrawPathSettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            showImportSettings = EditorGUILayout.Foldout(showImportSettings, "导入设置", true);
            if (showImportSettings)
            {
                EditorGUI.indentLevel++;
                DrawImportSettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            showUISettings = EditorGUILayout.Foldout(showUISettings, "UI组件设置", true);
            if (showUISettings)
            {
                EditorGUI.indentLevel++;
                DrawUIComponentSettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawPathSettings()
        {
            EditorGUILayout.BeginVertical();

            // JSON文件路径
            EditorGUILayout.BeginHorizontal();
            config.JsonPath = EditorGUILayout.TextField("JSON文件路径", config.JsonPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFilePanel("选择JSON文件", "", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    config.JsonPath = path;
                    config.ImageFolderPath = Path.Combine(Path.GetDirectoryName(path), "images");
                    if (string.IsNullOrEmpty(config.CustomPrefabName))
                        config.CustomPrefabName = Path.GetFileNameWithoutExtension(path);
                    if (string.IsNullOrEmpty(config.FunctionName))
                        config.FunctionName = Path.GetFileName(Path.GetDirectoryName(path));
                }
            }
            EditorGUILayout.EndHorizontal();

            // 图片文件夹路径
            EditorGUILayout.BeginHorizontal();
            config.ImageFolderPath = EditorGUILayout.TextField("图片文件夹", config.ImageFolderPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择图片文件夹", "", "");
                if (!string.IsNullOrEmpty(path))
                    config.ImageFolderPath = path;
            }
            EditorGUILayout.EndHorizontal();

            // 功能名称
            EditorGUILayout.BeginHorizontal();
            config.FunctionName = EditorGUILayout.TextField("功能名称", config.FunctionName);
            if (GUILayout.Button("重置", GUILayout.Width(60)))
                config.FunctionName = Path.GetFileName(Path.GetDirectoryName(config.JsonPath));
            EditorGUILayout.EndHorizontal();

            // Prefab名称
            EditorGUILayout.BeginHorizontal();
            config.CustomPrefabName = EditorGUILayout.TextField("Prefab名称", config.CustomPrefabName);
            if (GUILayout.Button("重置", GUILayout.Width(60)))
                config.CustomPrefabName = Path.GetFileNameWithoutExtension(config.JsonPath);
            EditorGUILayout.EndHorizontal();

            // Prefab保存路径
            EditorGUILayout.BeginHorizontal();
            config.PrefabSavePath = EditorGUILayout.TextField("Prefab保存路径", config.PrefabSavePath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择Prefab保存目录", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                    config.PrefabSavePath = path.Substring(path.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawImportSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("导入设置", EditorStyles.boldLabel);

            config.UseCompression = EditorGUILayout.Toggle("使用压缩", config.UseCompression);

            GUI.enabled = config.UseCompression;
            config.MaxTextureSize = EditorGUILayout.IntPopup("最大纹理尺寸", config.MaxTextureSize,
                new string[] { "256", "512", "1024", "2048", "4096" },
                new int[] { 256, 512, 1024, 2048, 4096 });

            config.TextureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("纹理格式", config.TextureFormat);
            config.CompressionQuality = EditorGUILayout.IntSlider("压缩质量", config.CompressionQuality, 0, 100);
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private void DrawUIComponentSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("UI组件设置", EditorStyles.boldLabel);

            // 字体选择
            EditorGUILayout.BeginHorizontal();
            Font newFont = (Font)EditorGUILayout.ObjectField("默认字体", defaultFont, typeof(Font), false);
            if (newFont != defaultFont)
            {
                defaultFont = newFont;
                config.DefaultFontPath = defaultFont != null ? AssetDatabase.GetAssetPath(defaultFont) : "";
            }
            EditorGUILayout.EndHorizontal();

            // 设计稿尺寸（只读）
            EditorGUILayout.LabelField("设计稿尺寸 (来自JSON)", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.FloatField("设计宽度", config.DesignWidth);
            EditorGUILayout.FloatField("设计高度", config.DesignHeight);
            GUI.enabled = true;

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("偏移设置", EditorStyles.boldLabel);
            config.OffsetX = EditorGUILayout.FloatField("X偏移", config.OffsetX);
            config.OffsetY = EditorGUILayout.FloatField("Y偏移", config.OffsetY);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("缩放设置", EditorStyles.boldLabel);
            config.UserScaleRatio = EditorGUILayout.Slider("缩放比例", config.UserScaleRatio, 0.1f, 2f);

            EditorGUILayout.Space(5);

            config.GenerateLayoutGroup = EditorGUILayout.Toggle("生成Layout Groups", config.GenerateLayoutGroup);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("按钮自动生成", EditorStyles.boldLabel);
            config.AutoGenerateButton = EditorGUILayout.Toggle("自动生成按钮", config.AutoGenerateButton);

            if (config.AutoGenerateButton)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("输入按钮关键字，用逗号分隔 (如: btn,button)", MessageType.Info);
                config.ButtonNameRules = EditorGUILayout.TextField("按钮关键字", config.ButtonNameRules);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void ImportResources()
        {
            PSDImporterProcessor processor = new PSDImporterProcessor(config);
            processor.ImportResources();
        }

        private void GeneratePrefab()
        {
            string unityJsonPath = Path.Combine(config.PrefabSavePath, config.FunctionName, "Json",
                Path.GetFileName(config.JsonPath)).Replace('\\', '/');
            if (!File.Exists(unityJsonPath))
            {
                EditorUtility.DisplayDialog("错误", "请先导入资源!", "确定");
                return;
            }

            string jsonContent = File.ReadAllText(unityJsonPath);
            JObject jsonData = JObject.Parse(jsonContent);

            PSDPrefabGenerator generator = new PSDPrefabGenerator(config, jsonData);
            generator.GeneratePrefab();
        }

        private void GenerateTextureAtlas()
        {
            string atlasDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "UIAtlas").Replace('\\', '/');
            string singleImageDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "UITexture").Replace('\\', '/');
            string textureDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices").Replace('\\', '/');

            if (!Directory.Exists(textureDir))
            {
                EditorUtility.DisplayDialog("错误", "找不到切图文件夹，请先导入资源!", "确定");
                return;
            }

            PSDImporterUtils.EnsureDirectoryExists(atlasDir);
            PSDImporterUtils.EnsureDirectoryExists(singleImageDir);

            List<string> allImageFiles = new List<string>();
            string[] files = Directory.GetFiles(textureDir, "*.png");
            foreach (string file in files)
                allImageFiles.Add(file.Replace('\\', '/'));

            List<string> atlasSprites = new List<string>();
            Dictionary<string, Vector4> slicedSprites = new Dictionary<string, Vector4>();
            Dictionary<string, string> assetRedirectMap = new Dictionary<string, string>();

            foreach (string imagePath in allImageFiles)
            {
                string assetPath = PSDImporterUtils.GetAssetPath(imagePath);
                if (string.IsNullOrEmpty(assetPath)) continue;

                Vector2 textureSize = PSDImporterUtils.GetTextureSize(assetPath);
                if (textureSize == Vector2.zero) continue;

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    Vector4 border = importer.spriteBorder;
                    if (border != Vector4.zero)
                    {
                        string spriteName = Path.GetFileNameWithoutExtension(assetPath);
                        slicedSprites[spriteName] = border;
                    }
                }

                if (textureSize.x <= 512 && textureSize.y <= 512)
                {
                    atlasSprites.Add(assetPath);
                }
                else
                {
                    string fileName = Path.GetFileName(assetPath);
                    string destPath = Path.Combine(singleImageDir, fileName).Replace('\\', '/');

                    string error = AssetDatabase.MoveAsset(assetPath, destPath);
                    if (!string.IsNullOrEmpty(error))
                    {
                        if (File.Exists(destPath))
                            assetRedirectMap[assetPath] = destPath;
                        else
                            Debug.LogError($"移动资源失败: {assetPath} -> {destPath}\n错误: {error}");
                    }
                }
            }

            if (atlasSprites.Count > 0)
            {
                TexturePackerConfig tpConfig = new TexturePackerConfig
                {
                    MaxSize = 2048,
                    Padding = 2,
                    AllowRotation = false,
                    TrimMode = TexturePackerTrimMode.None,
                    FastAndLooseMethod = true,
                    SlicedSprites = slicedSprites
                };

                string atlasName = $"{config.CustomPrefabName}_Atlas";
                string outputPath = Path.Combine(atlasDir, atlasName).Replace('\\', '/');

                var (success, outputPaths, spriteDataMap) = TexturePacker.Pack(atlasSprites.ToArray(), outputPath, tpConfig);

                if (success)
                {
                    string prefabPath = Path.Combine(config.PrefabSavePath, config.FunctionName, "UIPrefab",
                        $"{config.CustomPrefabName}.prefab").Replace('\\', '/');
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    if (prefab != null)
                    {
                        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                        bool needUpdate = false;

                        foreach (Image image in prefabInstance.GetComponentsInChildren<Image>(true))
                        {
                            if (image.sprite != null)
                            {
                                string spritePath = AssetDatabase.GetAssetPath(image.sprite);
                                string spriteName = Path.GetFileNameWithoutExtension(spritePath);

                                if (assetRedirectMap.ContainsKey(spritePath))
                                {
                                    string redirectPath = assetRedirectMap[spritePath];
                                    Sprite redirectSprite = AssetDatabase.LoadAssetAtPath<Sprite>(redirectPath);
                                    if (redirectSprite != null)
                                    {
                                        image.sprite = redirectSprite;
                                        needUpdate = true;
                                    }
                                }
                                else if (atlasSprites.Contains(spritePath))
                                {
                                    foreach (string atlasPath in outputPaths)
                                    {
                                        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(atlasPath)
                                            .OfType<Sprite>().ToArray();

                                        Sprite newSprite = sprites.FirstOrDefault(s => s.name == spriteName);
                                        if (newSprite != null)
                                        {
                                            image.sprite = newSprite;
                                            if (slicedSprites.ContainsKey(spriteName))
                                                image.type = Image.Type.Sliced;
                                            needUpdate = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (needUpdate)
                        {
                            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);

                            foreach (string spritePath in atlasSprites)
                                AssetDatabase.DeleteAsset(spritePath);

                            // 清理临时目录
                            string jsonDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "Json").Replace('\\', '/');
                            string slicesDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices").Replace('\\', '/');

                            if (Directory.Exists(jsonDir) && jsonDir.StartsWith("Assets"))
                                AssetDatabase.DeleteAsset(jsonDir);
                            if (Directory.Exists(slicesDir) && slicesDir.StartsWith("Assets"))
                                AssetDatabase.DeleteAsset(slicesDir);
                        }

                        UnityEngine.Object.DestroyImmediate(prefabInstance);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private void UpdateTextureAtlas()
        {
            try
            {
                string slicesPath = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices").Replace('\\', '/');
                if (!Directory.Exists(slicesPath))
                {
                    EditorUtility.DisplayDialog("错误", "找不到切图文件夹，请先导入资源!", "确定");
                    return;
                }

                Dictionary<string, Vector4> slicedSprites = new Dictionary<string, Vector4>();
                List<string> spritePaths = new List<string>();

                string[] allFiles = Directory.GetFiles(slicesPath, "*.png");
                foreach (string path in allFiles)
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(PSDImporterUtils.GetAssetPath(path));
                    if (texture != null && texture.width <= 512 && texture.height <= 512)
                    {
                        spritePaths.Add(path);

                        TextureImporter importer = AssetImporter.GetAtPath(PSDImporterUtils.GetAssetPath(path)) as TextureImporter;
                        if (importer != null)
                        {
                            Vector4 border = importer.spriteBorder;
                            if (border != Vector4.zero)
                            {
                                string spriteName = Path.GetFileNameWithoutExtension(path);
                                slicedSprites[spriteName] = border;
                            }
                        }
                    }
                }

                if (spritePaths.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示", "没有找到需要更新的图片!", "确定");
                    return;
                }

                string atlasDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "UIAtlas").Replace('\\', '/');
                string atlasName = $"{config.CustomPrefabName}_Atlas.png";
                string atlasPath = Path.Combine(atlasDir, atlasName).Replace('\\', '/');

                var texturePackerConfig = new TexturePackerConfig
                {
                    MaxSize = 2048,
                    Padding = 2,
                    AllowRotation = false,
                    TrimMode = TexturePackerTrimMode.None,
                    FastAndLooseMethod = true,
                    SlicedSprites = slicedSprites
                };

                bool success = TexturePacker.UpdateAtlas(spritePaths.ToArray(), PSDImporterUtils.GetAssetPath(atlasPath), texturePackerConfig);
                if (success)
                {
                    EditorUtility.DisplayDialog("成功", "图集更新完成!", "确定");

                    string jsonDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "Json").Replace('\\', '/');
                    string slicesDir = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices").Replace('\\', '/');

                    if (Directory.Exists(jsonDir) && jsonDir.StartsWith("Assets"))
                        AssetDatabase.DeleteAsset(jsonDir);
                    if (Directory.Exists(slicesDir) && slicesDir.StartsWith("Assets"))
                        AssetDatabase.DeleteAsset(slicesDir);
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "图集更新失败，请查看控制台日志!", "确定");
                }

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"更新图集出错: {e.Message}\n{e.StackTrace}");
                EditorUtility.DisplayDialog("错误", $"更新图集出错: {e.Message}", "确定");
            }
        }
    }
}
