# 快速设置参考 / Quick Setup Reference

## 🎯 三步完成游戏设置

### 步骤0: 设置项目Tags和Layers（首次运行）
```
1. 点击菜单: Breakout → Setup Project (Tags & Layers)
2. 确认对话框显示"Setup Complete"
3. 这会自动创建所有需要的Tags
```

### 步骤1: 设置StartMenu场景
```
1. 打开 Assets/Scenes/StartMenu.unity
2. 点击菜单: Breakout → Setup StartMenu Scene
3. 保存场景 (Ctrl+S)
4. 测试: 点击Play，点击"开始游戏"按钮
```

### 步骤2: 设置GameScene场景
```
1. 打开 Assets/Scenes/GameScene.unity
2. 点击菜单: Breakout → Setup GameScene
3. 保存场景 (Ctrl+S)
4. 测试: 从StartMenu启动，使用A/D控制挡板
```

### 步骤3: 验证设置
```
1. 点击菜单: Breakout → Validate StartMenu Scene
2. 检查验证报告，确保所有检查通过
3. 运行完整游戏流程测试
```

## 🛠️ 可用的编辑器工具

| 菜单项 | 功能 | 使用时机 |
|--------|------|----------|
| **Breakout → Setup Project (Tags & Layers)** | 自动创建Tags和验证Layers | 首次设置项目 |
| **Breakout → Setup StartMenu Scene** | 自动创建StartMenu场景UI | 首次设置或重置StartMenu |
| **Breakout → Setup GameScene** | 自动创建GameScene游戏对象 | 首次设置或重置GameScene |
| **Breakout → Validate StartMenu Scene** | 验证StartMenu场景配置 | 测试前验证设置 |

## 📋 场景检查清单

### StartMenu场景 ✅
- [ ] GameManager对象存在
- [ ] Canvas对象存在
- [ ] TitleText显示正确
- [ ] StartButton可点击
- [ ] InstructionsText显示完整
- [ ] StartMenuUI组件引用已设置
- [ ] 点击按钮可跳转到GameScene

### GameScene场景 ✅
- [ ] Paddle对象存在并可控制
- [ ] Ball对象存在并自动发射
- [ ] 三个边界墙存在
- [ ] Camera配置正确
- [ ] Canvas和ScoreText存在
- [ ] Tags和Layers正确设置
- [ ] 物理材质已分配
- [ ] 碰撞矩阵正确配置

## 🎮 控制说明

| 按键 | 功能 |
|------|------|
| **A** 或 **←** | 挡板向左移动 |
| **D** 或 **→** | 挡板向右移动 |
| **Esc** | 暂停（待实现） |

## 📊 预期Console日志

### StartMenu启动
```
[GameManager] Initialized
[ScoreSystem] Score reset to 0
[StartMenuUI] Initialized
```

### 点击开始按钮
```
[StartMenuUI] Start button clicked
[GameManager] Loading GameScene
[GameManager] GameScene loaded successfully
[GameManager] Starting game
```

### GameScene启动
```
[Paddle] Initialized
[Paddle] Loaded config - Speed: 10, Range: [-8, 8]
[Ball] Initialized
[Ball] Loaded config - Speed: 5, MinAngle: 30
[Ball] Launched at angle: XX° with velocity: (X, X)
```

### 游戏进行中
```
[Paddle] Ball collision at position: (X, Y)
[Ball] Paddle collision - Offset: X, Angle: X°
[Ball] Collided with boundary
```

### 球掉落
```
[Ball] Ball fell below bottom boundary
[GameManager] Ball fell - Game Over
[GameManager] Game ended - Game Over
```

## 🐛 快速故障排除

| 问题 | 快速解决方案 |
|------|-------------|
| UI不显示 | 检查Canvas的Render Mode是否为Screen Space - Overlay |
| 按钮不响应 | 确认EventSystem存在 |
| 挡板不移动 | 确认GameManager.CurrentState == Playing |
| 球不发射 | 从StartMenu启动，不要直接运行GameScene |
| 球穿墙 | 确认Ball的Collision Detection为Continuous |
| 速度不恒定 | 确认物理材质Friction = 0 |

## 📁 重要文件位置

### 脚本
```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── GameEvents.cs
│   └── ScoreSystem.cs
├── GameObjects/
│   ├── Paddle.cs
│   └── Ball.cs
├── UI/
│   └── StartMenuUI.cs
└── Data/
    └── GameConfig.cs
```

### 场景
```
Assets/Scenes/
├── StartMenu.unity
├── GameScene.unity
└── EndScreen.unity
```

### 配置
```
Assets/Resources/
└── GameConfig.asset
```

### 物理材质
```
Assets/Physics2D/
├── BallPhysicsMaterial.physicsMaterial2D
├── PaddlePhysicsMaterial.physicsMaterial2D
└── BoundaryPhysicsMaterial.physicsMaterial2D
```

## 🎯 下一步开发

### 优先级1: 砖块系统
```
1. 创建 Brick.cs
2. 创建 BrickGrid.cs
3. 创建 Brick预制体
4. 测试砖块销毁和分数
```

### 优先级2: UI系统
```
1. 创建 GameUI.cs
2. 创建 EndScreenUI.cs
3. 测试分数显示和结束界面
```

### 优先级3: 完整测试
```
1. 测试完整游戏流程
2. 测试胜利条件
3. 测试失败条件
4. 测试重新开始功能
```

## 💡 专业提示

1. **总是从StartMenu启动**: 这确保GameManager正确初始化
2. **使用验证工具**: 在测试前运行验证工具避免配置错误
3. **查看Console日志**: 所有重要事件都有日志输出
4. **保存场景**: 修改后记得保存（Ctrl+S）
5. **备份场景**: 在大改动前复制场景文件

## 📞 获取帮助

如果遇到问题：
1. 检查Console是否有错误日志
2. 运行验证工具检查配置
3. 参考详细文档：
   - `StartMenu_Setup_Instructions.md`
   - `GameScene_Auto_Setup_Instructions.md`
   - `StartMenu_Testing_Checklist.md`
   - `GameScene_Setup_Guide.md`

---

**准备好开始了吗？** 🚀

1. 点击 **Breakout → Setup Project (Tags & Layers)** （首次运行）
2. 打开StartMenu场景
3. 点击 **Breakout → Setup StartMenu Scene**
4. 打开GameScene场景
5. 点击 **Breakout → Setup GameScene**
6. 从StartMenu场景点击Play测试！
