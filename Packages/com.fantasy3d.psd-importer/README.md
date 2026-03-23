# PSD to UGUI Importer

将 PSD 导出的 JSON + 切图资源自动生成 Unity UGUI Prefab 的编辑器工具。

## 功能

- 导入 PSD 导出的 JSON 布局数据和切图资源
- 自动生成 UGUI Prefab（保留层级结构、位置、尺寸）
- 支持九宫格（9-Slice）图片
- 自动识别并生成 Button 组件
- 集成 TexturePacker 图集打包
- 支持图集增量更新
- 可配置纹理压缩参数（Android/iOS）

## 安装

### 方式一：本地 Package

将此文件夹放到项目的 `Packages/` 目录下，Unity 会自动识别。

### 方式二：Git URL

在 `Packages/manifest.json` 中添加：

```json
{
  "dependencies": {
    "com.fantasy3d.psd-importer": "https://your-git-repo.git#v1.0.0"
  }
}
```

## 依赖

- Unity 2019.4+
- Newtonsoft.Json（`com.unity.nuget.newtonsoft-json`）
- TexturePacker（图集功能需要，可选）

## 使用

### 第一步：从 Photoshop 导出

1. 在 Photoshop 中打开 PSD 文件
2. 菜单 `文件 > 脚本 > 浏览...` 选择 `PhotoshopExporter/PSD2Unity.jsx`
3. 选择一个导出目录
4. 脚本会生成：
   - `[PSD文件名].json` — 布局数据
   - `images/` — 所有切图 PNG

### 第二步：在 Unity 中导入

1. Unity 菜单 `Tools > PSD Importer` 打开工具窗口
2. 点击 "浏览" 选择上一步导出的 JSON 文件（图片文件夹会自动填充）
3. 设置功能名称和 Prefab 名称
4. 点击 "导入资源" 将切图导入 Unity
5. 点击 "生成UGUI Prefab" 创建 Prefab
6. 点击 "生成图集" 将小图打包为图集（需要 TexturePacker，可选）

### PSD 图层命名规范

| 命名方式 | 效果 |
|----------|------|
| 普通名称 | 导出为 PNG 图片 |
| 文本图层 | 自动识别为 Text 组件 |
| `txt_` 开头 | 强制识别为文本 |
| 图层组 | 识别为空节点容器 |
| 包含 `[9]` | 标记为九宫格（自动计算边距） |
| `[9:10,10,10,10]` | 九宫格 + 自定义边距 (left,top,right,bottom) |
| `_` 开头 | 跳过不导出 |

## JSON 格式

工具期望的 JSON 结构：

```json
{
  "CanvasSize": { "Width": 1920, "Height": 1080 },
  "InfoList": [
    {
      "Name": "节点名",
      "Type": "png|text|node",
      "Tree": "父节点/子节点",
      "FilePath": "图片文件名.png",
      "Pos": { "X": 100, "Y": 200 },
      "Size": { "Width": 300, "Height": 150 },
      "IsNineSlice": false,
      "Left": 0, "Top": 0, "Right": 0, "Bottom": 0,
      "Content": "文本内容",
      "FontSize": 24,
      "TextColor": { "Red": 255, "Green": 255, "Blue": 255 },
      "Alignment": "MiddleCenter"
    }
  ],
  "DuplicateMap": {}
}
```

## 配置

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| 缩放比例 | PSD 到 Unity 的缩放系数 | 1.0 |
| X/Y 偏移 | 根节点位置偏移 | 0 |
| 自动生成按钮 | 根据命名规则自动添加 Button | 关闭 |
| 按钮关键字 | 触发按钮生成的名称关键字 | btn,button |
| 纹理压缩 | 是否启用纹理压缩 | 开启 |
| 最大纹理尺寸 | 导入纹理的最大尺寸 | 2048 |
