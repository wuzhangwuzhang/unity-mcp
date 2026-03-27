using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 修正幻灵降临 prefab 节点：
/// 1. #btn_ / #imgBtn_ 节点补充 Button 组件
/// 2. 名字含 bg（不区分大小写）或不需要代码控制的装饰性节点，移除 # 前缀
/// </summary>
public class FixHuanLingPrefab
{
    private const string PrefabPath = "Assets/Res/common_by_u3d/res/Prefab/UI/Panels/ActivitySystems/ty_qyxlxxx/UIPrefab/ty_qyxlxxx.prefab";

    [MenuItem("Tools/修正幻灵降临Prefab节点")]
    public static void Execute()
    {
        var prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (prefabRoot == null)
        {
            Debug.LogError("无法打开 prefab");
            return;
        }

        try
        {
            var allTransforms = prefabRoot.GetComponentsInChildren<Transform>(true);

            // ========== 第一步：先打印当前所有节点信息用于分析 ==========
            Debug.Log("===== 当前 Prefab 节点列表 =====");
            foreach (var t in allTransforms)
            {
                var components = new List<string>();
                foreach (var c in t.GetComponents<Component>())
                {
                    if (c != null)
                        components.Add(c.GetType().Name);
                }
                string path = GetPath(t);
                Debug.Log($"[节点] {path} | 组件: {string.Join(", ", components)}");
            }

            // ========== 第二步：检查 #btn_ 和 #imgBtn_ 节点，补充 Button 组件 ==========
            Debug.Log("===== 检查按钮组件 =====");
            foreach (var t in allTransforms)
            {
                string name = t.name;
                if (name.StartsWith("#btn_") || name.StartsWith("#imgBtn_"))
                {
                    var btn = t.GetComponent<Button>();
                    if (btn == null)
                    {
                        // 补充 Button 组件
                        t.gameObject.AddComponent<Button>();
                        Debug.Log($"[补充Button] {name} - 已添加 Button 组件");
                    }
                    else
                    {
                        Debug.Log($"[已有Button] {name} - 无需处理");
                    }
                }
            }

            // ========== 第三步：移除不需要代码控制的节点的 # 前缀 ==========
            // 规则：
            // 1. 名字中包含 bg（不区分大小写）的节点 -> 移除 #
            // 2. 纯装饰性/不需要代码逻辑控制的节点 -> 移除 #
            Debug.Log("===== 移除装饰性节点的 # 前缀 =====");

            // 包含 bg 的节点自动移除 #
            // 其他不需要代码控制的装饰性节点手动列出
            var decorativeNodes = new HashSet<string>
            {
                // 以下是纯装饰性节点，不需要代码动态控制
                "#img_FireEffect1",      // 火焰特效装饰
                "#img_FireEffect2",      // 火焰特效装饰
                "#img_TitlePattern",     // 标题花纹装饰
                "#img_TitleDecor",       // 标题装饰图
                "#img_TitleGlow",        // 标题光效装饰
                "#img_GoldProgressBg",   // 含bg - 金币进度条底图
                "#img_GoldBannerBg",     // 含bg - 金币横幅底图
                "#img_BonusBg",          // 含bg - 加成底图
                "#img_FragmentBg",       // 含bg - 碎片底图
                "#img_BuyBtnBg",         // 含bg - 购买按钮底图
                "#img_ValueTagBg",       // 含bg - 超值标签底图
                "#img_PriceTab1Bg",      // 含bg - 价格标签1底图
                "#img_PriceTab2Bg",      // 含bg - 价格标签2底图
                "#img_SkillLine1",       // 技能分隔线装饰
                "#img_SkillLineBg",      // 含bg - 技能分隔线底
                "#img_SkillLine2",       // 技能分隔线装饰
                "#img_SkillFrame1",      // 技能图标底框装饰
                "#img_SkillFrame2",      // 技能图标底框装饰
                "#img_FragmentDivider",  // 碎片分隔线装饰
                "#img_FragmentLabel1",   // 碎片标签装饰
                "#img_FragmentLabel2",   // 碎片标签装饰
                "#img_FragmentLabel3",   // 碎片标签装饰
            };

            foreach (var t in allTransforms)
            {
                string name = t.name;
                if (!name.StartsWith("#")) continue;

                bool shouldRemoveHash = false;
                string reason = "";

                // 规则1：名字中包含 bg（不区分大小写）
                if (name.ToLower().Contains("bg"))
                {
                    shouldRemoveHash = true;
                    reason = "包含bg";
                }
                // 规则2：在装饰性节点列表中
                else if (decorativeNodes.Contains(name))
                {
                    shouldRemoveHash = true;
                    reason = "装饰性节点";
                }

                if (shouldRemoveHash)
                {
                    string newName = name.Substring(1); // 移除开头的 #
                    t.name = newName;
                    Debug.Log($"[移除#] {name} -> {newName} (原因: {reason})");
                }
            }

            // 保存
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Debug.Log("===== 修正完成 =====");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.Refresh();
    }

    private static string GetPath(Transform t)
    {
        string path = t.name;
        Transform parent = t.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
