# 故障排除指南 / Troubleshooting Guide

## 🐛 常见错误及解决方案

### 错误1: "Tag 'Paddle' is not defined"

**错误信息**:
```
Tag: Paddle is not defined
```

**原因**: Tags还没有在Unity的Tags & Layers中创建

**解决方案**:
```
点击菜单: Breakout → Setup Project (Tags & Layers)
```

这会自动创建所有需要的Tags（Paddle, Ball, Brick）

---

### 错误2: "NullReferenceException" 在CreatePaddle

**错误信息**:
```
NullReferenceException: Object reference not set to an instance of an object
BreakoutGame.Editor.GameSceneSetup.CreatePaddle
```

**原因**: 
1. GameConfig.asset文件不存在
2. Sprite创建失败

**解决方案**:

**方案A - 检查GameConfig**:
```
1. 确认 Assets/Resources/GameConfig.asset 存在
2. 如果不存在，右键 Resources 文件夹
3. Create → Breakout → GameConfig
4. 重新运行 Breakout → Setup GameScene
```

**方案B - 已修复**:
- 最新版本的代码已经添加了空值检查
- 即使Sprite创建失败，也会继续执行
- 会显示警告但不会中断

---

### 错误3: "GameConfig not found in Resources folder"

**错误信息**:
```
[GameSceneSetup] GameConfig not found in Resources folder!
```

**原因**: GameConfig.asset文件不在Resources文件夹中

**解决方案**:

**步骤1**: 创建Resources文件夹（如果不存在）
```
1. 在Project窗口右键 Assets 文件夹
2. Create → Folder
3. 命名为 "Resources"
```

**步骤2**: 创建GameConfig
```
1. 右键 Resources 文件夹
2. Create → Breakout → GameConfig
3. 命名为 "GameConfig"
```

**步骤3**: 配置GameConfig
```
1. 选中 GameConfig.asset
2. 在Inspector中设置参数（使用默认值即可）
3. 保存
```

---

### 错误4: 物理材质未分配

**症状**: 球或挡板的物理行为不正确

**原因**: 物理材质文件不存在或路径错误

**解决方案**:

**检查物理材质文件**:
```
确认以下文件存在:
- Assets/Physics2D/BallPhysicsMaterial.physicsMaterial2D
- Assets/Physics2D/PaddlePhysicsMaterial.physicsMaterial2D
- Assets/Physics2D/BoundaryPhysicsMaterial.physicsMaterial2D
```

**手动创建物理材质**:
```
1. 创建 Assets/Physics2D 文件夹
2. 右键 Physics2D 文件夹
3. Create → 2D → Physics Material 2D
4. 命名为 BallPhysicsMaterial
5. 设置参数:
   - Friction: 0
   - Bounciness: 1
6. 重复创建 PaddlePhysicsMaterial 和 BoundaryPhysicsMaterial
```

---

### 错误5: Layers不存在

**症状**: GameObject的Layer设置失败

**原因**: Layers还没有在Project Settings中创建

**解决方案**:

**自动验证**:
```
点击菜单: Breakout → Setup Project (Tags & Layers)
```

**手动创建**:
```
1. Edit → Project Settings → Tags and Layers
2. 展开 Layers
3. 设置以下Layers:
   - Layer 6: Ball
   - Layer 7: Paddle
   - Layer 8: Brick
   - Layer 9: Boundary
```

---

### 错误6: UI不显示

**症状**: 运行游戏后看不到UI元素

**原因**: Canvas配置不正确

**解决方案**:

**检查Canvas设置**:
```
1. 选中Canvas对象
2. 确认 Render Mode: Screen Space - Overlay
3. 确认 Canvas Scaler 存在
4. 确认 Graphic Raycaster 存在
```

**检查EventSystem**:
```
1. 确认场景中存在 EventSystem 对象
2. 如果不存在，右键 Hierarchy
3. UI → Event System
```

---

### 错误7: 挡板不响应输入

**症状**: 按A/D键挡板不移动

**原因**: GameManager状态不是Playing

**解决方案**:

**从StartMenu启动**:
```
1. 不要直接运行GameScene
2. 打开StartMenu场景
3. 点击Play
4. 点击"开始游戏"按钮
```

**检查GameManager**:
```
1. 确认场景中存在GameManager对象
2. 确认GameManager.CurrentState == Playing
3. 查看Console日志确认游戏已开始
```

---

### 错误8: 球不发射

**症状**: 游戏开始后球不动

**原因**: GameStarted事件未触发

**解决方案**:

**检查事件订阅**:
```
1. 确认Ball脚本订阅了GameStarted事件
2. 查看Console日志:
   - 应该显示 [Ball] Launched at angle: XX°
3. 如果没有日志，检查GameManager是否调用了StartGame()
```

**手动测试**:
```
1. 在Ball.cs的Launch()方法开始处添加:
   Debug.Log("[Ball] Launch() called");
2. 运行游戏查看是否调用
```

---

### 错误9: 球穿过墙壁

**症状**: 球直接穿过边界墙

**原因**: 碰撞检测配置不正确

**解决方案**:

**检查Ball设置**:
```
1. 选中Ball对象
2. Rigidbody2D → Collision Detection: Continuous
3. 确认CircleCollider2D存在
```

**检查碰撞矩阵**:
```
1. Edit → Project Settings → Physics 2D
2. Layer Collision Matrix:
   - Ball 与 Boundary: 勾选 ✓
   - Ball 与 Paddle: 勾选 ✓
   - Ball 与 Brick: 勾选 ✓
```

**检查Layers**:
```
1. 确认Ball的Layer设置为"Ball"
2. 确认墙壁的Layer设置为"Boundary"
```

---

### 错误10: 速度不恒定

**症状**: 球的速度越来越快或越来越慢

**原因**: 物理材质摩擦力不为0

**解决方案**:

**检查物理材质**:
```
1. 选中物理材质文件
2. 确认 Friction: 0
3. 确认 Bounciness: 1
```

**检查Ball脚本**:
```
1. 确认NormalizeVelocity()在Update中被调用
2. 查看Console是否有速度归一化警告
```

---

## 🔍 调试技巧

### 1. 查看Console日志

所有重要事件都有详细的日志输出：

```
[GameManager] Initialized
[Paddle] Initialized
[Ball] Initialized
[Ball] Launched at angle: XX°
[Paddle] Ball collision at position: (X, Y)
```

### 2. 使用Scene视图

在Scene视图中启用Gizmos可以看到：
- 碰撞体边界（绿色线框）
- 物理交互（碰撞点）

### 3. 使用Physics Debugger

```
Window → Analysis → Physics Debugger
```

可以实时查看：
- 碰撞体状态
- 速度向量
- 碰撞事件

### 4. 慢动作测试

在GameManager中添加：
```csharp
Time.timeScale = 0.5f; // 半速播放
```

### 5. 添加可视化调试

在Ball.cs的Update中添加：
```csharp
Debug.DrawRay(transform.position, rb.velocity, Color.red);
```

这会在Scene视图中显示球的速度方向

---

## 📋 检查清单

运行游戏前的完整检查清单：

### 项目设置
- [ ] Tags已创建（Paddle, Ball, Brick）
- [ ] Layers已配置（Ball=6, Paddle=7, Brick=8, Boundary=9）
- [ ] 碰撞矩阵已正确设置
- [ ] GameConfig.asset存在于Resources文件夹
- [ ] 物理材质已创建并配置

### StartMenu场景
- [ ] GameManager对象存在
- [ ] Canvas和UI元素存在
- [ ] StartMenuUI组件引用已设置
- [ ] EventSystem存在

### GameScene场景
- [ ] Paddle对象存在并配置正确
- [ ] Ball对象存在并配置正确
- [ ] 三个边界墙存在
- [ ] Canvas和ScoreText存在
- [ ] Camera配置正确

### Build Settings
- [ ] StartMenu场景已添加（index 0）
- [ ] GameScene场景已添加（index 1）
- [ ] 场景顺序正确

---

## 🆘 仍然有问题？

如果以上解决方案都不起作用：

1. **重新运行设置工具**:
   ```
   Breakout → Setup Project (Tags & Layers)
   Breakout → Setup StartMenu Scene
   Breakout → Setup GameScene
   ```

2. **检查Unity版本**:
   - 确认使用Unity 2022.3 LTS或更高版本

3. **清理并重新导入**:
   ```
   1. 关闭Unity
   2. 删除 Library 文件夹
   3. 重新打开Unity（会重新导入所有资源）
   ```

4. **查看详细文档**:
   - `SETUP_INSTRUCTIONS.md` - 设置指南
   - `GameScene_Auto_Setup_Instructions.md` - GameScene详细说明
   - `Development_Progress_Summary.md` - 开发进度

5. **检查Console错误**:
   - 双击Console中的错误可以跳转到代码位置
   - 查看完整的堆栈跟踪信息

---

## 💡 预防性建议

为了避免问题：

1. **按顺序运行设置工具**:
   - 先运行 Setup Project
   - 再运行 Setup StartMenu Scene
   - 最后运行 Setup GameScene

2. **总是保存场景**:
   - 修改后按 Ctrl+S 保存

3. **从StartMenu启动**:
   - 不要直接运行GameScene

4. **定期备份**:
   - 复制整个项目文件夹作为备份

5. **使用版本控制**:
   - 使用Git管理代码变更

---

祝你开发顺利！🎮
