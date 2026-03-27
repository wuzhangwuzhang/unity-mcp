using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CreateJiShiJiangZhiPrefab
{
    static Font font;

    public static void Execute()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var old = GameObject.Find("JiShiJiangZhiCanvas");
        if (old != null) Object.DestroyImmediate(old);

        var root = new GameObject("JiShiJiangZhiCanvas");
        var c = root.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = root.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920, 1080);
        sc.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        // 遮罩层
        var mask = MkRT(root, "Mask", 0, 0, 0, 0);
        Stretch(mask);
        mask.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        // 主面板
        var mp = MkRT(mask, "MainPanel", 0, 0, 1780, 980);
        mp.AddComponent<Image>().color = Hex("#EBEBEB");

        // 关闭按钮
        var btnClose = MkAnchor(mp, "#btn_Close", 1, 1, -30, -30, 48, 48);
        btnClose.AddComponent<Image>().color = Hex("#DDDDDD");
        btnClose.AddComponent<Button>();
        var closeTx = MkRT(btnClose, "#txt_Close", 0, 0, 48, 48); Stretch(closeTx);
        var ct = closeTx.AddComponent<Text>(); ct.text="X"; ct.fontSize=28;
        ct.alignment=TextAnchor.MiddleCenter; ct.color=Hex("#666666"); ct.font=font;

        // 标题区
        MkTxtLT(mp, "#txt_Title", 30, -30, 220, 48, "吉时将至", 36, "#333333", TextAnchor.MiddleLeft);
        MkBtnLT(mp, "#btn_Help", 270, -36, 36, 36, "?", 20, "#FFFFFF", "#888888");
        MkTxtLT(mp, "#txt_Time", 30, -75, 300, 24, "活动时间：x月x日-x月x日", 18, "#999999", TextAnchor.MiddleLeft);

        // 顶部奖励栏
        var topBar = MkAnchor(mp, "TopRewardBar", 0, 1, 250, -105, 900, 65);
        MkImg(topBar, "#img_MainReward", -350, 0, 100, 55, "#D0D0D0");
        MkImg(topBar, "#img_Reward1", -210, 0, 70, 50, "#D0D0D0");
        MkImg(topBar, "#img_Reward2", -110, 0, 70, 50, "#D0D0D0");
        MkImg(topBar, "#img_Reward3", -10, 0, 70, 50, "#D0D0D0");
        MkImg(topBar, "#img_Reward4", 90, 0, 70, 50, "#D0D0D0");
        MkBtn(topBar, "#btn_More", 200, 0, 70, 50, "更多", 18, "#FFFFFF", "#3388DD");

        // ========== 7日签到区域 ==========
        var sa = MkAnchor(mp, "SignInArea", 0, 1, 30, -180, 1300, 730);

        string[] dayTexts = {"第1日","第2日","第3日","第4日","第5日","第6日","第7日"};
        string[] btnTexts = {"签到开启","已领取","补签","日期未至","日期未至","日期未至","日期未至"};
        string[] btnBg = {"#FF8800","#AAAAAA","#FF4444","#FFCC00","#AAAAAA","#AAAAAA","#AAAAAA"};
        string[] btnTc = {"#FFFFFF","#666666","#FFFFFF","#333333","#666666","#666666","#666666"};
        float dayW = 155f;
        float gap = 20f;

        for (int i = 0; i < 7; i++)
        {
            float px = i * (dayW + gap);
            var day = MkAnchor(sa, "Day" + (i+1) + "Panel", 0, 1, px, 0, dayW, 680);

            MkTxt(day, "#txt_Day", 0, 240, 130, 30, dayTexts[i], 22, i == 6 ? "#FF6600" : "#333333", TextAnchor.MiddleCenter);
            var rw = MkImg(day, "#img_Reward", 0, 160, 100, 100, "#E5E5E5");
            MkTxt(rw, "#txt_Q", 0, 0, 40, 40, "?", 28, "#BBBBBB", TextAnchor.MiddleCenter);
            MkBtn(day, "#btn_Sign", 0, 85, 110, 34, btnTexts[i], 15, btnTc[i], btnBg[i]);
            var adv = MkImg(day, "#img_AdvLabel", 0, 50, 130, 24, "#FF8800");
            MkTxt(adv, "#txt_AdvLabel", 0, 0, 120, 22, "高级奖励", 13, "#FFFFFF", TextAnchor.MiddleCenter);

            if (i == 0)
            {
                MkImg(day, "#img_Adv1", 0, -10, 55, 55, "#E0E0E0");
                MkImg(day, "#img_Adv2", 0, -80, 55, 55, "#E0E0E0");
                MkTxt(day, "#txt_Claimed", -30, -145, 70, 22, "已领取", 13, "#999999", TextAnchor.MiddleCenter);
                MkBtn(day, "#btn_Claim", 40, -145, 65, 28, "领取", 14, "#FFFFFF", "#FF8800");
            }
            else if (i == 1)
            {
                MkTxt(day, "#txt_Q2", 0, -30, 40, 40, "?", 28, "#BBBBBB", TextAnchor.MiddleCenter);
            }
            else if (i == 2)
            {
                MkTxt(day, "#txt_Cond", 0, -10, 135, 44, "100万炮倍以上\n捕获百条鱼", 13, "#666666", TextAnchor.MiddleCenter);
                MkTxt(day, "#txt_Prog", 0, -65, 80, 24, "0/XX", 16, "#333333", TextAnchor.MiddleCenter);
                MkBtn(day, "#btn_Go", 0, -105, 80, 30, "前往", 15, "#FFFFFF", "#FF8800");
            }
            else if (i == 3)
            {
                MkTxt(day, "#txt_Wait", 0, -20, 120, 24, "日期未至", 14, "#999999", TextAnchor.MiddleCenter);
                MkTxt(day, "#txt_Timer", 0, -55, 120, 24, "XX:XX:XX", 16, "#333333", TextAnchor.MiddleCenter);
            }
            else
            {
                MkTxt(day, "#txt_Wait", 0, -20, 120, 24, "日期未至", 14, "#999999", TextAnchor.MiddleCenter);
            }
        }

        // ========== 右侧任务面板 ==========
        var rp = MkAnchor(mp, "RightPanel", 1, 1, -30, -170, 340, 730);
        rp.AddComponent<Image>().color = Color.white;

        var goldBg = MkAnchor(rp, "#img_GoldBg", 0.5f, 1, 0, -15, 310, 48, 0.5f, 1);
        goldBg.AddComponent<Image>().color = Hex("#FFF0E0");
        MkTxt(goldBg, "#txt_Gold", 0, 0, 300, 44, "到手 89960亿", 30, "#FF4400", TextAnchor.MiddleCenter);

        MkTxtTop(rp, "#txt_Extra", 0, -70, 200, 24, "额外赠送", 16, "#666666", TextAnchor.MiddleCenter);

        var grid = MkAnchor(rp, "RewardGrid", 0.5f, 1, 0, -100, 310, 120, 0.5f, 1);
        var ra1 = MkImg(grid, "#img_RewardA1", -80, 28, 135, 50, "#F2F2F2");
        MkTxt(ra1, "#txt_RewardA1", 0, 0, 120, 40, "奖励图标", 13, "#888888", TextAnchor.MiddleCenter);
        var ra2 = MkImg(grid, "#img_RewardA2", 80, 28, 135, 50, "#F2F2F2");
        MkTxt(ra2, "#txt_RewardA2", 0, 0, 120, 40, "奖励图标", 13, "#888888", TextAnchor.MiddleCenter);
        var rb1 = MkImg(grid, "#img_RewardB1", -80, -28, 135, 50, "#F2F2F2");
        MkTxt(rb1, "#txt_RewardB1", 0, 0, 120, 40, "奖励图标", 13, "#888888", TextAnchor.MiddleCenter);
        var rb2 = MkImg(grid, "#img_RewardB2", 80, -28, 135, 50, "#F2F2F2");
        MkTxt(rb2, "#txt_RewardB2", 0, 0, 120, 40, "奖励图标", 13, "#888888", TextAnchor.MiddleCenter);

        var vipBg = MkAnchor(rp, "#img_VipBg", 0.5f, 1, 0, -235, 200, 28, 0.5f, 1);
        vipBg.AddComponent<Image>().color = Hex("#FFE0E0");
        MkTxt(vipBg, "#txt_Vip", 0, 0, 190, 26, "投资 63960亿", 17, "#FF4400", TextAnchor.MiddleCenter);

        MkBtnTop(rp, "#btn_Task", 0, -272, 210, 36, "点券指标 XXXX", 16, "#FFFFFF", "#FF8800");
        MkAnchorImg(rp, "#img_Divider", 0.5f, 1, 0, -318, 280, 1, "#E0E0E0");
        MkTxtTop(rp, "#txt_Desc", 0, -330, 290, 55, "完成每日任务,礼包金币和道具数量提\n升100%,最高提升700%", 14, "#333333", TextAnchor.MiddleCenter);
        MkTxtTop(rp, "#txt_Hint", 0, -395, 270, 45, "提前购买,额外收益可在\n完成任务后领取", 14, "#3388DD", TextAnchor.MiddleCenter);

        // ========== 底部提示栏 ==========
        var bot = MkAnchor(mp, "BottomBar", 0.5f, 0, 0, 30, 1700, 48);
        bot.AddComponent<Image>().color = Hex("#E0E0E0");
        MkTxt(bot, "#txt_Bottom", 0, 0, 1600, 40,
            "签到领奖励,完成任务可抽取高级奖励,任意充值1笔点券,可免费开启补签功能!", 18, "#333333", TextAnchor.MiddleCenter);

        // 保存Prefab
        string path = "Assets/Prefabs/JiShiJiangZhi.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        AssetDatabase.Refresh();
        Debug.Log("吉时将至 Prefab 已创建: " + path);
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
        var tx = MkRT(g, "#txt_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static GameObject MkBtnLT(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkAnchor(p, n, 0, 1, x, y, w, h);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#txt_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static GameObject MkBtnTop(GameObject p, string n, float x, float y, float w, float h, string t, int s, string tc, string bc)
    {
        var g = MkAnchor(p, n, 0.5f, 1, x, y, w, h, 0.5f, 1);
        g.AddComponent<Image>().color = Hex(bc); g.AddComponent<Button>();
        var tx = MkRT(g, "#txt_Label", 0, 0, w, h); Stretch(tx);
        var tt = tx.AddComponent<Text>(); tt.text = t; tt.fontSize = s; tt.alignment = TextAnchor.MiddleCenter; tt.color = Hex(tc); tt.font = font;
        return g;
    }

    static Color Hex(string h) { Color co; ColorUtility.TryParseHtmlString(h, out co); return co; }
}
