using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// 吉时将至 签到活动面板 - 基于原型图精确还原
/// 设计分辨率: 1280x720
/// </summary>
public class CreateJiShiJiangZhi_1280x720
{
    static Font font;

    [MenuItem("Tools/创建吉时将至面板(1280x720)")]
    public static void Execute()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var old = GameObject.Find("JiShiJiangZhiCanvas_720");
        if (old != null) Object.DestroyImmediate(old);

        // ===== Canvas 根节点 =====
        var root = new GameObject("JiShiJiangZhiCanvas_720");
        var c = root.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = root.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1280, 720);
        sc.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        // 遮罩层
        var mask = MkRT(root, "Mask", 0, 0, 0, 0);
        Stretch(mask);
        mask.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        // ===== 主面板 1200x650 居中 =====
        var mp = MkRT(mask, "MainPanel", 0, 0, 1200, 650);
        mp.AddComponent<Image>().color = Hex("#EBEBEB");

        // ===== 关闭按钮 右上角 =====
        var btnClose = MkAnchor(mp, "#btn_Close", 1, 1, -20, -20, 36, 36);
        btnClose.AddComponent<Image>().color = Hex("#DDDDDD");
        btnClose.AddComponent<Button>();
        var closeTx = MkRT(btnClose, "#lbl_Close", 0, 0, 36, 36); Stretch(closeTx);
        var ct = closeTx.AddComponent<Text>(); ct.text = "X"; ct.fontSize = 22;
        ct.alignment = TextAnchor.MiddleCenter; ct.color = Hex("#666666"); ct.font = font;

        // ===== 标题区 左上角 =====
        MkTxtLT(mp, "#lbl_Title", 20, -18, 160, 36, "吉时将至", 28, "#333333", TextAnchor.MiddleLeft);
        MkBtnLT(mp, "#btn_Help", 190, -22, 28, 28, "?", 16, "#FFFFFF", "#888888");
        MkTxtLT(mp, "#lbl_Time", 20, -55, 240, 20, "活动时间：x月x日-x月x日", 14, "#999999", TextAnchor.MiddleLeft);

        // ===== 顶部高级奖励栏 =====
        var topBar = MkAnchor(mp, "TopRewardBar", 0, 1, 200, -80, 640, 55);

        // 高级奖励（大图标）
        var mainRwd = MkImg(topBar, "#img_MainReward", -250, 0, 80, 50, "#D0D0D0");
        MkTxt(mainRwd, "#lbl_MainReward", 0, 0, 70, 40, "高级\n奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);

        // 4个普通奖励图标
        for (int i = 0; i < 4; i++)
        {
            float rx = -130 + i * 80;
            var rImg = MkImg(topBar, "#img_TopReward" + (i + 1), rx, 0, 60, 45, "#D0D0D0");
            MkTxt(rImg, "#lbl_TopReward" + (i + 1), 0, 0, 50, 36, "奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);
        }

        // 更多按钮
        MkBtn(topBar, "#btn_More", 210, 0, 60, 45, "更多", 14, "#FFFFFF", "#3388DD");

        // ===== 7日签到区域 =====
        // 签到区域从左上角 (20, -140) 开始，宽度约 860，高度约 460
        var sa = MkAnchor(mp, "SignInArea", 0, 1, 20, -140, 860, 460);

        // 每天面板: 宽115, 间距8, 高度460
        string[] dayTexts = { "第1日", "第2日", "第3日", "第4日", "第5日", "第6日", "第7日" };
        string[] signBtnTexts = { "签到开启", "已领取", "补签", "日期未至", "日期未至", "日期未至", "日期未至" };
        string[] signBtnBg = { "#FF8800", "#AAAAAA", "#FF4444", "#FFCC00", "#AAAAAA", "#AAAAAA", "#AAAAAA" };
        string[] signBtnTc = { "#FFFFFF", "#666666", "#FFFFFF", "#333333", "#666666", "#666666", "#666666" };
        float dayW = 115f;
        float dayGap = 8f;
        float dayH = 450f;

        for (int i = 0; i < 7; i++)
        {
            float px = i * (dayW + dayGap);
            var day = MkAnchor(sa, "Day" + (i + 1) + "Panel", 0, 1, px, 0, dayW, dayH);

            // 天数标题
            string dayCol = i == 6 ? "#FF6600" : "#333333";
            MkTxtTop(day, "#lbl_Day" + (i + 1), 0, -5, 100, 26, dayTexts[i], 18, dayCol, TextAnchor.MiddleCenter);

            // 主奖励图标区（问号占位）
            var rwBox = MkAnchorImg(day, "#img_RewardBox" + (i + 1), 0.5f, 1, 0, -38, 80, 80, "#E5E5E5");
            MkTxt(rwBox, "#lbl_Q" + (i + 1), 0, 0, 40, 40, "?", 24, "#BBBBBB", TextAnchor.MiddleCenter);

            // 签到/状态按钮
            MkBtnTop(day, "#btn_Sign" + (i + 1), 0, -125, 95, 28, signBtnTexts[i], 13, signBtnTc[i], signBtnBg[i]);

            // 高级奖励标签
            var advLabel = MkAnchorImg(day, "#img_AdvLabel" + (i + 1), 0.5f, 1, 0, -160, 105, 20, "#FF8800");
            MkTxt(advLabel, "#lbl_AdvLabel" + (i + 1), 0, 0, 100, 18, "高级奖励", 11, "#FFFFFF", TextAnchor.MiddleCenter);

            // ===== 底部区域：奖励图标在上，操作按钮在下 =====
            // 从面板底部往上排列，使用 bottom anchor
            if (i == 0)
            {
                // 第1日: 2个奖励图标 + "已领取"按钮
                MkAnchorImg(day, "#img_AdvIcon1_1", 0.5f, 0, 0, 80, 48, 48, "#E0E0E0");
                MkAnchorImg(day, "#img_AdvIcon1_2", 0.5f, 0, 0, 135, 48, 48, "#E0E0E0");
                MkBtnBottom(day, "#btn_Claim1", 0, 22, 80, 26, "已领取", 12, "#999999", "#DDDDDD");
            }
            else if (i == 1)
            {
                // 第2日: 2个奖励图标 + "领取"按钮
                MkAnchorImg(day, "#img_AdvIcon2_1", 0.5f, 0, 0, 80, 48, 48, "#E0E0E0");
                MkAnchorImg(day, "#img_AdvIcon2_2", 0.5f, 0, 0, 135, 48, 48, "#E0E0E0");
                MkBtnBottom(day, "#btn_Claim2", 0, 22, 80, 26, "领取", 12, "#FFFFFF", "#FF8800");
            }
            else if (i == 2)
            {
                // 第3日: 2个奖励图标 + 条件 + 进度 + "前往"按钮
                MkAnchorImg(day, "#img_AdvIcon3_1", 0.5f, 0, 0, 148, 48, 48, "#E0E0E0");
                MkAnchorImg(day, "#img_AdvIcon3_2", 0.5f, 0, 0, 203, 48, 48, "#E0E0E0");
                MkTxtBottom(day, "#lbl_Cond3", 0, 95, 110, 36, "100万炮倍以上\n捕获百条鱼", 11, "#666666", TextAnchor.MiddleCenter);
                MkTxtBottom(day, "#lbl_Prog3", 0, 55, 70, 22, "0/XX", 14, "#333333", TextAnchor.MiddleCenter);
                MkBtnBottom(day, "#btn_Go3", 0, 22, 70, 26, "前往", 13, "#FFFFFF", "#FF8800");
            }
            else if (i == 3)
            {
                // 第4日: 2个奖励图标 + 日期未至 + 倒计时
                MkAnchorImg(day, "#img_AdvIcon4_1", 0.5f, 0, 0, 100, 48, 48, "#E0E0E0");
                MkAnchorImg(day, "#img_AdvIcon4_2", 0.5f, 0, 0, 155, 48, 48, "#E0E0E0");
                MkTxtBottom(day, "#lbl_Wait4", 0, 50, 100, 22, "日期未至", 12, "#999999", TextAnchor.MiddleCenter);
                MkTxtBottom(day, "#lbl_Timer4", 0, 22, 100, 22, "XX:XX:XX", 14, "#333333", TextAnchor.MiddleCenter);
            }
            else
            {
                // 第5-7日: 2个奖励图标 + 日期未至
                MkAnchorImg(day, "#img_AdvIcon" + (i+1) + "_1", 0.5f, 0, 0, 80, 48, 48, "#E0E0E0");
                MkAnchorImg(day, "#img_AdvIcon" + (i+1) + "_2", 0.5f, 0, 0, 135, 48, 48, "#E0E0E0");
                MkTxtBottom(day, "#lbl_Wait" + (i + 1), 0, 22, 100, 22, "日期未至", 12, "#999999", TextAnchor.MiddleCenter);
            }
        }

        // ===== 右侧任务面板 =====
        // 右侧面板宽280，从右边距20开始
        var rp = MkAnchor(mp, "RightPanel", 1, 1, -20, -140, 280, 460);
        rp.AddComponent<Image>().color = Color.white;

        // 到手金额
        var goldBg = MkAnchorImg(rp, "#img_GoldBg", 0.5f, 1, 0, -10, 260, 40, "#FFF0E0");
        MkTxt(goldBg, "#lbl_Gold", 0, 0, 250, 36, "到手 89960亿", 24, "#FF4400", TextAnchor.MiddleCenter);

        // 额外赠送标题
        MkTxtTop(rp, "#lbl_Extra", 0, -58, 160, 20, "额外赠送", 13, "#666666", TextAnchor.MiddleCenter);

        // 奖励图标 2x2 网格
        var grid = MkAnchor(rp, "RewardGrid", 0.5f, 1, 0, -82, 260, 100, 0.5f, 1);
        var ga1 = MkImg(grid, "#img_ExtraA1", -66, 24, 120, 44, "#F2F2F2");
        MkTxt(ga1, "#lbl_ExtraA1", 0, 0, 100, 36, "奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);
        var ga2 = MkImg(grid, "#img_ExtraA2", 66, 24, 120, 44, "#F2F2F2");
        MkTxt(ga2, "#lbl_ExtraA2", 0, 0, 100, 36, "奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);
        var gb1 = MkImg(grid, "#img_ExtraB1", -66, -24, 120, 44, "#F2F2F2");
        MkTxt(gb1, "#lbl_ExtraB1", 0, 0, 100, 36, "奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);
        var gb2 = MkImg(grid, "#img_ExtraB2", 66, -24, 120, 44, "#F2F2F2");
        MkTxt(gb2, "#lbl_ExtraB2", 0, 0, 100, 36, "奖励\n图标", 11, "#888888", TextAnchor.MiddleCenter);

        // 投资金额
        var vipBg = MkAnchorImg(rp, "#img_VipBg", 0.5f, 1, 0, -195, 170, 24, "#FFE0E0");
        MkTxt(vipBg, "#lbl_Vip", 0, 0, 160, 22, "投资 63960亿", 14, "#FF4400", TextAnchor.MiddleCenter);

        // 点券指标按钮
        MkBtnTop(rp, "#btn_Task", 0, -228, 180, 30, "点券指标 XXXX", 13, "#FFFFFF", "#FF8800");

        // 分割线
        MkAnchorImg(rp, "#img_Divider", 0.5f, 1, 0, -268, 240, 1, "#E0E0E0");

        // 描述文本
        MkTxtTop(rp, "#lbl_Desc", 0, -278, 250, 48, "完成每日任务,礼包金币和道具数量提\n升100%,最高提升700%", 12, "#333333", TextAnchor.MiddleCenter);

        // 提示文本
        MkTxtTop(rp, "#lbl_Hint", 0, -335, 230, 40, "提前购买,额外收益可在\n完成任务后领取", 12, "#3388DD", TextAnchor.MiddleCenter);

        // ===== 底部提示栏 =====
        var bot = MkAnchor(mp, "BottomBar", 0.5f, 0, 0, 15, 1160, 38);
        bot.AddComponent<Image>().color = Hex("#E0E0E0");
        MkTxt(bot, "#lbl_Bottom", 0, 0, 1100, 34,
            "签到领奖励,完成任务可抽取高级奖励,任意充值1笔点券,可免费开启补签功能!", 15, "#333333", TextAnchor.MiddleCenter);

        // ===== 保存 Prefab =====
        string path = "Assets/Prefabs/JiShiJiangZhi_720.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        AssetDatabase.Refresh();
        Debug.Log("吉时将至 Prefab (1280x720) 已创建: " + path);
    }

    // ===== 工具方法 =====
    static GameObject MkRT(GameObject p, string n, float x, float y, float w, float h)
    {
        var g = new GameObject(n);
        g.transform.SetParent(p.transform, false);
        var r = g.AddComponent<RectTransform>();
        r.anchoredPosition = new Vector2(x, y);
        r.sizeDelta = new Vector2(w, h);
        return g;
    }

    static GameObject MkAnchor(GameObject p, string n, float ax, float ay, float px, float py, float w, float h)
    {
        var g = new GameObject(n);
        g.transform.SetParent(p.transform, false);
        var r = g.AddComponent<RectTransform>();
        r.anchorMin = new Vector2(ax, ay);
        r.anchorMax = new Vector2(ax, ay);
        r.pivot = new Vector2(ax, ay);
        r.anchoredPosition = new Vector2(px, py);
        r.sizeDelta = new Vector2(w, h);
        return g;
    }

    static GameObject MkAnchor(GameObject p, string n, float ax, float ay, float px, float py, float w, float h, float pvx, float pvy)
    {
        var g = new GameObject(n);
        g.transform.SetParent(p.transform, false);
        var r = g.AddComponent<RectTransform>();
        r.anchorMin = new Vector2(ax, ay);
        r.anchorMax = new Vector2(ax, ay);
        r.pivot = new Vector2(pvx, pvy);
        r.anchoredPosition = new Vector2(px, py);
        r.sizeDelta = new Vector2(w, h);
        return g;
    }

    static void Stretch(GameObject g)
    {
        var r = g.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
    }

    static GameObject MkTxt(GameObject p, string n, float x, float y, float w, float h, string t, int s, string col, TextAnchor a)
    {
        var g = MkRT(p, n, x, y, w, h);
        var tx = g.AddComponent<Text>(); tx.text = t; tx.fontSize = s; tx.alignment = a; tx.color = Hex(col); tx.font = font;
        return g;
    }

    static GameObject MkTxtLT(GameObject p, string n, float x, float y, float w, float h, string t, int s, string col, TextAnchor a)
    {
        var g = MkAnchor(p, n, 0, 1, x, y, w, h);
        var tx = g.AddComponent<Text>(); tx.text = t; tx.fontSize = s; tx.alignment = a; tx.color = Hex(col); tx.font = font;
        return g;
    }

    static GameObject MkTxtTop(GameObject p, string n, float x, float y, float w, float h, string t, int s, string col, TextAnchor a)
    {
        var g = MkAnchor(p, n, 0.5f, 1, x, y, w, h, 0.5f, 1);
        var tx = g.AddComponent<Text>(); tx.text = t; tx.fontSize = s; tx.alignment = a; tx.color = Hex(col); tx.font = font;
        return g;
    }

    static GameObject MkImg(GameObject p, string n, float x, float y, float w, float h, string col)
    {
        var g = MkRT(p, n, x, y, w, h);
        g.AddComponent<Image>().color = Hex(col);
        return g;
    }

    static GameObject MkAnchorImg(GameObject p, string n, float ax, float ay, float px, float py, float w, float h, string col)
    {
        var g = MkAnchor(p, n, ax, ay, px, py, w, h, 0.5f, ay);
        g.AddComponent<Image>().color = Hex(col);
        return g;
    }

    static GameObject MkBtn(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkRT(p, n, x, y, w, h);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#lbl_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static GameObject MkBtnLT(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkAnchor(p, n, 0, 1, x, y, w, h);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#lbl_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static GameObject MkBtnTop(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkAnchor(p, n, 0.5f, 1, x, y, w, h, 0.5f, 1);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#lbl_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    // 底部锚点文本（anchor bottom-center, pivot bottom-center）
    static GameObject MkTxtBottom(GameObject p, string n, float x, float y, float w, float h, string t, int s, string col, TextAnchor a)
    {
        var g = MkAnchor(p, n, 0.5f, 0, x, y, w, h, 0.5f, 0);
        var tx = g.AddComponent<Text>(); tx.text = t; tx.fontSize = s; tx.alignment = a; tx.color = Hex(col); tx.font = font;
        return g;
    }

    // 底部锚点按钮（anchor bottom-center, pivot bottom-center）
    static GameObject MkBtnBottom(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkAnchor(p, n, 0.5f, 0, x, y, w, h, 0.5f, 0);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#lbl_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static Color Hex(string h) { Color co; ColorUtility.TryParseHtmlString(h, out co); return co; }
}
