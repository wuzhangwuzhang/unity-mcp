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
    public class PSDPrefabGenerator
    {
        private PSDImporterConfig config;
        private JObject jsonData;
        private Dictionary<string, GameObject> nodeDict;
        private Font defaultFont;

        public PSDPrefabGenerator(PSDImporterConfig config, JObject jsonData)
        {
            this.config = config;
            this.jsonData = jsonData;
            this.nodeDict = new Dictionary<string, GameObject>();

            // 加载字体：优先使用配置路径，否则使用Unity内置字体
            if (!string.IsNullOrEmpty(config.DefaultFontPath))
            {
                this.defaultFont = AssetDatabase.LoadAssetAtPath<Font>(config.DefaultFontPath);
            }
            if (this.defaultFont == null)
            {
                this.defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        public bool GeneratePrefab()
        {
            try
            {
                string prefabPath = GetPrefabPath();

                // 检查prefab是否已存在
                GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (existingPrefab != null)
                {
                    if (!HandleExistingPrefab(existingPrefab, prefabPath))
                        return false;
                }

                GameObject root = new GameObject(config.CustomPrefabName);
                SetupRootNode(root);
                nodeDict["PSDRoot"] = root;

                var nodeList = ((JArray)jsonData["InfoList"]).Select(n => (JObject)n).ToList();

                // 第一阶段：创建所有节点
                foreach (JObject node in nodeList)
                {
                    CreateNode(node);
                }

                // 第二阶段：设置所有节点的位置
                foreach (JObject node in nodeList)
                {
                    SetupNodeTransform(node);
                }

                // 保存Prefab
                Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                UnityEngine.Object.DestroyImmediate(root);

                if (prefab != null)
                {
                    EditorUtility.DisplayDialog("成功", $"UGUI Prefab '{config.CustomPrefabName}' 生成成功!", "确定");
                    Selection.activeObject = prefab;
                    return true;
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "创建Prefab失败!", "确定");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"生成Prefab出错: {e.Message}\n{e.StackTrace}");
                EditorUtility.DisplayDialog("错误", $"生成Prefab失败: {e.Message}", "确定");
                return false;
            }
        }

        private bool HandleExistingPrefab(GameObject existingPrefab, string prefabPath)
        {
            HashSet<string> resourcePaths = new HashSet<string>();
            bool hasAtlasReference = false;

            foreach (Image image in existingPrefab.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite != null && image.sprite.texture != null)
                {
                    string texturePath = AssetDatabase.GetAssetPath(image.sprite.texture);

                    if (texturePath.Contains("UIAtlas"))
                    {
                        hasAtlasReference = true;
                        resourcePaths.Add(texturePath);

                        string tpsheetPath = texturePath.Replace(".png", ".tpsheet");
                        if (File.Exists(tpsheetPath))
                            resourcePaths.Add(tpsheetPath);

                        string directory = Path.GetDirectoryName(texturePath);
                        string atlasName = Path.GetFileNameWithoutExtension(texturePath);
                        string[] allFiles = Directory.GetFiles(directory, $"{atlasName}_*.png");
                        foreach (string file in allFiles)
                            resourcePaths.Add(file.Replace("\\", "/"));
                    }
                    else
                    {
                        resourcePaths.Add(texturePath);
                    }
                }
            }

            if (resourcePaths.Count > 0 && hasAtlasReference)
            {
                bool shouldProceed = EditorUtility.DisplayDialog(
                    "警告",
                    $"Prefab '{config.CustomPrefabName}' 已存在并引用了图集资源。重新创建会删除相关资源文件，是否继续？",
                    "继续", "取消");

                if (!shouldProceed) return false;

                foreach (string path in resourcePaths)
                {
                    if (File.Exists(path))
                        AssetDatabase.DeleteAsset(path);
                }
                AssetDatabase.Refresh();
            }

            return true;
        }

        private void SetupRootNode(GameObject root)
        {
            RectTransform rootRectTransform = root.AddComponent<RectTransform>();
            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.offsetMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;

            root.AddComponent<CanvasRenderer>();
            root.layer = LayerMask.NameToLayer("UI");
        }

        private string GetPrefabPath()
        {
            return Path.Combine(config.PrefabSavePath, config.FunctionName, "UIPrefab",
                config.CustomPrefabName + ".prefab").Replace('\\', '/');
        }

        private void CreateNode(JObject node)
        {
            if (node == null) return;

            string name = node["Name"].ToString();
            string type = node["Type"].ToString();
            string tree = node["Tree"].ToString().Replace('\\', '/');

            if (!tree.StartsWith("PSDRoot/"))
                tree = "PSDRoot/" + tree;

            if (nodeDict.ContainsKey(tree)) return;

            try
            {
                string[] pathParts = tree.Split('/');
                string parentPath = string.Join("/", pathParts.Take(pathParts.Length - 1));

                if (!nodeDict.ContainsKey(parentPath))
                    CreateParentNode(parentPath);

                GameObject parentObj = nodeDict[parentPath];
                if (parentObj == null) return;

                GameObject obj = null;
                switch (type.ToLower())
                {
                    case "node":
                        obj = CreateEmptyNode(name);
                        break;
                    case "png":
                        obj = CreateImageNode(node, name);
                        break;
                    case "text":
                        obj = CreateTextNode(node, name);
                        break;
                }

                if (obj != null)
                {
                    obj.name = name;
                    obj.layer = LayerMask.NameToLayer("UI");
                    obj.transform.SetParent(parentObj.transform, false);
                    nodeDict[tree] = obj;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"创建节点 {name} 出错: {e.Message}");
            }
        }

        private GameObject CreateEmptyNode(string name)
        {
            GameObject obj = new GameObject(name);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private GameObject CreateImageNode(JObject node, string name)
        {
            GameObject obj = new GameObject(name);
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            Image image = obj.AddComponent<Image>();

            string fileName = Path.GetFileName(node["FilePath"].ToString());
            string spritePath = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices", fileName).Replace('\\', '/');

            JObject duplicateMap = jsonData["DuplicateMap"] as JObject;
            if (duplicateMap != null && duplicateMap[node["FilePath"].ToString()] != null)
            {
                fileName = Path.GetFileName(duplicateMap[node["FilePath"].ToString()].ToString());
                spritePath = Path.Combine(config.PrefabSavePath, config.FunctionName, "PSDSlices", fileName).Replace('\\', '/');
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                image.sprite = sprite;

                if (node["IsNineSlice"]?.ToObject<bool>() == true)
                {
                    image.type = Image.Type.Sliced;
                    image.fillCenter = true;
                    image.preserveAspect = false;
                }
                else
                {
                    image.preserveAspect = true;
                }

                // 自动按钮识别
                if (config.AutoGenerateButton && !string.IsNullOrEmpty(config.ButtonNameRules))
                {
                    string lowerName = name.ToLower();
                    string[] rules = config.ButtonNameRules.ToLower().Split(',');

                    bool shouldBeButton = rules.Any(rule =>
                        !string.IsNullOrEmpty(rule) && lowerName.Contains(rule.Trim()));

                    if (shouldBeButton)
                    {
                        Button button = obj.AddComponent<Button>();
                        button.targetGraphic = image;
                        button.transition = Selectable.Transition.ColorTint;

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.white;
                        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                        button.colors = colors;
                    }
                }
            }

            return obj;
        }

        private GameObject CreateTextNode(JObject node, string name)
        {
            GameObject obj = new GameObject(name);
            Text text = obj.AddComponent<Text>();
            ContentSizeFitter contentFitter = obj.AddComponent<ContentSizeFitter>();

            contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            text.text = node["Content"].ToString();
            text.fontSize = (int)(node["FontSize"].ToObject<float>() * config.UserScaleRatio);
            text.font = defaultFont;
            text.raycastTarget = false;

            JObject textColor = node["TextColor"].ToObject<JObject>();
            text.color = new Color(
                textColor["Red"].ToObject<float>() / 255f,
                textColor["Green"].ToObject<float>() / 255f,
                textColor["Blue"].ToObject<float>() / 255f,
                1f
            );

            string alignment = node["Alignment"].ToString();
            switch (alignment)
            {
                case "MiddleLeft":
                    text.alignment = TextAnchor.MiddleLeft;
                    break;
                case "MiddleCenter":
                case "MiddleJustify":
                    text.alignment = TextAnchor.MiddleCenter;
                    break;
                case "MiddleRight":
                    text.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    text.alignment = TextAnchor.MiddleLeft;
                    break;
            }

            return obj;
        }

        private void CreateParentNode(string path)
        {
            if (nodeDict.ContainsKey(path)) return;

            string[] pathParts = path.Split('/');
            string parentPath = string.Join("/", pathParts.Take(pathParts.Length - 1));
            string nodeName = pathParts[pathParts.Length - 1];

            GameObject parentObj = null;
            if (!nodeDict.TryGetValue(parentPath, out parentObj))
            {
                CreateParentNode(parentPath);
                parentObj = nodeDict[parentPath];
            }

            if (parentObj == null) return;

            GameObject obj = CreateEmptyNode(nodeName);
            obj.transform.SetParent(parentObj.transform, false);
            nodeDict[path] = obj;
        }

        private void SetupNodeTransform(JObject node)
        {
            if (node == null) return;

            string tree = node["Tree"].ToString().Replace('\\', '/');
            if (!tree.StartsWith("PSDRoot/"))
                tree = "PSDRoot/" + tree;

            GameObject obj = nodeDict[tree];
            if (obj == null) return;

            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            try
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);

                bool isTextNode = node["Type"].ToString().ToLower() == "text";

                if (node["Size"] != null && !isTextNode)
                {
                    float width = node["Size"]["Width"].ToObject<float>();
                    float height = node["Size"]["Height"].ToObject<float>();
                    rectTransform.sizeDelta = new Vector2(width * config.UserScaleRatio, height * config.UserScaleRatio);
                }

                if (node["Pos"] != null)
                {
                    float absoluteX = node["Pos"]["X"].ToObject<float>();
                    float absoluteY = node["Pos"]["Y"].ToObject<float>();

                    string[] pathParts = tree.Split('/');
                    if (pathParts.Length > 2)
                    {
                        string parentTree = string.Join("/", pathParts.Take(pathParts.Length - 1));
                        var parentNode = ((JArray)jsonData["InfoList"])
                            .FirstOrDefault(n =>
                            {
                                string nTree = n["Tree"].ToString().Replace('\\', '/');
                                if (!nTree.StartsWith("PSDRoot/"))
                                    nTree = "PSDRoot/" + nTree;
                                return nTree == parentTree;
                            }) as JObject;

                        if (parentNode != null && parentNode["Pos"] != null)
                        {
                            float parentAbsoluteX = parentNode["Pos"]["X"].ToObject<float>();
                            float parentAbsoluteY = parentNode["Pos"]["Y"].ToObject<float>();

                            float relativeX = (absoluteX - parentAbsoluteX) * config.UserScaleRatio;
                            float relativeY = (absoluteY - parentAbsoluteY) * config.UserScaleRatio;

                            if (isTextNode && node["Size"] != null)
                                AdjustTextNodePosition(node, obj, ref relativeX, ref relativeY);

                            rectTransform.anchoredPosition = new Vector2(relativeX, -relativeY);
                        }
                        else
                        {
                            rectTransform.anchoredPosition = new Vector2(absoluteX, -absoluteY);
                        }
                    }
                    else
                    {
                        rectTransform.anchoredPosition = new Vector2(
                            (absoluteX - config.OffsetX) * config.UserScaleRatio,
                            -(absoluteY - config.OffsetY) * config.UserScaleRatio
                        );
                    }
                }

                if (!isTextNode)
                {
                    AdjustPositionForPivotChange(rectTransform, new Vector2(0, 1), new Vector2(0.5f, 0.5f));
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"设置 {obj.name} Transform出错: {e.Message}");
            }
        }

        private void AdjustPositionForPivotChange(RectTransform rectTransform, Vector2 oldPivot, Vector2 newPivot)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 pivotDelta = newPivot - oldPivot;
            Vector2 positionDelta = new Vector2(pivotDelta.x * size.x, pivotDelta.y * size.y);
            rectTransform.anchoredPosition += positionDelta;
        }

        private void AdjustTextNodePosition(JObject node, GameObject obj, ref float relativeX, ref float relativeY)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            Text text = obj.GetComponent<Text>();

            float originalWidth = node["Size"]["Width"].ToObject<float>() * config.UserScaleRatio;
            float originalHeight = node["Size"]["Height"].ToObject<float>() * config.UserScaleRatio;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            float actualWidth = rectTransform.rect.width;
            float actualHeight = rectTransform.rect.height;

            float widthDiff = originalWidth - actualWidth;
            float heightDiff = originalHeight - actualHeight;

            switch (text.alignment)
            {
                case TextAnchor.MiddleLeft:
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    relativeY += heightDiff / 2;
                    break;
                case TextAnchor.MiddleCenter:
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    relativeX += widthDiff / 2;
                    relativeY += heightDiff / 2;
                    break;
                case TextAnchor.MiddleRight:
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    relativeX += widthDiff;
                    relativeY += heightDiff / 2;
                    break;
            }
        }
    }
}
