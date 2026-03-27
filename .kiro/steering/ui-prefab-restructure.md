---
inclusion: manual
---

# Unity UI Prefab 重构与重命名工作流

将已有 Prefab 根据策划文档进行节点布局重构、语义化重命名及组件检查修正的通用工作流。

## 用户输入

执行此工作流需要用户提供：
1. **策划需求文档**（`.kiro/design/` 下的 .md 文件，或聊天中直接提供）
2. **目标 Prefab 路径**（Unity 项目中的 .prefab 文件）
3. **（可选）原型图/效果图**（用于辅助理解布局意图）

命名规范参考 #[[file:.kiro/steering/ui-naming-convention.md]]

---

## 阶段一：分析 Prefab 结构

### 1.1 解析层级树

读取 Prefab 的 YAML 源文件，通过以下字段还原父子层级：
- `m_Father` / `m_Children` / `m_RootOrder` → 层级关系
- `m_Name` → 当前节点名
- `m_Script` 的 `guid` → 组件类型（Text / Image / Button 等）
- `m_Text` → 文本内容（辅助理解节点用途）
- `m_Sprite` → 引用的图片资源
- `m_RaycastTarget` → 是否可交互
- `m_IsActive` → 是否激活

输出一份完整的缩进层级树，标注每个节点的组件类型和关键属性。

### 1.2 对照策划文档建立语义映射

逐一将 Prefab 节点与策划文档中的功能模块对应。对于每个节点判断：
- 它属于策划文档中的哪个功能区域？
- 它的实际用途是什么？（背景、图标、文本、按钮、容器…）
- 它是否需要代码动态控制？

---

## 阶段二：重命名

### 2.1 命名规则

| 节点类型 | 命名方式 | 示例 |
|---------|---------|------|
| 需要代码控制的组件节点 | `#前缀_功能描述` | `#img_HeroAvatar`、`#lbl_GoldNum`、`#btn_Buy` |
| 纯容器/面板节点 | 大驼峰描述性名称，无前缀 | `MainPanel`、`GoldArea`、`SkillPanel` |
| 背景底图（含 bg，不区分大小写） | `前缀_功能描述`，无 `#` | `img_MainBg`、`img_PanelBg` |
| 纯装饰节点（花纹/光效/分隔线/边框） | `前缀_功能描述`，无 `#` | `img_TitleDecor`、`img_Divider` |
| 循环生成的节点 | 编号后缀 | `#img_Fragment1`、`#img_Fragment2` |

### 2.2 `#` 前缀判断标准

`#` 是代码自动绑定标识，只有需要运行时代码控制的节点才加 `#`。

**保留 `#` 的节点（需要代码控制）：**
- 动态文本（数量、价格、名称、倒计时等会变化的文字）
- 需要运行时替换 sprite 的图标（头像、技能图标、奖励图标等）
- 可点击的按钮（购买、关闭、切换等）
- 需要显示/隐藏控制的节点（限购提示、状态标记等）
- 需要事件注册的节点

**不加 `#` 的节点（纯展示/装饰）：**
- 名称中包含 `bg`（不区分大小写）的背景底图
- 花纹、光效、边框等固定装饰
- 分隔线、固定标签装饰图
- 不会被代码修改的静态图片

### 2.3 生成重命名脚本

在 `Assets/Editor/` 下创建 Editor 脚本，使用 `Dictionary<string, string>` 存储旧名→新名映射。

```csharp
// 脚本模板
var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
try
{
    var renameMap = new Dictionary<string, string>
    {
        { "旧名称", "新名称" },
    };
    var allTransforms = prefabRoot.GetComponentsInChildren<Transform>(true);
    foreach (var t in allTransforms)
    {
        if (renameMap.TryGetValue(t.name, out string newName))
            t.name = newName;
    }
    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
}
finally
{
    PrefabUtility.UnloadPrefabContents(prefabRoot);
}
```

**注意：** 如果 Prefab 中存在同名节点，需要结合父节点路径做精确匹配，避免误改。

---

## 阶段三：检查与修正

重命名完成后，创建第二个 Editor 脚本执行以下检查：

### 3.1 按钮组件补充

| 前缀 | 必须有的组件 |
|------|------------|
| `#btn_` | `Button` |
| `#imgBtn_` | `Image` + `Button` |

缺少则自动添加。

### 3.2 其他组件一致性检查

| 前缀 | 必须有的组件 |
|------|------------|
| `#lbl_` | `Text`（旧版） |
| `#txt_` | `TextMeshProUGUI` |
| `#img_` | `Image` |
| `#sld_` | `Slider` |
| `#scro_` | `ScrollRect` |
| `#tgl_` | `Toggle` |
| `#ipt_` | `InputField` |
| `#drp_` | `Dropdown` |

### 3.3 `#` 前缀清理

遍历所有以 `#` 开头的节点，按以下规则移除 `#`：
1. 名称中包含 `bg`（`name.ToLower().Contains("bg")`）→ 移除
2. 在预定义的装饰性节点列表中 → 移除

装饰性节点需要根据具体 Prefab 分析后列出，通常包括：特效、花纹、分隔线、固定边框、光效等。

### 3.4 输出验证日志

脚本执行后输出：
- 补充组件的节点列表及数量
- 移除 `#` 的节点列表、新名称及原因
- 最终所有 `#` 开头节点的汇总（确认都是需要代码控制的）

---

## 执行清单

每次执行此工作流时，按顺序完成：

- [ ] 阅读策划需求文档，理解所有功能模块
- [ ] 解析 Prefab YAML，输出完整层级树
- [ ] 建立节点→功能模块的语义映射
- [ ] 编写重命名脚本并执行
- [ ] 编写检查修正脚本并执行
- [ ] 确认日志输出无异常
- [ ] 删除一次性 Editor 脚本（可选）

---

## 注意事项

1. 操作前建议先提交 Git，方便回滚
2. Editor 脚本使用 `[MenuItem("Tools/xxx")]` 注册菜单，方便手动触发
3. Prefab 编辑三步流程：`LoadPrefabContents` → 修改 → `SaveAsPrefabAsset` + `UnloadPrefabContents`
4. 重命名和检查分两个脚本，便于分步验证
5. 策划文档中未提及但 Prefab 中存在的节点，根据组件类型和上下文推断用途后命名
