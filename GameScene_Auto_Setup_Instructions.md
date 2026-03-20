# GameScene自动设置说明 / GameScene Auto-Setup Instructions

## 🚀 快速开始

### 一键自动创建GameScene场景元素

1. **打开GameScene场景**
   - 在Project窗口中双击 `Assets/Scenes/GameScene.unity`

2. **运行自动设置工具**
   - 点击Unity顶部菜单：**Breakout → Setup GameScene**
   - 等待自动创建完成（约2-3秒）
   - 点击弹出对话框的"OK"按钮

3. **保存场景**
   - 按 `Ctrl+S` 保存场景

4. **测试游戏**
   - 点击Play按钮
   - 使用A/D或左右方向键控制挡板
   - 观察球的物理行为

## ✅ 自动创建的元素

### 1. Paddle（挡板）
- **位置**: (0, -5, 0)
- **大小**: 从GameConfig读取（默认2x0.3）
- **组件**:
  - Paddle.cs 脚本
  - BoxCollider2D
  - Rigidbody2D (Kinematic)
  - SpriteRenderer (白色方块)
- **Layer**: Paddle (Layer 7)
- **Tag**: Paddle
- **物理材质**: PaddlePhysicsMaterial

### 2. Ball（弹球）
- **位置**: (0, -4, 0) - 在挡板上方
- **大小**: 0.3x0.3
- **组件**:
  - Ball.cs 脚本
  - CircleCollider2D
  - Rigidbody2D (Dynamic)
  - SpriteRenderer (青色圆形)
- **Layer**: Ball (Layer 6)
- **Tag**: Ball
- **物理材质**: BallPhysicsMaterial
- **碰撞检测**: Continuous

### 3. Boundaries（边界墙）
包含三个墙壁：

#### LeftWall（左墙）
- **位置**: 从GameConfig读取（默认-9, 0, 0）
- **大小**: 0.5x12x1
- **Layer**: Boundary (Layer 9)

#### RightWall（右墙）
- **位置**: 从GameConfig读取（默认9, 0, 0）
- **大小**: 0.5x12x1
- **Layer**: Boundary (Layer 9)

#### TopWall（顶墙）
- **位置**: 从GameConfig读取（默认0, 5, 0）
- **大小**: 20x0.5x1
- **Layer**: Boundary (Layer 9)

所有墙壁都包含：
- BoxCollider2D
- SpriteRenderer (灰色)
- BoundaryPhysicsMaterial

### 4. Main Camera
- **投影模式**: Orthographic
- **大小**: 从GameConfig读取（默认5）
- **位置**: (0, 0, -10)
- **背景色**: 深蓝色 (#1a1a2e)

### 5. Canvas（UI）
- **渲染模式**: Screen Space - Overlay
- **Canvas Scaler**: Scale With Screen Size (1920x1080)
- **子对象**:
  - **ScoreText**: 显示"Score: 0"
    - 位置: 左上角 (20, -20)
    - 字体大小: 36
    - 颜色: 白色

## 📋 场景层级结构

自动创建后的场景结构：

```
GameScene
├── Main Camera
├── Paddle
│   ├── Paddle (Script)
│   ├── BoxCollider2D
│   ├── Rigidbody2D
│   └── SpriteRenderer
├── Ball
│   ├── Ball (Script)
│   ├── CircleCollider2D
│   ├── Rigidbody2D
│   └── SpriteRenderer
├── Boundaries
│   ├── LeftWall
│   │   ├── BoxCollider2D
│   │   └── SpriteRenderer
│   ├── RightWall
│   │   ├── BoxCollider2D
│   │   └── SpriteRenderer
│   └── TopWall
│       ├── BoxCollider2D
│       └── SpriteRenderer
└── Canvas
    ├── Canvas
    ├── CanvasScaler
    ├── GraphicRaycaster
    └── ScoreText (Text)
```

## 🎮 测试步骤

### 1. 基本功能测试

1. **启动游戏**
   - 从StartMenu场景开始
   - 点击"开始游戏"按钮
   - 应该加载到GameScene

2. **观察Console日志**
   ```
   [GameManager] Loading GameScene
   [GameManager] GameScene loaded successfully
   [GameManager] Starting game
   [Paddle] Initialized
   [Paddle] Loaded config - Speed: 10, Range: [-8, 8]
   [Ball] Initialized
   [Ball] Loaded config - Speed: 5, MinAngle: 30
   [Ball] Launched at angle: XX° with velocity: (X, X)
   ```

3. **测试挡板控制**
   - 按 **A** 或 **←** 键 → 挡板向左移动
   - 按 **D** 或 **→** 键 → 挡板向右移动
   - 挡板应该停在边界处，不会超出屏幕

4. **测试弹球物理**
   - 球应该自动向上发射
   - 球应该与墙壁反弹
   - 球应该与挡板反弹并改变角度
   - 球速度应该保持恒定

5. **测试碰撞**
   - 球与挡板碰撞时Console显示：`[Paddle] Ball collision at position: (X, Y)`
   - 球与挡板碰撞时Console显示：`[Ball] Paddle collision - Offset: X, Angle: X°`
   - 球与墙壁碰撞时Console显示：`[Ball] Collided with boundary`

6. **测试游戏结束**
   - 让球掉落到底部
   - Console应该显示：`[Ball] Ball fell below bottom boundary`
   - 游戏应该结束（目前EndScreen未实现，会看到空场景）

## ⚙️ 配置说明

### GameConfig参数

自动设置工具从 `Resources/GameConfig.asset` 读取以下参数：

**Paddle设置:**
- `paddleMoveSpeed`: 挡板移动速度（默认10）
- `paddleMinX`: 挡板最小X坐标（默认-8）
- `paddleMaxX`: 挡板最大X坐标（默认8）
- `paddleWidth`: 挡板宽度（默认2）
- `paddleHeight`: 挡板高度（默认0.3）

**Ball设置:**
- `ballSpeed`: 球速度（默认5）
- `ballMinAngle`: 最小反弹角度（默认30°）
- `ballLaunchAngleRange`: 发射角度范围（默认30°）

**Boundary设置:**
- `leftBoundary`: 左边界X坐标（默认-9）
- `rightBoundary`: 右边界X坐标（默认9）
- `topBoundary`: 顶部边界Y坐标（默认5）
- `bottomBoundary`: 底部边界Y坐标（默认-6）
- `boundaryThickness`: 边界墙厚度（默认0.5）

**Camera设置:**
- `cameraSize`: 正交相机大小（默认5）
- `cameraPosition`: 相机位置（默认0, 0, -10）

### 修改配置

1. 在Project窗口找到 `Assets/Resources/GameConfig.asset`
2. 在Inspector中修改参数
3. 重新运行 **Breakout → Setup GameScene** 应用新配置

## 🔧 手动调整

### 调整挡板大小
```
选中Paddle对象
修改Transform.Scale: (宽度, 高度, 1)
```

### 调整球大小
```
选中Ball对象
修改Transform.Scale: (大小, 大小, 1)
```

### 调整墙壁位置
```
选中Boundaries下的墙壁对象
修改Transform.Position
```

### 更改颜色
```
选中对象
在SpriteRenderer组件中修改Color
```

## 🐛 常见问题

### 问题1: "GameConfig not found"
**原因**: GameConfig.asset不在Resources文件夹中

**解决方案**:
1. 确认 `Assets/Resources/GameConfig.asset` 存在
2. 如果不存在，运行 **Breakout → Setup StartMenu Scene** 会自动创建
3. 或者手动创建：右键 → Create → Breakout → GameConfig

### 问题2: 物理材质未分配
**原因**: 物理材质文件不存在

**解决方案**:
1. 确认 `Assets/Physics2D/` 文件夹存在
2. 确认三个物理材质文件存在：
   - BallPhysicsMaterial.physicsMaterial2D
   - PaddlePhysicsMaterial.physicsMaterial2D
   - BoundaryPhysicsMaterial.physicsMaterial2D
3. 如果不存在，需要手动创建（任务1应该已创建）

### 问题3: Tags或Layers不存在
**原因**: Tags和Layers未在Project Settings中创建

**解决方案**:
1. Edit → Project Settings → Tags and Layers
2. 添加Tags: Paddle, Ball, Brick
3. 确认Layers: Ball (6), Paddle (7), Brick (8), Boundary (9)

### 问题4: 球不发射
**原因**: GameManager未触发GameStarted事件

**解决方案**:
1. 确认场景中存在GameManager对象（应该从StartMenu场景带过来）
2. 如果不存在，在Hierarchy中创建空对象，命名为GameManager
3. 添加GameManager和ScoreSystem组件

### 问题5: 挡板不响应输入
**原因**: GameManager状态不是Playing

**解决方案**:
1. 从StartMenu场景启动游戏，而不是直接运行GameScene
2. 确认Console显示 `[GameManager] Starting game`
3. 确认GameManager.CurrentState == Playing

## 📝 后续步骤

完成GameScene自动设置后：

1. **测试基本功能**
   - 从StartMenu启动游戏
   - 测试挡板和球的物理行为
   - 确认所有碰撞正常工作

2. **实现砖块系统**
   - 创建Brick.cs脚本
   - 创建BrickGrid.cs脚本
   - 创建Brick预制体
   - 运行游戏测试砖块销毁

3. **实现GameUI**
   - 创建GameUI.cs脚本
   - 添加到Canvas对象
   - 连接ScoreText引用
   - 测试分数显示

4. **实现EndScreenUI**
   - 创建EndScreenUI.cs脚本
   - 在Canvas下创建EndScreenPanel
   - 添加结束提示和按钮
   - 测试游戏结束流程

## 🎉 优势

使用自动设置工具的优势：

✅ **快速**: 2-3秒完成所有设置
✅ **准确**: 所有参数从GameConfig读取，确保一致性
✅ **完整**: 自动创建所有必需的组件和引用
✅ **可重复**: 可以随时重新运行，覆盖旧设置
✅ **易于调试**: 所有对象命名规范，结构清晰

## 💡 提示

- 自动设置工具会删除已存在的同名对象，所以可以安全地重新运行
- 如果修改了GameConfig，重新运行工具会应用新配置
- 工具创建的Sprite是程序生成的简单图形，可以替换为自定义Sprite
- 所有物理参数都可以在Inspector中手动微调

---

**准备好了吗？** 点击 **Breakout → Setup GameScene** 开始自动设置！🚀
