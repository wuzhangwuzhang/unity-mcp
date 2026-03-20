using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BreakoutGame.Editor
{
    /// <summary>
    /// StartMenu场景自动设置工具 / StartMenu scene auto-setup tool
    /// </summary>
    public class StartMenuSetup : EditorWindow
    {
        [MenuItem("Breakout/Setup StartMenu Scene")]
        public static void SetupStartMenuScene()
        {
            // 打开StartMenu场景 / Open StartMenu scene
            EditorSceneManager.OpenScene("Assets/Scenes/StartMenu.unity");
            
            // 创建GameManager / Create GameManager
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<Core.GameManager>();
            gameManagerObj.AddComponent<Core.ScoreSystem>();
            
            // 创建GameProgress / Create GameProgress
            GameObject gameProgressObj = new GameObject("GameProgress");
            gameProgressObj.AddComponent<Data.GameProgress>();
            
            // 创建Canvas / Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f; // 0=宽度优先 / 0=width priority
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建EventSystem / Create EventSystem
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // 创建TitleText / Create TitleText
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "弹球打砖块\nBreakout Game";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 60;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 30;
            titleText.resizeTextMaxSize = 60;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.75f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 创建StartButton / Create StartButton
            GameObject buttonObj = new GameObject("StartButton");
            buttonObj.transform.SetParent(canvasObj.transform, false);
            Button button = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = Color.white;
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.35f, 0.45f);
            buttonRect.anchorMax = new Vector2(0.65f, 0.55f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // 创建Button的Text子对象 / Create Button's Text child
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "开始游戏 / Start Game";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 32;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.black;
            buttonText.resizeTextForBestFit = true;
            buttonText.resizeTextMinSize = 18;
            buttonText.resizeTextMaxSize = 32;
            
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            // 创建InstructionsText / Create InstructionsText
            GameObject instructionsObj = new GameObject("InstructionsText");
            instructionsObj.transform.SetParent(canvasObj.transform, false);
            Text instructionsText = instructionsObj.AddComponent<Text>();
            instructionsText.text = "操作说明 / Controls:\n" +
                                   "A / ← : 向左移动挡板 / Move paddle left\n" +
                                   "D / → : 向右移动挡板 / Move paddle right\n\n" +
                                   "目标 / Goal:\n" +
                                   "击碎所有砖块获得胜利！\n" +
                                   "Destroy all bricks to win!";
            instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructionsText.fontSize = 24;
            instructionsText.alignment = TextAnchor.MiddleCenter;
            instructionsText.color = Color.white;
            instructionsText.resizeTextForBestFit = true;
            instructionsText.resizeTextMinSize = 14;
            instructionsText.resizeTextMaxSize = 24;
            
            RectTransform instructionsRect = instructionsObj.GetComponent<RectTransform>();
            instructionsRect.anchorMin = new Vector2(0.15f, 0.05f);
            instructionsRect.anchorMax = new Vector2(0.55f, 0.35f);
            instructionsRect.offsetMin = Vector2.zero;
            instructionsRect.offsetMax = Vector2.zero;
            
            // 创建ProgressText / Create ProgressText
            GameObject progressObj = new GameObject("ProgressText");
            progressObj.transform.SetParent(canvasObj.transform, false);
            Text progressText = progressObj.AddComponent<Text>();
            progressText.text = "Progress / 进度:\nLoading...";
            progressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            progressText.fontSize = 28;
            progressText.alignment = TextAnchor.UpperRight;
            progressText.color = Color.yellow;
            progressText.resizeTextForBestFit = true;
            progressText.resizeTextMinSize = 16;
            progressText.resizeTextMaxSize = 28;
            
            RectTransform progressRect = progressObj.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0.65f, 0.75f);
            progressRect.anchorMax = new Vector2(0.95f, 0.95f);
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = Vector2.zero;
            
            // 添加StartMenuUI脚本并设置引用 / Add StartMenuUI script and set references
            UI.StartMenuUI startMenuUI = canvasObj.AddComponent<UI.StartMenuUI>();
            
            // 使用反射设置私有字段 / Use reflection to set private fields
            var startButtonField = typeof(UI.StartMenuUI).GetField("startButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var titleTextField = typeof(UI.StartMenuUI).GetField("titleText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var instructionsTextField = typeof(UI.StartMenuUI).GetField("instructionsText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var progressTextField = typeof(UI.StartMenuUI).GetField("progressText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (startButtonField != null) startButtonField.SetValue(startMenuUI, button);
            if (titleTextField != null) titleTextField.SetValue(startMenuUI, titleText);
            if (instructionsTextField != null) instructionsTextField.SetValue(startMenuUI, instructionsText);
            if (progressTextField != null) progressTextField.SetValue(startMenuUI, progressText);
            
            // 标记场景为已修改 / Mark scene as modified
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("[StartMenuSetup] StartMenu scene setup completed!");
            EditorUtility.DisplayDialog("Setup Complete", 
                "StartMenu scene has been set up successfully!\n\n" +
                "Please save the scene (Ctrl+S) and assign the UI references in the Inspector if needed.", 
                "OK");
        }
    }
}
