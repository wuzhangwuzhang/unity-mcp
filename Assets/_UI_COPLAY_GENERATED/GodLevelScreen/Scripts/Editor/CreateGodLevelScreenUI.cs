using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class CreateGodLevelScreenUI
{
    [System.Serializable]
    public class UIElement
    {
        public string name;
        public VectorPos2Wrapper position;
        public VectorSize2Wrapper size;
        public string type;
        public string backgroundType;
        public string text;
        public List<UIElement> children;
        
        public Vector2 GetPosition()
        {
            return position.ToVector2();
        }

        public Vector2 GetSize()
        {
            return size.ToVector2();
        }
    }

    [System.Serializable]
    public class UIConfig
    {
        public string name;
        public VectorPos2Wrapper position;
        public VectorSize2Wrapper size;
        public string type;
        public string backgroundType;
        public List<UIElement> children;
        
        public Vector2 GetPosition()
        {
            return position.ToVector2();
        }

        public Vector2 GetSize()
        {
            return size.ToVector2();
        }
    }
    
    [System.Serializable]
    public class VectorSize2Wrapper
    {
        public float width;
        public float height;

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }
    }
    
    [System.Serializable]
    public class VectorPos2Wrapper
    {
        public float x;
        public float y;

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }

    [MenuItem("Tools/Create God Level Screen UI")]
    public static void Execute()
    {
        string jsonPath = "Assets/_UI_COPLAY_GENERATED/GodLevelScreen/config.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Config file not found at " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        UIConfig config = JsonUtility.FromJson<UIConfig>(json);

        // Delete existing Canvas if it exists
        GameObject existingCanvas = GameObject.Find(config.name);
        if (existingCanvas != null)
        {
            GameObject.DestroyImmediate(existingCanvas);
        }

        // Create Canvas
        GameObject canvasObj = new GameObject(config.name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = config.GetSize();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create children
        if (config.children != null)
        {
            foreach (var child in config.children)
            {
                CreateElement(child, canvasObj.transform);
            }
        }
        
        // Save as prefab
        string prefabDir = "Assets/_UI_COPLAY_GENERATED/GodLevelScreen/Prefabs";
        if (!Directory.Exists(prefabDir))
        {
            Directory.CreateDirectory(prefabDir);
        }
        PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabDir + "/" + config.name + ".prefab");
        Debug.Log("UI Prefab created successfully at " + prefabDir + "/" + config.name + ".prefab");
    }

    private static void CreateElement(UIElement element, Transform parent)
    {
        GameObject obj = new GameObject(element.name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = element.GetSize();
        rect.anchoredPosition = element.GetPosition();

        if (element.type == "Image" || element.type == "Button" || element.type == "Slider")
        {
            Image img = obj.AddComponent<Image>();
            img.type = Image.Type.Simple;
            if (element.backgroundType == "Sliced")
            {
                img.type = Image.Type.Sliced;
            }
            
            // Use default UI sprite
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            
            if (element.type == "Button")
            {
                obj.AddComponent<Button>();
            }
            else if (element.type == "Slider")
            {
                Slider slider = obj.AddComponent<Slider>();
                
                // Create Fill Area
                GameObject fillArea = new GameObject("Fill Area");
                fillArea.transform.SetParent(obj.transform, false);
                RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
                fillAreaRect.anchorMin = new Vector2(0, 0.25f);
                fillAreaRect.anchorMax = new Vector2(1, 0.75f);
                fillAreaRect.offsetMin = new Vector2(5, 0);
                fillAreaRect.offsetMax = new Vector2(-5, 0);
                
                // Create Fill
                GameObject fill = new GameObject("Fill");
                fill.transform.SetParent(fillArea.transform, false);
                RectTransform fillRect = fill.AddComponent<RectTransform>();
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                Image fillImg = fill.AddComponent<Image>();
                fillImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                fillImg.type = Image.Type.Sliced;
                fillImg.color = Color.yellow;
                
                slider.fillRect = fillRect;
                slider.value = 0.5f;
            }
        }

        if (element.type == "Text" || !string.IsNullOrEmpty(element.text))
        {
            GameObject textObj = obj;
            if (element.type != "Text")
            {
                textObj = new GameObject("Text");
                textObj.transform.SetParent(obj.transform, false);
                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
            }

            Text text = textObj.AddComponent<Text>();
            text.text = element.text;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.fontSize = 16;
            text.font = Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;
        }

        if (element.children != null)
        {
            foreach (var child in element.children)
            {
                CreateElement(child, obj.transform);
            }
        }
    }
}