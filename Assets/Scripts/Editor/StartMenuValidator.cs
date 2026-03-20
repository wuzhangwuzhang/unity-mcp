using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BreakoutGame.Editor
{
    /// <summary>
    /// StartMenu场景验证工具 / StartMenu scene validation tool
    /// </summary>
    public class StartMenuValidator : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool validationComplete = false;
        private string validationReport = "";
        
        [MenuItem("Breakout/Validate StartMenu Scene")]
        public static void ShowWindow()
        {
            GetWindow<StartMenuValidator>("StartMenu Validator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("StartMenu Scene Validator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Run Validation", GUILayout.Height(30)))
            {
                RunValidation();
            }
            
            GUILayout.Space(10);
            
            if (validationComplete)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                GUILayout.TextArea(validationReport, GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
            }
        }
        
        private void RunValidation()
        {
            validationReport = "";
            int passCount = 0;
            int failCount = 0;
            
            AddReport("=== StartMenu Scene Validation Report ===\n");
            
            // 检查场景是否打开 / Check if scene is open
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.name != "StartMenu")
            {
                AddReport("⚠️ WARNING: StartMenu scene is not currently open");
                AddReport("   Please open StartMenu scene first\n");
            }
            else
            {
                AddReport("✅ StartMenu scene is open\n");
                passCount++;
            }
            
            // 检查GameManager / Check GameManager
            AddReport("--- GameManager Check ---");
            var gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                AddReport("❌ FAIL: GameManager GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: GameManager GameObject exists");
                passCount++;
                
                var gmComponent = gameManager.GetComponent<Core.GameManager>();
                if (gmComponent == null)
                {
                    AddReport("❌ FAIL: GameManager component not found");
                    failCount++;
                }
                else
                {
                    AddReport("✅ PASS: GameManager component exists");
                    passCount++;
                }
                
                var scoreSystem = gameManager.GetComponent<Core.ScoreSystem>();
                if (scoreSystem == null)
                {
                    AddReport("❌ FAIL: ScoreSystem component not found");
                    failCount++;
                }
                else
                {
                    AddReport("✅ PASS: ScoreSystem component exists");
                    passCount++;
                }
            }
            AddReport("");
            
            // 检查Canvas / Check Canvas
            AddReport("--- Canvas Check ---");
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                AddReport("❌ FAIL: Canvas GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: Canvas GameObject exists");
                passCount++;
                
                var canvasComponent = canvas.GetComponent<Canvas>();
                if (canvasComponent == null)
                {
                    AddReport("❌ FAIL: Canvas component not found");
                    failCount++;
                }
                else
                {
                    AddReport("✅ PASS: Canvas component exists");
                    AddReport($"   Render Mode: {canvasComponent.renderMode}");
                    passCount++;
                }
                
                var startMenuUI = canvas.GetComponent<UI.StartMenuUI>();
                if (startMenuUI == null)
                {
                    AddReport("❌ FAIL: StartMenuUI component not found on Canvas");
                    failCount++;
                }
                else
                {
                    AddReport("✅ PASS: StartMenuUI component exists");
                    passCount++;
                    
                    // 使用反射检查引用 / Check references using reflection
                    var startButtonField = typeof(UI.StartMenuUI).GetField("startButton", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var titleTextField = typeof(UI.StartMenuUI).GetField("titleText", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var instructionsTextField = typeof(UI.StartMenuUI).GetField("instructionsText", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (startButtonField != null)
                    {
                        var startButtonRef = startButtonField.GetValue(startMenuUI) as Button;
                        if (startButtonRef == null)
                        {
                            AddReport("❌ FAIL: Start Button reference not set");
                            failCount++;
                        }
                        else
                        {
                            AddReport("✅ PASS: Start Button reference is set");
                            passCount++;
                        }
                    }
                    
                    if (titleTextField != null)
                    {
                        var titleTextRef = titleTextField.GetValue(startMenuUI) as Text;
                        if (titleTextRef == null)
                        {
                            AddReport("⚠️ WARNING: Title Text reference not set");
                        }
                        else
                        {
                            AddReport("✅ PASS: Title Text reference is set");
                            passCount++;
                        }
                    }
                    
                    if (instructionsTextField != null)
                    {
                        var instructionsTextRef = instructionsTextField.GetValue(startMenuUI) as Text;
                        if (instructionsTextRef == null)
                        {
                            AddReport("⚠️ WARNING: Instructions Text reference not set");
                        }
                        else
                        {
                            AddReport("✅ PASS: Instructions Text reference is set");
                            passCount++;
                        }
                    }
                }
            }
            AddReport("");
            
            // 检查UI元素 / Check UI elements
            AddReport("--- UI Elements Check ---");
            var titleText = GameObject.Find("TitleText");
            if (titleText == null)
            {
                AddReport("❌ FAIL: TitleText GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: TitleText GameObject exists");
                passCount++;
            }
            
            var startButton = GameObject.Find("StartButton");
            if (startButton == null)
            {
                AddReport("❌ FAIL: StartButton GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: StartButton GameObject exists");
                passCount++;
            }
            
            var instructionsText = GameObject.Find("InstructionsText");
            if (instructionsText == null)
            {
                AddReport("❌ FAIL: InstructionsText GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: InstructionsText GameObject exists");
                passCount++;
            }
            AddReport("");
            
            // 检查EventSystem / Check EventSystem
            AddReport("--- EventSystem Check ---");
            var eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                AddReport("❌ FAIL: EventSystem GameObject not found");
                failCount++;
            }
            else
            {
                AddReport("✅ PASS: EventSystem GameObject exists");
                passCount++;
            }
            AddReport("");
            
            // 总结 / Summary
            AddReport("=== Validation Summary ===");
            AddReport($"✅ Passed: {passCount}");
            AddReport($"❌ Failed: {failCount}");
            AddReport("");
            
            if (failCount == 0)
            {
                AddReport("🎉 All checks passed! StartMenu scene is ready.");
            }
            else
            {
                AddReport("⚠️ Some checks failed. Please fix the issues above.");
                AddReport("\nSuggested Actions:");
                AddReport("1. Run 'Breakout → Setup StartMenu Scene' to auto-create UI");
                AddReport("2. Manually set missing references in Inspector");
                AddReport("3. Check StartMenu_Setup_Instructions.md for details");
            }
            
            validationComplete = true;
        }
        
        private void AddReport(string message)
        {
            validationReport += message + "\n";
        }
    }
}
