# 调试说明 / Debug Instructions

## 🔍 如何查看详细日志

如果遇到NullReferenceException错误，请按照以下步骤查看详细日志：

### 步骤1: 打开Console窗口
```
Window → General → Console
或按快捷键: Ctrl+Shift+C (Windows) / Cmd+Shift+C (Mac)
```

### 步骤2: 清空Console
```
点击Console窗口左上角的"Clear"按钮
```

### 步骤3: 运行设置工具
```
Breakout → Setup GameScene
```

### 步骤4: 查看日志输出

**正常情况下，你应该看到：**
```
[GameSceneSetup] Starting GameScene setup...
[GameSceneSetup] Paddle created
[GameSceneSetup] Ball created
[GameSceneSetup] Boundaries created
[GameSceneSetup] Camera configured
[GameSceneSetup] Canvas and ScoreText created
[GameSceneSetup] GameScene setup completed!
```

**如果GameConfig不存在，你会看到：**
```
[GameSceneSetup] GameConfig not found, creating default config...
[GameSceneSetup] Creating default GameConfig...
[GameSceneSetup] Resources folder created (如果不存在)
[GameSceneSetup] GameConfig instance created
[GameSceneSetup] GameConfig saved to Assets/Resources/GameConfig.asset
[GameSceneSetup] GameConfig created and verified successfully
[GameSceneSetup] Starting GameScene setup...
...
```

**如果出现错误，你会看到：**
```
[GameSceneSetup] Config is null in CreatePaddle!
或
[GameSceneSetup] Failed to create ScriptableObject instance!
或
[GameSceneSetup] Exception while creating GameConfig: ...
```

## 🐛 常见错误及解决方案

### 错误1: "Config is null in CreatePaddle"

**原因**: GameConfig创建失败

**解决方案**:
1. 检查Console中是否有"Failed to create GameConfig"错误
2. 手动创建GameConfig:
   ```
   1. 右键 Assets 文件夹
   2. Create → Folder → 命名为 "Resources"
   3. 右键 Resources 文件夹
   4. Create → Breakout → GameConfig
   5. 重新运行 Breakout → Setup GameScene
   ```

### 错误2: "Failed to create ScriptableObject instance"

**原因**: GameConfig.cs脚本有编译错误

**解决方案**:
1. 检查Console中是否有编译错误
2. 确保Assets/Scripts/Data/GameConfig.cs文件存在
3. 确保GameConfig.cs没有语法错误
4. 点击菜单: Assets → Reimport All

### 错误3: "Failed to load created GameConfig"

**原因**: 文件保存失败或权限问题

**解决方案**:
1. 检查Assets/Resources文件夹是否存在
2. 检查是否有文件权限问题
3. 尝试手动创建GameConfig（见错误1的解决方案）

## 📋 完整的调试检查清单

运行设置工具前，请确认：

- [ ] Unity版本是2022.3 LTS或更高
- [ ] 没有编译错误（Console中没有红色错误）
- [ ] Assets/Scripts/Data/GameConfig.cs文件存在
- [ ] 项目没有只读权限
- [ ] 有足够的磁盘空间

## 🔧 高级调试

### 查看完整的堆栈跟踪

1. 在Console中双击错误信息
2. 会跳转到出错的代码行
3. 查看变量值和调用堆栈

### 启用详细日志

在GameSceneSetup.cs的SetupGameScene方法开始处添加：
```csharp
Debug.Log($"[GameSceneSetup] Unity Version: {Application.unityVersion}");
Debug.Log($"[GameSceneSetup] Project Path: {Application.dataPath}");
```

### 手动测试GameConfig创建

在Unity编辑器中：
```
1. 右键 Assets/Resources 文件夹
2. Create → Breakout → GameConfig
3. 如果成功，说明GameConfig.cs脚本正常
4. 如果失败，说明GameConfig.cs有问题
```

## 💡 预防性措施

为了避免问题：

1. **保持Unity更新**: 使用最新的2022.3 LTS版本
2. **定期保存**: 修改后按Ctrl+S保存
3. **检查编译错误**: 运行工具前确保没有编译错误
4. **备份项目**: 定期备份整个项目文件夹
5. **使用版本控制**: 使用Git管理代码

## 📞 仍然有问题？

如果以上方法都不起作用：

1. **查看完整的错误信息**: 
   - 在Console中右键错误
   - 选择"Copy"
   - 粘贴到文本编辑器查看完整信息

2. **检查文件完整性**:
   ```
   确认以下文件存在:
   - Assets/Scripts/Data/GameConfig.cs
   - Assets/Scripts/Editor/GameSceneSetup.cs
   - Assets/Scripts/Core/GameManager.cs
   - Assets/Scripts/Core/GameEvents.cs
   - Assets/Scripts/Core/ScoreSystem.cs
   - Assets/Scripts/GameObjects/Paddle.cs
   - Assets/Scripts/GameObjects/Ball.cs
   ```

3. **重新导入脚本**:
   ```
   1. 选中 Assets/Scripts 文件夹
   2. 右键 → Reimport
   3. 等待Unity重新编译
   4. 重新运行设置工具
   ```

4. **清理并重建**:
   ```
   1. 关闭Unity
   2. 删除 Library 文件夹
   3. 删除 Temp 文件夹
   4. 重新打开Unity
   5. 等待重新导入完成
   6. 重新运行设置工具
   ```

## 📚 相关文档

- `Troubleshooting_Guide.md` - 故障排除指南
- `SETUP_INSTRUCTIONS.md` - 设置说明
- `FINAL_SETUP_SUMMARY.md` - 最终设置总结

---

**记住**: 详细的日志是你最好的朋友！总是先查看Console输出。🔍
