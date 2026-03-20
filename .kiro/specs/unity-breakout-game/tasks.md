# 实现计划: Unity弹球打砖块游戏

## 概述

本实现计划将Unity 2022弹球打砖块游戏分解为离散的编码任务。游戏采用模块化架构，包含核心系统层（GameManager、ScoreSystem、EventSystem）、游戏对象层（Ball、Paddle、Brick、BrickGrid、Boundary）和UI层（StartMenuUI、GameUI、EndScreenUI）。实现将按照从基础设施到核心功能再到UI集成的顺序进行。

## 任务

- [x] 1. 设置项目结构和核心系统
  - [x] 1.1 创建Unity 2022项目和场景结构
    - 创建三个场景：StartMenu、GameScene、EndScreen
    - 配置Build Settings中的场景顺序
    - 设置项目物理层（Ball、Paddle、Brick、Boundary）和碰撞矩阵
    - 创建物理材质（Ball、Paddle、Boundary）：Friction=0, Bounciness=1
    - _需求: 1.1, 1.2, 8.1, 8.2, 8.3, 8.5_

  - [x] 1.2 实现EventSystem事件系统
    - 创建`Scripts/Core/GameEvents.cs`静态类
    - 定义所有游戏事件：GameStarted, GameEnded, BrickDestroyed, BallFell, BallPaddleCollision, ScoreChanged
    - 实现事件触发方法和异常处理包装
    - _需求: 1.3, 1.4, 3.5, 3.6, 4.2, 4.3_

  - [ ]* 1.3 为EventSystem编写单元测试
    - 测试事件订阅和触发机制
    - 测试事件处理器异常不影响其他订阅者
    - _需求: 1.3, 1.4_

  - [x] 1.4 实现ScoreSystem得分系统
    - 创建`Scripts/Core/ScoreSystem.cs`单例类
    - 实现分数管理方法：AddScore(), ResetScore()
    - 订阅BrickDestroyed事件并增加分数
    - 触发ScoreChanged事件
    - _需求: 4.3, 5.1, 5.2_

  - [ ]* 1.5 为ScoreSystem编写属性测试
    - **属性 12: 砖块销毁增加分数**
    - **验证: 需求 4.3, 5.2**

  - [ ]* 1.6 为ScoreSystem编写单元测试
    - 测试分数初始化为0（需求 5.1）
    - 测试AddScore正确增加分数
    - 测试ResetScore重置分数
    - _需求: 5.1, 5.2_

  - [x] 1.7 创建GameConfig ScriptableObject
    - 创建`Scripts/Data/GameConfig.cs`
    - 定义所有游戏配置参数（球速、挡板速度、砖块布局、边界等）
    - 在项目中创建GameConfig资源实例
    - _需求: 2.4, 3.1, 3.2, 4.1, 8.1, 8.2, 8.3, 8.5_

- [ ] 2. 实现游戏对象核心组件
  - [x] 2.1 实现Paddle挡板控制
    - 创建`Scripts/GameObjects/Paddle.cs`
    - 添加BoxCollider2D和Rigidbody2D（Kinematic）组件
    - 实现HandleInput()处理键盘输入（A/D和左右方向键）
    - 实现Move()和ClampPosition()限制移动范围
    - _需求: 2.1, 2.2, 2.3, 2.4_

  - [ ]* 2.2 为Paddle编写属性测试
    - **属性 4: 挡板左移响应**
    - **验证: 需求 2.2**
    - **属性 5: 挡板右移响应**
    - **验证: 需求 2.3**
    - **属性 6: 挡板边界约束不变量**
    - **验证: 需求 2.4**

  - [ ]* 2.3 为Paddle编写单元测试
    - 测试挡板在左边界时继续左移（边缘情况）
    - 测试挡板在右边界时继续右移（边缘情况）
    - 测试挡板响应输入移动
    - _需求: 2.2, 2.3, 2.4_

  - [x] 2.4 实现Ball弹球物理行为
    - 创建`Scripts/GameObjects/Ball.cs`
    - 添加CircleCollider2D和Rigidbody2D组件
    - 实现Launch()以随机角度发射弹球
    - 实现NormalizeVelocity()保持恒定速度
    - 实现AdjustAngle()调整反弹角度避免过小角度
    - 实现底部边界检测和BallFell事件触发
    - _需求: 3.1, 3.2, 3.6_

  - [ ]* 2.5 为Ball编写属性测试
    - **属性 8: 弹球恒定速度不变量**
    - **验证: 需求 3.2**
    - **属性 2: 球掉落触发游戏结束**
    - **验证: 需求 1.3, 3.6**

  - [ ]* 2.6 为Ball编写单元测试
    - 测试游戏场景开始时球从挡板上方发射（需求 3.1）
    - 测试弹球速度异常时的归一化处理
    - 测试底部边界检测
    - _需求: 3.1, 3.2, 3.6_

  - [x] 2.7 实现Ball与Paddle碰撞交互
    - 在Ball.cs中实现OnCollisionEnter2D处理与Paddle碰撞
    - 根据碰撞位置计算水平速度分量
    - 确保Y方向速度反转为向上
    - 触发BallPaddleCollision事件
    - _需求: 2.5, 3.4_

  - [ ]* 2.8 为Ball-Paddle碰撞编写属性测试
    - **属性 7: 挡板反弹弹球**
    - **验证: 需求 2.5**
    - **属性 10: 挡板碰撞改变反弹角度**
    - **验证: 需求 3.4**

  - [x] 2.9 实现Ball与墙壁碰撞交互
    - 在Ball.cs中处理与Boundary层碰撞
    - 验证物理反弹规则（入射角等于反射角）
    - _需求: 3.3, 8.4_

  - [ ]* 2.10 为Ball-Wall碰撞编写属性测试
    - **属性 9: 墙壁反弹物理规则**
    - **验证: 需求 3.3, 8.4**

- [ ] 3. 检查点 - 确保基础物理交互正常
  - 确保所有测试通过，如有问题请询问用户。

- [ ] 4. 实现砖块系统
  - [x] 4.1 实现Brick砖块组件
    - 创建`Scripts/GameObjects/Brick.cs`
    - 添加BoxCollider2D和SpriteRenderer组件
    - 实现TakeDamage()和DestroyBrick()方法
    - 在OnCollisionEnter2D中处理与Ball碰撞
    - 触发BrickDestroyed事件
    - _需求: 3.5, 4.2, 4.3_

  - [ ]* 4.2 为Brick编写属性测试
    - **属性 11: 砖块碰撞销毁和反弹**
    - **验证: 需求 3.5, 4.2**

  - [ ]* 4.3 为Brick编写单元测试
    - 测试砖块被弹球碰撞后销毁
    - 测试砖块销毁触发事件
    - 测试多生命值砖块
    - _需求: 4.2, 4.3_

  - [x] 4.4 实现BrickGrid砖块网格生成器
    - 创建`Scripts/GameObjects/BrickGrid.cs`
    - 实现GenerateGrid()根据配置生成砖块网格
    - 实现SpawnBrick()实例化单个砖块
    - 添加预制体缺失的错误处理
    - _需求: 4.1_

  - [ ]* 4.5 为BrickGrid编写单元测试
    - 测试游戏场景加载时生成砖块网格（需求 4.1）
    - 测试砖块预制体缺失时的错误处理
    - 测试无效网格尺寸的默认值处理
    - _需求: 4.1_

  - [x] 4.6 创建Brick预制体
    - 在Unity编辑器中创建Brick预制体
    - 配置BoxCollider2D、SpriteRenderer和Brick脚本
    - 设置默认颜色和生命值
    - 分配到BrickGrid的BrickPrefab字段
    - _需求: 4.1, 4.2_

- [ ] 5. 实现边界墙系统
  - [ ] 5.1 创建Boundary边界墙
    - 在GameScene中创建左、右、顶部边界墙GameObject
    - 添加BoxCollider2D组件并配置为Boundary层
    - 分配Boundary物理材质
    - 设置位置和尺寸覆盖游戏区域
    - _需求: 8.1, 8.2, 8.3, 8.4_

  - [ ]* 5.2 为Boundary编写单元测试
    - 测试游戏场景包含左侧边界墙（需求 8.1）
    - 测试游戏场景包含右侧边界墙（需求 8.2）
    - 测试游戏场景包含顶部边界墙（需求 8.3）
    - 测试游戏场景定义底部边界（需求 8.5）
    - _需求: 8.1, 8.2, 8.3, 8.5_

- [ ] 6. 实现GameManager游戏流程管理
  - [x] 6.1 实现GameManager核心逻辑
    - 创建`Scripts/Core/GameManager.cs`单例类
    - 定义GameState枚举（StartMenu, Playing, GameOver, Victory）
    - 实现场景加载方法：LoadStartMenu(), LoadGameScene(), RestartGame()
    - 实现游戏流程控制：StartGame(), EndGame()
    - 实现砖块计数：RegisterBrick(), UnregisterBrick(), GetRemainingBricks()
    - _需求: 1.1, 1.2, 1.3, 1.4, 1.5, 4.4, 4.5_

  - [x] 6.2 订阅游戏事件并处理游戏状态
    - 订阅BrickDestroyed事件，减少砖块计数并检查胜利条件
    - 订阅BallFell事件，触发游戏结束
    - 实现场景加载错误处理和回退机制
    - _需求: 1.3, 1.4, 3.6, 4.5_

  - [ ]* 6.3 为GameManager编写属性测试
    - **属性 1: 场景切换正确性**
    - **验证: 需求 1.2, 1.5, 6.3, 7.4, 7.6**
    - **属性 3: 所有砖块被销毁触发胜利**
    - **验证: 需求 1.4, 4.5**

  - [ ]* 6.4 为GameManager编写单元测试
    - 测试游戏启动显示开始菜单（需求 1.1）
    - 测试最后一个砖块被销毁触发胜利（边缘情况）
    - 测试场景加载失败的错误处理
    - _需求: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ] 7. 检查点 - 确保核心游戏逻辑完整
  - 确保所有测试通过，如有问题请询问用户。

- [ ] 8. 实现UI系统
  - [x] 8.1 实现StartMenuUI开始菜单
    - 创建`Scripts/UI/StartMenuUI.cs`
    - 在StartMenu场景中创建Canvas和UI元素
    - 添加游戏标题Text、开始按钮Button、操作说明Text
    - 实现OnStartButtonClicked()调用GameManager.LoadGameScene()
    - _需求: 1.2, 6.1, 6.2, 6.3, 6.4_

  - [ ]* 8.2 为StartMenuUI编写单元测试
    - 测试开始菜单显示游戏标题（需求 6.1）
    - 测试开始菜单显示开始按钮（需求 6.2）
    - 测试开始菜单显示操作说明（需求 6.4）
    - 测试点击开始按钮触发场景加载（需求 6.3）
    - _需求: 6.1, 6.2, 6.3, 6.4_

  - [x] 8.3 实现GameUI游戏界面HUD
    - 创建`Scripts/UI/GameUI.cs`
    - 在GameScene的Canvas中添加ScoreText
    - 实现UpdateScore()更新分数显示
    - 订阅ScoreChanged事件并更新UI
    - 添加UI组件引用缺失的错误处理
    - _需求: 5.3_

  - [ ]* 8.4 为GameUI编写属性测试
    - **属性 13: 游戏中UI显示当前分数**
    - **验证: 需求 5.3**

  - [ ]* 8.5 为GameUI编写单元测试
    - 测试分数变化时UI实时更新
    - 测试UI组件引用缺失时的错误处理
    - _需求: 5.3_

  - [x] 8.6 实现EndScreenUI结束界面
    - 创建`Scripts/UI/EndScreenUI.cs`
    - 在GameScene的Canvas中创建EndScreenPanel（初始隐藏）
    - 添加结束提示Text、最终分数Text、重新开始Button、返回主菜单Button
    - 实现Show()方法显示结束界面并更新文本
    - 实现OnRestartButtonClicked()调用GameManager.RestartGame()
    - 实现OnMainMenuButtonClicked()调用GameManager.LoadStartMenu()
    - _需求: 1.3, 1.4, 1.5, 5.4, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_

  - [ ]* 8.7 为EndScreenUI编写属性测试
    - **属性 14: 结束界面显示最终分数**
    - **验证: 需求 5.4, 7.2**
    - **属性 15: 返回主菜单场景切换**
    - **验证: 需求 7.6**

  - [ ]* 8.8 为EndScreenUI编写单元测试
    - 测试结束界面显示结束提示（需求 7.1）
    - 测试结束界面显示重新开始按钮（需求 7.3）
    - 测试结束界面显示返回主菜单按钮（需求 7.5）
    - 测试点击重新开始按钮重新加载游戏场景（需求 7.4）
    - 测试点击返回主菜单按钮加载开始菜单场景（需求 7.6）
    - _需求: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_

- [ ] 9. 集成和场景配置
  - [ ] 9.1 配置GameScene游戏场景
    - 在GameScene中放置所有游戏对象：Ball、Paddle、BrickGrid、Boundaries
    - 配置Camera的正交大小和位置
    - 设置GameManager和ScoreSystem为DontDestroyOnLoad
    - 配置所有GameObject的Layer和Tag
    - 连接所有脚本的公共字段引用
    - _需求: 1.1, 1.2, 2.1, 3.1, 4.1, 8.1, 8.2, 8.3, 8.5_

  - [ ] 9.2 配置StartMenu场景
    - 设置StartMenuUI的所有UI元素
    - 配置按钮的OnClick事件
    - 调整UI布局和样式
    - _需求: 1.1, 6.1, 6.2, 6.3, 6.4_

  - [ ] 9.3 测试完整游戏流程
    - 手动测试从开始菜单到游戏场景的流程
    - 测试游戏胜利和失败的流程
    - 测试重新开始和返回主菜单功能
    - 验证所有UI元素正确显示和交互
    - _需求: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ] 10. 最终检查点 - 确保所有测试通过
  - 确保所有测试通过，如有问题请询问用户。

## 注意事项

- 标记为`*`的任务是可选的测试任务，可以跳过以加快MVP开发
- 每个任务都引用了具体的需求编号以确保可追溯性
- 检查点任务确保增量验证和及时发现问题
- 属性测试验证通用正确性属性，单元测试验证具体示例和边缘情况
- 所有代码使用C#编写，基于Unity 2022.3 LTS
- 物理交互使用Unity 2D Physics系统
- UI系统使用Unity UI (uGUI)
