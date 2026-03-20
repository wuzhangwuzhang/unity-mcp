# GameScene场景设置指南 / GameScene Setup Guide

## 概述 / Overview

本指南将帮助你在Unity编辑器中设置GameScene场景，包括Paddle（挡板）、Ball（弹球）、边界墙等游戏对象。

## 已完成的脚本 / Completed Scripts

✅ **Paddle.cs** - 挡板控制器
- 键盘输入处理（A/D和左右方向键）
- 边界限制
- 与球的碰撞检测
- 从GameConfig加载配置

✅ **Ball.cs** - 弹球控制器
- 随机角度发射
- 恒定速度维持
- 角度调整避免过小反弹角
- 底部边界检测
- 与挡板/砖块/墙壁的碰撞处理

## GameScene场景结构 / Scene Structure

```
GameScene
├── Main Camera
├── Canvas (UI)
│   └── ScoreText (待实现)
├── Paddle
├── Ball
├── Boundaries
│   ├── LeftWall
│   ├── RightWall
│   └── TopWall
└── BrickGrid (待实现)
```

## 设置步骤 / Setup Steps

### 1. 创建Paddle（挡板）

#### 1.1 创建GameObject
```
Hierarchy → 右键 → Create Empty
命名: Paddle
```

#### 1.2 设置Transform
```
Position: (0, -5, 0)
Scale: (2, 0.3, 1)
```

#### 1.3 添加组件
```
Add Component → Paddle (Script)
Add Component → Box Collider 2D
Add Component → Rigidbody 2D
Add Component → Sprite Renderer (可选，用于显示)
```

#### 1.4 配置组件
**Box Collider 2D:**
- Size: (2, 0.3)

**Rigidbody 2D:**
- Body Type: Kinematic (脚本会自动设置)
- Gravity Scale: 0
- Constraints: Freeze Rotation, Freeze Position Y

**Sprite Renderer (可选):**
- Color: 白色或任意颜色
- 或者添加一个简单的白色方块Sprite

#### 1.5 设置Layer和Tag
```
Layer: Paddle (Layer 7)
Tag: Paddle (需要先在Tags & Layers中创建)
```

#### 1.6 分配物理材质
```
在Project窗口找到: Assets/Physics2D/PaddlePhysicsMaterial
拖拽到Box Collider 2D的Material字段
```

### 2. 创建Ball（弹球）

#### 2.1 创建GameObject
```
Hierarchy → 右键 → Create Empty
命名: Ball
```

#### 2.2 设置Transform
```
Position: (0, -4, 0) - 在挡板上方
Scale: (0.3, 0.3, 1)
```

#### 2.3 添加组件
```
Add Component → Ball (Script)
Add Component → Circle Collider 2D
Add Component → Rigidbody 2D
Add Component → Sprite Renderer (可选)
```

#### 2.4 配置组件
**Circle Collider 2D:**
- Radius: 0.15

**Rigidbody 2D:**
- Body Type: Dynamic (脚本会自动设置)
- Gravity Scale: 0
- Collision Detection: Continuous
- Constraints: Freeze Rotation

**Sprite Renderer (可选):**
- 使用圆形Sprite或白色圆圈
- Color: 白色或任意颜色

#### 2.5 设置Layer和Tag
```
Layer: Ball (Layer 6)
Tag: Ball (需要先在Tags & Layers中创建)
```

#### 2.6 分配物理材质
```
在Project窗口找到: Assets/Physics2D/BallPhysicsMaterial
拖拽到Circle Collider 2D的Material字段
```

### 3. 创建Boundaries（边界墙）

#### 3.1 创建父对象
```
Hierarchy → 右键 → Create Empty
命名: Boundaries
```

#### 3.2 创建LeftWall（左墙）
```
Boundaries → 右键 → Create Empty
命名: LeftWall
```

**Transform:**
- Position: (-9, 0, 0)
- Scale: (0.5, 12, 1)

**添加组件:**
```
Add Component → Box Collider 2D
Add Component → Sprite Renderer (可选)
```

**Box Collider 2D:**
- Size: (0.5, 12)

**设置:**
- Layer: Boundary (Layer 9)
- Material: BoundaryPhysicsMaterial

#### 3.3 创建RightWall（右墙）
```
Boundaries → 右键 → Create Empty
命名: RightWall
```

**Transform:**
- Position: (9, 0, 0)
- Scale: (0.5, 12, 1)

**配置:** 与LeftWall相同

#### 3.4 创建TopWall（顶墙）
```
Boundaries → 右键 → Create Empty
命名: TopWall
```

**Transform:**
- Position: (0, 5, 0)
- Scale: (20, 0.5, 1)

**添加组件:**
```
Add Component → Box Collider 2D
Add Component → Sprite Renderer (可选)
```

**Box Collider 2D:**
- Size: (20, 0.5)

**设置:**
- Layer: Boundary (Layer 9)
- Material: BoundaryPhysicsMaterial

### 4. 配置Camera

#### 4.1 选中Main Camera

#### 4.2 设置Camera组件
```
Projection: Orthographic
Size: 5 (或从GameConfig读取)
Position: (0, 0, -10)
Background: 深色背景 (如 #1a1a2e)
```

### 5. 创建Tags和Layers

#### 5.1 创建Tags
```
Edit → Project Settings → Tags and Layers

添加Tags:
- Paddle
- Ball
- Brick
```

#### 5.2 验证Layers
确认以下Layers存在（应该在任务1中已创建）:
- Layer 6: Ball
- Layer 7: Paddle
- Layer 8: Brick
- Layer 9: Boundary

### 6. 配置碰撞矩阵

```
Edit → Project Settings → Physics 2D

Layer Collision Matrix:
- Ball 与 Paddle: ✓ (勾选)
- Ball 与 Brick: ✓ (勾选)
- Ball 与 Boundary: ✓ (勾选)
- 其他组合: ✗ (不勾选)
```

## 测试步骤 / Testing Steps

### 1. 基本测试
1. 打开GameScene场景
2. 点击Play按钮
3. 观察Console日志

**预期日志:**
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

### 2. 挡板控制测试
- 按 **A** 或 **←** 键，挡板应向左移动
- 按 **D** 或 **→** 键，挡板应向右移动
- 挡板应该被限制在屏幕边界内

### 3. 弹球物理测试
- 球应该在游戏开始时自动发射
- 球应该以恒定速度移动
- 球应该与墙壁正确反弹
- 球应该与挡板反弹，并根据碰撞位置改变角度
- 球掉落到底部时应该触发游戏结束

### 4. 碰撞测试
- 球与挡板碰撞应该在Console显示日志
- 球与墙壁碰撞应该正确反弹
- 碰撞时速度应该保持恒定

## 常见问题 / Common Issues

### 问题1: 球不发射
**原因:** GameManager未触发GameStarted事件

**解决方案:**
1. 确认GameManager存在于场景中
2. 确认GameManager.StartGame()被调用
3. 检查Ball是否订阅了GameStarted事件

### 问题2: 挡板不响应输入
**原因:** GameManager状态不是Playing

**解决方案:**
1. 确认GameManager.CurrentState == Playing
2. 检查Console是否有错误日志
3. 确认Paddle脚本已添加到GameObject

### 问题3: 球穿过墙壁
**原因:** 碰撞检测配置不正确

**解决方案:**
1. 确认Ball的Rigidbody2D使用Continuous碰撞检测
2. 确认碰撞矩阵正确配置
3. 确认物理材质已分配

### 问题4: 球速度不恒定
**原因:** 物理材质摩擦力不为0

**解决方案:**
1. 确认所有物理材质的Friction = 0
2. 确认Ball.NormalizeVelocity()正在Update中调用
3. 检查Console是否有速度归一化警告

### 问题5: 球与挡板碰撞角度不变
**原因:** Tag未正确设置

**解决方案:**
1. 确认Paddle的Tag设置为"Paddle"
2. 确认Ball的Tag设置为"Ball"
3. 检查HandlePaddleCollision()是否被调用

## 下一步 / Next Steps

完成GameScene基本设置后，接下来需要实现：

1. **Brick系统** (任务4)
   - Brick.cs 脚本
   - BrickGrid.cs 生成器
   - Brick预制体

2. **UI系统** (任务8)
   - GameUI.cs (分数显示)
   - EndScreenUI.cs (结束界面)

3. **完整游戏流程测试**
   - StartMenu → GameScene → EndScreen
   - 胜利/失败条件
   - 重新开始功能

## 调试技巧 / Debugging Tips

### 1. 启用Gizmos
在Scene视图中启用Gizmos可以看到碰撞体边界

### 2. 使用Debug.DrawRay
在Ball脚本中添加速度方向可视化：
```csharp
Debug.DrawRay(transform.position, rb.velocity, Color.red);
```

### 3. 慢动作测试
在GameManager中添加：
```csharp
Time.timeScale = 0.5f; // 半速
```

### 4. 查看物理调试信息
```
Window → Analysis → Physics Debugger
```

## 资源清单 / Resource Checklist

- [ ] Paddle GameObject 已创建
- [ ] Ball GameObject 已创建
- [ ] 三个边界墙已创建
- [ ] Tags (Paddle, Ball) 已创建
- [ ] Layers (Ball, Paddle, Boundary) 已配置
- [ ] 碰撞矩阵已正确设置
- [ ] 物理材质已分配
- [ ] GameConfig.asset 已创建并配置
- [ ] 场景已保存

## 成功标准 / Success Criteria

✅ 挡板可以左右移动并被边界限制
✅ 球在游戏开始时自动发射
✅ 球与墙壁正确反弹
✅ 球与挡板碰撞并改变角度
✅ 球掉落时触发游戏结束
✅ 速度保持恒定
✅ Console无错误日志

完成这些设置后，你就有了一个可玩的基础打砖块游戏框架！
