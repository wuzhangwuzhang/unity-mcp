# StartMenu场景设置说明 / StartMenu Scene Setup Instructions

## 场景结构 / Scene Structure

请按照以下步骤在Unity编辑器中设置StartMenu场景：

### 1. 创建GameManager对象

1. 在Hierarchy中右键 → Create Empty
2. 命名为 "GameManager"
3. 添加组件：`GameManager.cs` (Assets/Scripts/Core/GameManager.cs)
4. 添加组件：`ScoreSystem.cs` (Assets/Scripts/Core/ScoreSystem.cs)

### 2. 创建Canvas和UI元素

#### 2.1 创建Canvas
1. 右键Hierarchy → UI → Canvas
2. Canvas设置：
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080
     - Match: 0.5

#### 2.2 创建EventSystem
- Unity会自动创建EventSystem，如果没有，右键 → UI → Event System

#### 2.3 创建标题文本 (TitleText)
1. 右键Canvas → UI → Text
2. 命名为 "TitleText"
3. 设置：
   - Text: "弹球打砖块\nBreakout Game"
   - Font Size: 60
   - Alignment: Center, Middle
   - Color: 白色
   - Rect Transform:
     - Anchor: Top Center
     - Pos X: 0, Pos Y: -150
     - Width: 800, Height: 200

#### 2.4 创建开始按钮 (StartButton)
1. 右键Canvas → UI → Button
2. 命名为 "StartButton"
3. 设置：
   - Rect Transform:
     - Anchor: Middle Center
     - Pos X: 0, Pos Y: 0
     - Width: 300, Height: 80
   - Button组件的Text子对象：
     - Text: "开始游戏 / Start Game"
     - Font Size: 32
     - Color: 黑色

#### 2.5 创建操作说明文本 (InstructionsText)
1. 右键Canvas → UI → Text
2. 命名为 "InstructionsText"
3. 设置：
   - Text: 
     ```
     操作说明 / Controls:
     A / ← : 向左移动挡板 / Move paddle left
     D / → : 向右移动挡板 / Move paddle right
     
     目标 / Goal:
     击碎所有砖块获得胜利！
     Destroy all bricks to win!
     ```
   - Font Size: 24
   - Alignment: Center, Middle
   - Color: 白色
   - Rect Transform:
     - Anchor: Bottom Center
     - Pos X: 0, Pos Y: 150
     - Width: 700, Height: 250

### 3. 添加StartMenuUI脚本

1. 在Canvas上添加组件：`StartMenuUI.cs` (Assets/Scripts/UI/StartMenuUI.cs)
2. 在Inspector中设置引用：
   - Start Button: 拖拽 StartButton 到此字段
   - Title Text: 拖拽 TitleText 到此字段
   - Instructions Text: 拖拽 InstructionsText 到此字段

### 4. 保存场景

按 Ctrl+S 保存场景

## 最终层级结构 / Final Hierarchy

```
StartMenu (Scene)
├── Main Camera
├── GameManager
│   ├── GameManager (Script)
│   └── ScoreSystem (Script)
├── Canvas
│   ├── StartMenuUI (Script)
│   ├── TitleText (Text)
│   ├── StartButton (Button)
│   │   └── Text (Text)
│   └── InstructionsText (Text)
└── EventSystem
```

## 测试 / Testing

1. 确保Build Settings中包含StartMenu和GameScene场景
2. 运行StartMenu场景
3. 点击"开始游戏"按钮应该加载GameScene

## 注意事项 / Notes

- GameManager和ScoreSystem会在场景切换时保持（DontDestroyOnLoad）
- 确保所有UI元素的引用都正确设置
- 如果按钮点击没有反应，检查EventSystem是否存在
