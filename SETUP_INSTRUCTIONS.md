# Unity弹球打砖块游戏 - 快速设置指南

## 🚀 一键设置（推荐）

### 第一步：设置项目Tags和Layers
```
点击菜单: Breakout → Setup Project (Tags & Layers)
```
这会自动创建所有需要的Tags（Paddle, Ball, Brick）

### 第二步：设置StartMenu场景
```
1. 打开 Assets/Scenes/StartMenu.unity
2. 点击菜单: Breakout → Setup StartMenu Scene
3. 保存场景 (Ctrl+S)
```

### 第三步：设置GameScene场景
```
1. 打开 Assets/Scenes/GameScene.unity
2. 点击菜单: Breakout → Setup GameScene
3. 保存场景 (Ctrl+S)
```
**注意**: 如果GameConfig不存在，工具会自动创建！

### 第四步：测试游戏
```
1. 打开 Assets/Scenes/StartMenu.unity
2. 点击Play按钮
3. 点击"开始游戏"按钮
4. 使用A/D或左右方向键控制挡板
```

## ✅ 完成！

现在你有一个可玩的基础打砖块游戏了！

## 🎮 控制说明

- **A** 或 **←** : 挡板向左移动
- **D** 或 **→** : 挡板向右移动

## 📋 可用的编辑器工具

| 菜单项 | 功能 |
|--------|------|
| Breakout → Setup Project (Tags & Layers) | 创建Tags和验证Layers |
| Breakout → Setup StartMenu Scene | 创建StartMenu UI |
| Breakout → Setup GameScene | 创建GameScene游戏对象 |
| Breakout → Validate StartMenu Scene | 验证StartMenu配置 |

## 🐛 遇到问题？

### 问题：Tag 'Paddle' is not defined
**解决方案**: 运行 `Breakout → Setup Project (Tags & Layers)`

### 问题：UI不显示
**解决方案**: 确认Canvas的Render Mode为Screen Space - Overlay

### 问题：挡板不移动
**解决方案**: 从StartMenu启动游戏，不要直接运行GameScene

### 问题：球不发射
**解决方案**: 确认GameManager存在并且游戏状态为Playing

## 📚 详细文档

- `Quick_Setup_Reference.md` - 快速参考
- `GameScene_Auto_Setup_Instructions.md` - GameScene详细说明
- `StartMenu_Setup_Instructions.md` - StartMenu详细说明
- `Development_Progress_Summary.md` - 开发进度总结

## 🎉 下一步

游戏基础框架已完成，接下来可以：

1. 实现砖块系统（Brick + BrickGrid）
2. 实现UI系统（GameUI + EndScreenUI）
3. 添加音效和粒子效果
4. 创建更多关卡

祝你开发愉快！🎮
