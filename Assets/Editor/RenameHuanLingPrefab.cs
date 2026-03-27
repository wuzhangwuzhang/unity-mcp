using UnityEngine;
using UnityEditor;

/// <summary>
/// 根据策划需求文档和UI命名规范，重命名幻灵降临礼包界面 prefab 的所有节点
/// </summary>
public class RenameHuanLingPrefab
{
    // prefab 路径
    private const string PrefabPath = "Assets/Res/common_by_u3d/res/Prefab/UI/Panels/ActivitySystems/ty_qyxlxxx/UIPrefab/ty_qyxlxxx.prefab";

    [MenuItem("Tools/重命名幻灵降临Prefab节点")]
    public static void Execute()
    {
        // 加载 prefab
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"找不到 prefab: {PrefabPath}");
            return;
        }

        // 打开 prefab 编辑模式
        var prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (prefabRoot == null)
        {
            Debug.LogError("无法打开 prefab 进行编辑");
            return;
        }

        try
        {
            // 定义重命名映射表: 旧名称 -> 新名称
            // 按照层级从深到浅的顺序处理，避免路径变化导致找不到节点
            var renameMap = new System.Collections.Generic.Dictionary<string, string>
            {
                // === 根节点 ===
                // ty_qyxlxxx 是根节点，保持不变

                // === ui 层 (主容器) ===
                // img_bg_nz -> #img_BgNz (背景图)
                { "img_bg_nz", "#img_Bg" },

                // ui -> MainPanel (主UI容器，纯容器不加前缀)
                { "ui", "MainPanel" },

                // === MainPanel 下的子节点 ===
                // img_nz -> #img_HeroFull (幻灵形象大图)
                { "img_nz", "#img_HeroFull" },
                // img_fire_2 -> #img_FireEffect2 (火焰特效2)
                { "img_fire_2", "#img_FireEffect2" },
                // img_fire_1 -> #img_FireEffect1 (火焰特效1)
                { "img_fire_1", "#img_FireEffect1" },

                // right -> RightArea (右侧区域，纯容器)
                { "right", "RightArea" },

                // === 金币面板 panel_gold ===
                { "panel_gold", "GoldPanel" },
                { "img_gold", "#img_Gold" },
                { "pamel_num", "GoldNumArea" },
                { "jx_3", "#img_GoldProgressBg" },
                { "999", "#lbl_GoldNum" },
                { "img_banzi", "#img_GoldBannerBg" },
                { "z_1239343", "#img_FragmentTip" },

                // === 集成面板 panel_jc ===
                { "panel_jc", "BonusPanel" },
                { "didi", "#img_BonusBg" },
                { "jbjc", "#img_BonusIcon" },
                { "btn_xq", "#imgBtn_BonusDetail" },
                { "1000", "#lbl_BonusPercent" },

                // === 下方碎片区域 xia ===
                { "xia", "FragmentArea" },
                { "img_di", "#img_FragmentBg" },
                { "z_1239407", "FragmentListPanel" },
                { "jq_gsp", "#lbl_FragmentCollectTip" },
                { "jksz", "#img_FragmentDivider" },
                { "z_7_kb_2", "#img_Fragment1" },
                { "z_7_kb_3", "#img_Fragment2" },
                { "z_7_kb_4", "#img_Fragment3" },
                { "10", "#lbl_FragmentCount" },
                { "z_1239380_kb", "#img_FragmentLabel1" },
                { "z_1239380_kb_2", "#img_FragmentLabel2" },
                { "z_1239380_kb_3", "#img_FragmentLabel3" },
                { "jb", "#img_FragmentReward" },

                // === 购买区域 buy ===
                { "buy", "BuyArea" },
                { "btn_buy", "#btn_Buy" },
                { "ui_buy_btn_buy", "#img_BuyBtnBg" },
                { "img_dq", "#img_BuyTicketIcon" },
                { "648", "#lbl_BuyPrice" },
                { "gmcs_0_4", "#lbl_BuyCount" },
                { "cz", "ValueTagPanel" },
                { "img_kuang", "#img_ValueTagBg" },
                { "ui_buy_cz_cz_cz", "#img_ValueTagIcon" },
                { "ui_buy_cz_1000", "#lbl_ValueTagPercent" },

                // === 技能面板 panel_skill ===
                { "panel_skill", "SkillPanel" },
                { "di", "SkillContentPanel" },
                { "jx_605", "#img_SkillLine1" },
                { "jx_605_kb", "#img_SkillLineBg" },
                { "jx_605_kb_2", "#img_SkillLine2" },
                { "dzmc", "#lbl_SkillName" },
                { "ty_1", "#img_SkillFrame1" },
                { "ty_1_kb", "#img_SkillFrame2" },
                { "img_skill_1", "#img_SkillIcon1" },
                { "jxdz", "#lbl_AwakenSkillName" },
                { "img_skill_2", "#img_SkillIcon2" },

                // === 点券按钮面板 panel_btn ===
                { "panel_btn", "PriceTabPanel" },
                { "btn_liang", "#btn_PriceTab1" },
                { "btn_yellow", "#img_PriceTab1Bg" },
                { "50", "#lbl_PriceTab1Value" },
                { "dq", "#lbl_PriceTab1Ticket" },
                { "btn_an", "#btn_PriceTab2" },
                { "btn_red", "#img_PriceTab2Bg" },
                { "98", "#lbl_PriceTab2Value" },
                { "btn_ui_dq", "#lbl_PriceTab2Ticket" },

                // === 其他按钮 ===
                { "btn_quanyi", "#imgBtn_Benefit" },
                { "btn_close", "#imgBtn_Close" },

                // === 标题区域 img_title ===
                { "img_title", "TitleArea" },
                { "img_huawen", "#img_TitlePattern" },
                { "z_1_kb_41", "#img_TitleDecor" },
                { "img_txt", "#img_TitleText" },
                { "h", "#img_TitleGlow" },

                // === 底部幻灵切换区 panel_bottom ===
                { "panel_bottom", "HeroSwitchPanel" },
                { "panel_model", "HeroModelPanel" },
                { "img_tx", "#img_HeroAvatar" },
                { "img_xz", "#img_HeroSelected" },
                { "ar", "#lbl_HeroName" },
            };

            // 收集所有 Transform（广度优先）
            var allTransforms = prefabRoot.GetComponentsInChildren<Transform>(true);

            // 执行重命名
            int renamedCount = 0;
            foreach (var t in allTransforms)
            {
                if (renameMap.TryGetValue(t.name, out string newName))
                {
                    string oldName = t.name;
                    t.name = newName;
                    renamedCount++;
                    Debug.Log($"重命名: {oldName} -> {newName}");
                }
            }

            // 保存 prefab
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Debug.Log($"幻灵降临 Prefab 节点重命名完成，共重命名 {renamedCount} 个节点");
        }
        finally
        {
            // 卸载 prefab
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.Refresh();
    }
}
