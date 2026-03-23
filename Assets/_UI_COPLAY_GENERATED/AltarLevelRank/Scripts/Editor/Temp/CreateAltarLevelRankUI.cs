using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class CreateAltarLevelRankUI
{
    [System.Serializable]
    public class UIControl
    {
        public string name;
        public Position position;
        public Size size;
        public string type;
        public string backgroundType;
        public string text;
    }

    [System.Serializable]
    public class UIConfig
    {
        public string name;
        public Position position;
        public Size size;
        public string type;
        public List<UIControl> controls;
    }

    [System.Serializable]
    public class Position
    {
        public int x;
        public int y;
    }

    [System.Serializable]
    public class Size
    {
        public int width;
        public int height;
    }

    public static void Execute()
    {
        string jsonPath = "Assets/_UI_COPLAY_GENERATED/AltarLevelRank/config.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Config file not found at " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        UIConfig config = JsonUtility.FromJson<UIConfig>(json);

        if (config == null)
        {
            Debug.LogError("Failed to parse JSON config.");
            return;
        }

        // Delete existing if any
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
        scaler.referenceResolution = new Vector2(config.size.width, config.size.height);
        canvasObj.AddComponent<GraphicRaycaster>();

        if (config.controls == null)
        {
            Debug.LogError("No controls found in JSON config.");
            return;
        }

        // Create Controls
        foreach (var control in config.controls)
        {
            GameObject obj = new GameObject(control.name);
            obj.transform.SetParent(canvasObj.transform, false);

            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(control.position.x, control.position.y);
            rt.sizeDelta = new Vector2(control.size.width, control.size.height);

            if (control.type == "Image" || control.type == "Button" || control.type == "Toggle")
            {
                if (control.backgroundType != "None")
                {
                    Image img = obj.AddComponent<Image>();
                    string spritePath = $"Assets/_UI_COPLAY_GENERATED/AltarLevelRank/Sprites/{control.name}.png";
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                    if (sprite != null)
                    {
                        img.sprite = sprite;
                        if (control.backgroundType == "Sliced")
                        {
                            img.type = Image.Type.Sliced;
                        }
                        else
                        {
                            img.type = Image.Type.Simple;
                        }
                    }
                    else
                    {
                        // Default color if no sprite
                        img.color = new Color(1, 1, 1, 0.5f);
                    }
                }
            }

            if (control.type == "Text" || !string.IsNullOrEmpty(control.text))
            {
                GameObject textObj = obj;
                if (control.type != "Text")
                {
                    textObj = new GameObject("Text");
                    textObj.transform.SetParent(obj.transform, false);
                    RectTransform textRt = textObj.AddComponent<RectTransform>();
                    textRt.anchorMin = Vector2.zero;
                    textRt.anchorMax = Vector2.one;
                    textRt.sizeDelta = Vector2.zero;
                    textRt.anchoredPosition = Vector2.zero;
                }

                TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
                tmp.text = control.text;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
                tmp.fontSize = 36;
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = 14;
                tmp.fontSizeMax = 72;
            }

            if (control.type == "Button")
            {
                obj.AddComponent<Button>();
            }
            else if (control.type == "Toggle")
            {
                obj.AddComponent<Toggle>();
            }
        }

        // Save as Prefab
        string prefabDir = "Assets/_UI_COPLAY_GENERATED/AltarLevelRank/Prefabs";
        if (!Directory.Exists(prefabDir))
        {
            Directory.CreateDirectory(prefabDir);
        }
        string prefabPath = $"{prefabDir}/{config.name}.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(canvasObj, prefabPath, InteractionMode.UserAction);
        
        Debug.Log("UI created successfully!");
    }
}