# StartMenu场景测试检查清单 / StartMenu Scene Testing Checklist

## 测试前准备 / Pre-Testing Setup

### 1. 场景设置检查
- [ ] StartMenu场景已在Build Settings中添加（File → Build Settings）
- [ ] GameScene场景已在Build Settings中添加
- [ ] 场景顺序正确：StartMenu (index 0), GameScene (index 1)

### 2. 场景内容检查
打开StartMenu场景，确认以下对象存在：

- [ ] **GameManager** GameObject
  - [ ] 包含 GameManager 组件
  - [ ] 包含 ScoreSystem 组件
  
- [ ] **Canvas** GameObject
  - [ ] 包含 Canvas 组件
  - [ ] 包含 CanvasScaler 组件
  - [ ] 包含 GraphicRaycaster 组件
  - [ ] 包含 StartMenuUI 组件
  
- [ ] **EventSystem** GameObject
  - [ ] 包含 EventSystem 组件
  - [ ] 包含 StandaloneInputModule 组件

- [ ] **UI元素**（Canvas的子对象）
  - [ ] TitleText (Text组件)
  - [ ] StartButton (Button组件)
  - [ ] InstructionsText (Text组件)

### 3. StartMenuUI组件引用检查
选中Canvas对象，在Inspector中检查StartMenuUI组件：

- [ ] Start Button 字段已赋值（拖拽StartButton对象）
- [ ] Title Text 字段已赋值（拖拽TitleText对象）
- [ ] Instructions Text 字段已赋值（拖拽InstructionsText对象）

## 运行测试 / Run Tests

### 测试步骤 / Test Steps

1. **启动场景**
   - 打开StartMenu场景
   - 点击Play按钮
   - 观察Console窗口

2. **预期日志输出 / Expected Console Output**
   ```
   [GameManager] Initialized
   [ScoreSystem] Score reset to 0
   [StartMenuUI] Initialized
   ```

3. **UI显示检查**
   - [ ] 标题文本显示："弹球打砖块\nBreakout Game"
   - [ ] 开始按钮显示："开始游戏 / Start Game"
   - [ ] 操作说明文本显示完整

4. **按钮功能测试**
   - 点击"开始游戏"按钮
   - 预期日志：`[StartMenuUI] Start button clicked`
   - 预期日志：`[GameManager] Loading GameScene`

## 常见错误及解决方案 / Common Errors and Solutions

### 错误1: "StartMenuUI组件引用缺失"
**症状**: Console显示 `[StartMenuUI] Start button reference is missing!`

**解决方案**:
1. 选中Canvas对象
2. 在Inspector中找到StartMenuUI组件
3. 手动拖拽UI元素到对应字段：
   - StartButton → Start Button字段
   - TitleText → Title Text字段
   - InstructionsText → Instructions Text字段

### 错误2: "GameManager instance not found"
**症状**: 点击按钮时显示 `[StartMenuUI] GameManager instance not found!`

**解决方案**:
1. 确认场景中存在GameManager对象
2. 确认GameManager对象上有GameManager组件
3. 重新运行场景

### 错误3: "Scene 'GameScene' couldn't be loaded"
**症状**: 点击按钮后显示场景加载失败

**解决方案**:
1. 打开 File → Build Settings
2. 确认GameScene.unity在场景列表中
3. 确认场景名称拼写正确（区分大小写）
4. 点击"Add Open Scenes"添加当前打开的场景

### 错误4: "Arial.ttf is no longer a valid built in font"
**症状**: 使用自动设置工具时出现字体错误

**解决方案**:
- ✅ 已修复：现在使用 LegacyRuntime.ttf
- 重新运行 Breakout → Setup StartMenu Scene

### 错误5: UI元素不显示
**症状**: 场景运行但看不到UI

**解决方案**:
1. 检查Canvas的Render Mode是否为 Screen Space - Overlay
2. 检查Camera是否存在
3. 检查UI元素的RectTransform位置是否在屏幕内
4. 检查Canvas Scaler设置

### 错误6: 按钮点击无反应
**症状**: 点击按钮没有任何反应

**解决方案**:
1. 确认EventSystem存在于场景中
2. 确认Button组件的Interactable选项已勾选
3. 确认StartMenuUI组件的Start Button引用已设置
4. 检查Console是否有错误日志

## 手动创建场景（如果自动工具失败）/ Manual Scene Creation

如果自动设置工具出现问题，请按照以下步骤手动创建：

### 步骤1: 创建GameManager
```
Hierarchy → 右键 → Create Empty
命名: GameManager
添加组件: GameManager (Script)
添加组件: ScoreSystem (Script)
```

### 步骤2: 创建Canvas
```
Hierarchy → 右键 → UI → Canvas
Canvas设置:
  - Render Mode: Screen Space - Overlay
  - Canvas Scaler:
    - UI Scale Mode: Scale With Screen Size
    - Reference Resolution: 1920 x 1080
```

### 步骤3: 创建UI元素
```
Canvas → 右键 → UI → Text (TitleText)
Canvas → 右键 → UI → Button (StartButton)
Canvas → 右键 → UI → Text (InstructionsText)
```

### 步骤4: 配置位置和文本
参考 `StartMenu_Setup_Instructions.md` 中的详细配置

### 步骤5: 添加StartMenuUI脚本
```
选中Canvas
Add Component → StartMenuUI
设置引用字段
```

## 调试技巧 / Debugging Tips

### 1. 启用详细日志
所有脚本都包含Debug.Log输出，观察Console窗口可以追踪执行流程

### 2. 检查单例实例
在GameManager.cs的Awake方法中添加断点，确认单例正确初始化

### 3. 验证事件订阅
确认GameEvents的事件订阅没有异常

### 4. 场景加载测试
在GameManager.LoadGameScene()方法中添加断点，确认方法被调用

## 成功标准 / Success Criteria

场景测试成功的标准：

✅ 场景启动无错误
✅ UI元素正确显示
✅ Console显示初始化日志
✅ 点击按钮触发场景加载
✅ 没有空引用异常
✅ GameManager单例正常工作

## 下一步 / Next Steps

如果StartMenu场景测试通过：
1. 继续实现GameScene场景
2. 实现Paddle、Ball、Brick等游戏对象
3. 实现GameUI和EndScreenUI

如果测试失败：
1. 记录Console中的错误信息
2. 按照上述解决方案逐一排查
3. 检查所有引用是否正确设置
