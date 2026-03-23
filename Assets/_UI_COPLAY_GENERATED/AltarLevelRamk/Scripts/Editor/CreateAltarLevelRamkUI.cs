using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class CreateAltarLevelRamkUI
{
    [MenuItem("Tools/Create AltarLevelRamk UI")]
    public static void Execute()
    {
        string configPath = "Assets/_UI_COPLAY_GENERATED/AltarLevelRamk/config.json";
        if (!File.Exists(configPath))
        {
            Debug.LogError("Config file not found at " + configPath);
            return;
        }

        string json = File.ReadAllText(configPath);
        UIConfig config = JsonUtility.FromJson<UIConfig>(json);

        // Create Canvas
        GameObject canvasObj = new GameObject("AltarLevelRamkCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(config.size.width, config.size.height);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create children
        foreach (var child in config.children)
        {
            CreateUIElement(child, canvasObj.transform);
        }

        // Save as prefab
        string prefabFolder = "Assets/_UI_COPLAY_GENERATED/AltarLevelRamk/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            Directory.CreateDirectory(prefabFolder);
            AssetDatabase.Refresh();
        }

        string prefabPath = prefabFolder + "/Canvas.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
        GameObject.DestroyImmediate(canvasObj);
        Debug.Log("UI Prefab created at " + prefabPath);
    }

    private static void CreateUIElement(UIElementConfig config, Transform parent)
    {
        GameObject obj = new GameObject(config.name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(config.position.x, config.position.y);
        rect.sizeDelta = new Vector2(config.size.width, config.size.height);

        if (config.type == "Image" || config.type == "Button" || config.backgroundType == "Sliced" || config.backgroundType == "Simple")
        {
            Image img = obj.AddComponent<Image>();
            if (config.backgroundType == "Sliced")
            {
                img.type = Image.Type.Sliced;
            }
            else
            {
                img.type = Image.Type.Simple;
            }
            // Set default sprite if needed
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        if (config.type == "Button")
        {
            obj.AddComponent<Button>();
        }

        if (!string.IsNullOrEmpty(config.text))
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = config.text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            tmp.fontSize = 30;
        }

        if (config.children != null)
        {
            foreach (var child in config.children)
            {
                CreateUIElement(child, obj.transform);
            }
        }
    }

    [System.Serializable]
    public class UIConfig
    {
        public string name;
        public string type;
        public string backgroundType;
        public Position position;
        public Size size;
        public List<UIElementConfig> children;
    }

    [System.Serializable]
    public class UIElementConfig
    {
        public string name;
        public string type;
        public string backgroundType;
        public string text;
        public Position position;
        public Size size;
        public List<UIElementConfig> children;
    }

    [System.Serializable]
    public class Position
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class Size
    {
        public float width;
        public float height;
    }
}
