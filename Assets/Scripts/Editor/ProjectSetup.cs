using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BreakoutGame.Editor
{
    /// <summary>
    /// 项目设置工具，自动创建Tags和Layers / Project setup tool to auto-create Tags and Layers
    /// </summary>
    public class ProjectSetup : EditorWindow
    {
        [MenuItem("Breakout/Setup Project (Tags & Layers)")]
        public static void SetupProject()
        {
            Debug.Log("[ProjectSetup] Starting project setup...");
            
            bool tagsCreated = CreateTags();
            bool layersVerified = VerifyLayers();
            
            if (tagsCreated && layersVerified)
            {
                EditorUtility.DisplayDialog("Setup Complete", 
                    "Project setup completed successfully!\n\n" +
                    "Tags created:\n" +
                    "- Paddle\n" +
                    "- Ball\n" +
                    "- Brick\n\n" +
                    "Layers verified:\n" +
                    "- Ball (Layer 6)\n" +
                    "- Paddle (Layer 7)\n" +
                    "- Brick (Layer 8)\n" +
                    "- Boundary (Layer 9)\n\n" +
                    "You can now run:\n" +
                    "- Breakout → Setup StartMenu Scene\n" +
                    "- Breakout → Setup GameScene", 
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Setup Issues", 
                    "Project setup completed with some issues.\n" +
                    "Please check the Console for details.", 
                    "OK");
            }
        }
        
        private static bool CreateTags()
        {
            // 获取TagManager / Get TagManager
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            
            // 需要创建的Tags / Tags to create
            string[] requiredTags = { "Paddle", "Ball", "Brick" };
            
            foreach (string tag in requiredTags)
            {
                // 检查Tag是否已存在 / Check if tag already exists
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(tag))
                    {
                        found = true;
                        break;
                    }
                }
                
                // 如果不存在，添加Tag / If not found, add tag
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
                    newTag.stringValue = tag;
                    Debug.Log($"[ProjectSetup] Created tag: {tag}");
                }
                else
                {
                    Debug.Log($"[ProjectSetup] Tag already exists: {tag}");
                }
            }
            
            // 保存更改 / Save changes
            tagManager.ApplyModifiedProperties();
            
            Debug.Log("[ProjectSetup] Tags setup completed");
            return true;
        }
        
        private static bool VerifyLayers()
        {
            // 验证Layers是否存在 / Verify layers exist
            string[] requiredLayers = { "Ball", "Paddle", "Brick", "Boundary" };
            int[] requiredLayerIndices = { 6, 7, 8, 9 };
            
            bool allLayersExist = true;
            
            for (int i = 0; i < requiredLayers.Length; i++)
            {
                string layerName = requiredLayers[i];
                int expectedIndex = requiredLayerIndices[i];
                
                int actualIndex = LayerMask.NameToLayer(layerName);
                
                if (actualIndex == -1)
                {
                    Debug.LogWarning($"[ProjectSetup] Layer '{layerName}' not found at index {expectedIndex}");
                    Debug.LogWarning($"[ProjectSetup] Please manually create layer '{layerName}' at index {expectedIndex} in Edit → Project Settings → Tags and Layers");
                    allLayersExist = false;
                }
                else if (actualIndex != expectedIndex)
                {
                    Debug.LogWarning($"[ProjectSetup] Layer '{layerName}' found at index {actualIndex}, but expected at {expectedIndex}");
                    allLayersExist = false;
                }
                else
                {
                    Debug.Log($"[ProjectSetup] Layer '{layerName}' verified at index {expectedIndex}");
                }
            }
            
            if (!allLayersExist)
            {
                Debug.LogWarning("[ProjectSetup] Some layers are missing or incorrectly configured");
                Debug.LogWarning("[ProjectSetup] Layers should have been created in Task 1.1");
                Debug.LogWarning("[ProjectSetup] Please check ProjectSettings/TagManager.asset");
            }
            else
            {
                Debug.Log("[ProjectSetup] All layers verified successfully");
            }
            
            return allLayersExist;
        }
    }
}
