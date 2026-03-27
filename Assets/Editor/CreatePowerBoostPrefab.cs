using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CreatePowerBoostPrefab
{
    // 效果图分辨率 1920x1080，以中心为原点
    public static void Execute()
    {
        // 创建根 Canvas GameObject
        var root = new GameObject("PowerBoostPanel");
        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        // === 弹窗主面板（居中） ===
        var panel = CreateChild(root, "Panel", 0, 0, 1200, 900);

        // === 标题: "威力助战·横扫全场" ===
        // 位置：面板顶部居中，约 y=390 (相对面板中心)
        var title = CreateChild(panel, "TxtTitle", 0, 370, 700, 80);
        var titleText = title.AddComponent<Text>();
        titleText.text = "威力助战·横扫全场";
        titleText.fontSize = 52;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.85f, 0.2f, 1f); // 金色
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === 关闭按钮 "X" ===
        // 位置：右上角，约 (540, 380) 相对面板中心
        var closeBtn = CreateChild(panel, "BtnClose", 540, 380, 60, 60);
        closeBtn.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        closeBtn.AddComponent<Button>();
        var closeTxt = CreateChild(closeBtn, "Text", 0, 0, 60, 60);
        var ct = closeTxt.AddComponent<Text>();
        ct.text = "X";
        ct.fontSize = 36;
        ct.alignment = TextAnchor.MiddleCenter;
        ct.color = Color.white;
        ct.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === 金色拱形装饰框背景 ===
        // 覆盖面板大部分区域
        var archBg = CreateChild(panel, "ImgArchFrame", 0, 20, 1100, 750);
        archBg.AddComponent<Image>().color = new Color(0.8f, 0.6f, 0.1f, 0.3f);

        // === 中央紫色/星空背景区域 ===
        var centerBg = CreateChild(panel, "ImgCenterBg", 0, 50, 800, 500);
        centerBg.AddComponent<Image>().color = new Color(0.2f, 0.1f, 0.4f, 0.8f);

        // === 中央角色/吉祥物（雪人） ===
        // 位置：面板中心偏上
        var mascot = CreateChild(panel, "ImgMascot", 0, 120, 220, 260);
        mascot.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.9f);

        // === 4张卡片容器 ===
        var cardsContainer = CreateChild(panel, "CardsContainer", 0, -20, 1000, 380);

        // 卡片1: 雪绒圣歌（左侧）位置约 x=-380
        CreateCard(cardsContainer, "Card_XueRongShengGe", -380, 0, 200, 320,
            "雪绒圣歌", "永久", "1177038750");

        // 卡片2: 金币（中左）位置约 x=-140
        CreateCard(cardsContainer, "Card_JinBi", -140, 0, 200, 320,
            "金币", "金币", "剩余1177038750");

        // 卡片3: 锁定（中右）位置约 x=140
        CreateCard(cardsContainer, "Card_SuoDing", 140, 20, 200, 320,
            "锁定卡", "锁定", "");

        // 卡片4: 冰冻（右侧）位置约 x=380
        CreateCard(cardsContainer, "Card_BingDong", 380, 0, 200, 320,
            "冰冻", "冰冻", "");

        // === 展示台/舞台（底部金色圆台） ===
        var stage = CreateChild(panel, "ImgStage", 0, -200, 900, 180);
        stage.AddComponent<Image>().color = new Color(0.85f, 0.65f, 0.15f, 0.6f);

        // === 超值专享标签 "低至1折" ===
        var discountTag = CreateChild(panel, "ImgDiscountTag", 160, -280, 140, 60);
        discountTag.AddComponent<Image>().color = new Color(0.4f, 0.2f, 0.8f, 0.9f);
        var discountTxt = CreateChild(discountTag, "Text", 0, 0, 130, 50);
        var dt = discountTxt.AddComponent<Text>();
        dt.text = "超值专享\n低至1折";
        dt.fontSize = 18;
        dt.alignment = TextAnchor.MiddleCenter;
        dt.color = Color.white;
        dt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === 购买按钮 "6.8元 限购2次" ===
        var buyBtn = CreateChild(panel, "BtnBuy", 0, -380, 360, 80);
        var buyImg = buyBtn.AddComponent<Image>();
        buyImg.color = new Color(0.1f, 0.8f, 0.9f, 0.9f); // 青色渐变
        buyBtn.AddComponent<Button>();

        var priceTxt = CreateChild(buyBtn, "TxtPrice", 0, 5, 300, 40);
        var pt = priceTxt.AddComponent<Text>();
        pt.text = "6.8元";
        pt.fontSize = 36;
        pt.alignment = TextAnchor.MiddleCenter;
        pt.color = Color.white;
        pt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var limitTxt = CreateChild(buyBtn, "TxtLimit", 0, -25, 300, 30);
        var lt = limitTxt.AddComponent<Text>();
        lt.text = "限购2次";
        lt.fontSize = 20;
        lt.alignment = TextAnchor.MiddleCenter;
        lt.color = new Color(1f, 1f, 1f, 0.8f);
        lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === 特殊加成标签 "特殊加成 0" ===
        var bonusTag = CreateChild(panel, "BonusTag", -80, 130, 140, 36);
        bonusTag.AddComponent<Image>().color = new Color(0.1f, 0.6f, 0.1f, 0.9f);
        var bonusTxt = CreateChild(bonusTag, "Text", 0, 0, 130, 30);
        var bt = bonusTxt.AddComponent<Text>();
        bt.text = "特殊加成  0";
        bt.fontSize = 18;
        bt.alignment = TextAnchor.MiddleCenter;
        bt.color = Color.white;
        bt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 保存为 Prefab
        string prefabPath = "Assets/Prefabs/PowerBoostPanel.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);

        AssetDatabase.Refresh();
        Debug.Log("PowerBoostPanel Prefab 已创建: " + prefabPath);
    }

    static GameObject CreateChild(GameObject parent, string name, float x, float y, float w, float h)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
        return go;
    }

    static void CreateCard(GameObject parent, string name, float x, float y, float w, float h,
        string cardName, string labelText, string numberText)
    {
        // 卡片容器
        var card = CreateChild(parent, name, x, y, w, h);
        var cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.95f, 0.9f, 0.75f, 0.95f); // 米黄色卡片底色

        // 卡片图标区域（上半部分）
        var icon = CreateChild(card, "ImgIcon", 0, 40, 160, 180);
        icon.AddComponent<Image>().color = new Color(0.9f, 0.85f, 0.7f, 0.5f);

        // 卡片底部标签（红色底条）
        var label = CreateChild(card, "ImgLabel", 0, -120, 180, 50);
        label.AddComponent<Image>().color = new Color(0.85f, 0.15f, 0.1f, 0.95f);

        var labelTxtGo = CreateChild(label, "Text", 0, 0, 170, 40);
        var labelTxtComp = labelTxtGo.AddComponent<Text>();
        labelTxtComp.text = labelText;
        labelTxtComp.fontSize = 28;
        labelTxtComp.alignment = TextAnchor.MiddleCenter;
        labelTxtComp.color = Color.white;
        labelTxtComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 卡片名称（底部）
        var nameTxtGo = CreateChild(card, "TxtName", 0, -155, 180, 30);
        var nameTxtComp = nameTxtGo.AddComponent<Text>();
        nameTxtComp.text = cardName;
        nameTxtComp.fontSize = 22;
        nameTxtComp.alignment = TextAnchor.MiddleCenter;
        nameTxtComp.color = new Color(1f, 0.9f, 0.6f, 1f);
        nameTxtComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 数字标签（如果有）
        if (!string.IsNullOrEmpty(numberText))
        {
            var numTag = CreateChild(card, "TxtNumber", 0, 100, 180, 28);
            var numTxtComp = numTag.AddComponent<Text>();
            numTxtComp.text = numberText;
            numTxtComp.fontSize = 16;
            numTxtComp.alignment = TextAnchor.MiddleCenter;
            numTxtComp.color = new Color(1f, 0.85f, 0.2f, 1f);
            numTxtComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }
}
