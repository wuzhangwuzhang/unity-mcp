using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BreakoutGame.Editor
{
    /// <summary>
    /// GameScene场景自动设置工具 / GameScene scene auto-setup tool
    /// </summary>
    public class GameSceneSetup : EditorWindow
    {
        [MenuItem("Breakout/Setup GameScene")]
        public static void SetupGameScene()
        {
            // 打开GameScene场景 / Open GameScene
            EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
            
            // 加载或创建GameConfig / Load or create GameConfig
            var config = Resources.Load<Data.GameConfig>("GameConfig");
            if (config == null)
            {
                Debug.LogWarning("[GameSceneSetup] GameConfig not found, creating default config...");
                config = CreateDefaultGameConfig();
                
                if (config == null)
                {
                    Debug.LogError("[GameSceneSetup] Failed to create GameConfig!");
                    EditorUtility.DisplayDialog("Error", 
                        "Failed to create GameConfig!\nPlease check the Console for details.", 
                        "OK");
                    return;
                }
            }
            
            Debug.Log("[GameSceneSetup] Starting GameScene setup...");
            
            // 0. 创建GameManager、ScoreSystem和GameProgress / Create GameManager, ScoreSystem and GameProgress
            CreateGameManager();
            CreateGameProgress();
            CreateScoreSystem();
            
            // 1. 创建Paddle / Create Paddle
            CreatePaddle(config);
            
            // 2. 创建Ball / Create Ball
            CreateBall(config);
            
            // 3. 创建Boundaries / Create Boundaries
            CreateBoundaries(config);
            
            // 4. 配置Camera / Configure Camera
            ConfigureCamera(config);
            
            // 5. 创建Brick预制体和BrickGrid / Create Brick prefab and BrickGrid
            CreateBrickPrefabAndGrid(config);
            
            // 6. 创建Canvas (UI) / Create Canvas
            CreateGameCanvas();
            
            // 7. 创建EventSystem / Create EventSystem
            CreateEventSystem();
            
            // 标记场景为已修改 / Mark scene as modified
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("[GameSceneSetup] GameScene setup completed!");
            EditorUtility.DisplayDialog("Setup Complete", 
                "GameScene has been set up successfully!\n\n" +
                "Objects created:\n" +
                "- GameManager\n" +
                "- ScoreSystem\n" +
                "- Paddle\n" +
                "- Ball\n" +
                "- Boundaries (Left, Right, Top)\n" +
                "- BrickGrid with Brick Prefab\n" +
                "- Canvas (UI)\n\n" +
                "Please save the scene (Ctrl+S) and test!", 
                "OK");
        }
        
        /// <summary>
        /// 创建GameManager / Create GameManager
        /// </summary>
        private static void CreateGameManager()
        {
            // 删除已存在的GameManager / Delete existing GameManager
            var existingManager = GameObject.Find("GameManager");
            if (existingManager != null)
            {
                DestroyImmediate(existingManager);
                Debug.Log("[GameSceneSetup] Removed existing GameManager");
            }
            
            // 创建GameManager GameObject / Create GameManager GameObject
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<Core.GameManager>();
            
            Debug.Log("[GameSceneSetup] GameManager created");
        }
        
        /// <summary>
        /// 创建ScoreSystem / Create ScoreSystem
        /// </summary>
        private static void CreateScoreSystem()
        {
            // 删除已存在的ScoreSystem / Delete existing ScoreSystem
            var existingScoreSystem = GameObject.Find("ScoreSystem");
            if (existingScoreSystem != null)
            {
                DestroyImmediate(existingScoreSystem);
                Debug.Log("[GameSceneSetup] Removed existing ScoreSystem");
            }
            
            // 创建ScoreSystem GameObject / Create ScoreSystem GameObject
            GameObject scoreSystem = new GameObject("ScoreSystem");
            scoreSystem.AddComponent<Core.ScoreSystem>();
            
            Debug.Log("[GameSceneSetup] ScoreSystem created");
        }
        
        /// <summary>
        /// 创建GameProgress / Create GameProgress
        /// </summary>
        private static void CreateGameProgress()
        {
            // 删除已存在的GameProgress / Delete existing GameProgress
            var existingProgress = GameObject.Find("GameProgress");
            if (existingProgress != null)
            {
                DestroyImmediate(existingProgress);
                Debug.Log("[GameSceneSetup] Removed existing GameProgress");
            }
            
            // 创建GameProgress GameObject / Create GameProgress GameObject
            GameObject gameProgress = new GameObject("GameProgress");
            gameProgress.AddComponent<Data.GameProgress>();
            
            Debug.Log("[GameSceneSetup] GameProgress created");
        }
        
        private static void CreatePaddle(Data.GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSceneSetup] Config is null in CreatePaddle!");
                return;
            }
            
            try
            {
                // 删除已存在的Paddle / Delete existing Paddle
                var existingPaddle = GameObject.Find("Paddle");
                if (existingPaddle != null)
                {
                    DestroyImmediate(existingPaddle);
                    Debug.Log("[GameSceneSetup] Removed existing Paddle");
                }
                
                // 创建Paddle GameObject / Create Paddle GameObject
                Debug.Log("[GameSceneSetup] Creating Paddle GameObject...");
                GameObject paddle = new GameObject("Paddle");
                
                if (paddle == null)
                {
                    Debug.LogError("[GameSceneSetup] Failed to create Paddle GameObject!");
                    return;
                }
                
                Debug.Log("[GameSceneSetup] Setting Paddle transform...");
                paddle.transform.position = new Vector3(0f, -5f, 0f);
                paddle.transform.localScale = new Vector3(config.paddleWidth, config.paddleHeight, 1f);
                paddle.layer = LayerMask.NameToLayer("Paddle");
                SetTagSafely(paddle, "Paddle");
                
                // 添加BoxCollider2D和Rigidbody2D（必须在Paddle脚本之前）
                // Add BoxCollider2D and Rigidbody2D (must be before Paddle script due to RequireComponent)
                Debug.Log("[GameSceneSetup] Adding BoxCollider2D...");
                var boxCollider = paddle.AddComponent<BoxCollider2D>();
                boxCollider.size = new Vector2(1f, 1f);
                
                Debug.Log("[GameSceneSetup] Adding Rigidbody2D...");
                var rb = paddle.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
                
                // 添加Paddle脚本（RequireComponent会验证依赖）
                // Add Paddle script (RequireComponent will validate dependencies)
                Debug.Log("[GameSceneSetup] Adding Paddle script...");
                paddle.AddComponent<GameObjects.Paddle>();
                
                // 添加SpriteRenderer用于显示 / Add SpriteRenderer for display
                Debug.Log("[GameSceneSetup] Adding SpriteRenderer...");
                var spriteRenderer = paddle.AddComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogError("[GameSceneSetup] Failed to add SpriteRenderer!");
                    return;
                }
                spriteRenderer.color = Color.white;
                var sprite = CreateSimpleSprite();
                if (sprite != null)
                {
                    spriteRenderer.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("[GameSceneSetup] Failed to create sprite for Paddle, using default");
                }
                
                // 加载并分配物理材质 / Load and assign physics material
                Debug.Log("[GameSceneSetup] Loading physics material...");
                var paddleMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/PhysicsMaterials/PaddlePhysicsMaterial.physicsMaterial2D");
                if (paddleMaterial != null)
                {
                    boxCollider.sharedMaterial = paddleMaterial;
                    Debug.Log("[GameSceneSetup] Physics material assigned");
                }
                else
                {
                    Debug.LogWarning("[GameSceneSetup] PaddlePhysicsMaterial not found");
                }
                
                Debug.Log("[GameSceneSetup] Paddle created successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameSceneSetup] Exception in CreatePaddle: {e.Message}");
                Debug.LogError($"[GameSceneSetup] Stack trace: {e.StackTrace}");
            }
        }
        
        private static void CreateBall(Data.GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSceneSetup] Config is null in CreateBall!");
                return;
            }
            
            // 删除已存在的Ball / Delete existing Ball
            var existingBall = GameObject.Find("Ball");
            if (existingBall != null)
            {
                DestroyImmediate(existingBall);
            }
            
            // 创建Ball GameObject / Create Ball GameObject
            GameObject ball = new GameObject("Ball");
            ball.transform.position = new Vector3(0f, -4f, 0f);
            ball.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            ball.layer = LayerMask.NameToLayer("Ball");
            SetTagSafely(ball, "Ball");
            
            // 添加CircleCollider2D和Rigidbody2D（必须在Ball脚本之前）
            // Add CircleCollider2D and Rigidbody2D (must be before Ball script due to RequireComponent)
            var circleCollider = ball.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;
            
            var rb = ball.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // 添加Ball脚本（RequireComponent会验证依赖）
            // Add Ball script (RequireComponent will validate dependencies)
            ball.AddComponent<GameObjects.Ball>();
            
            // 添加SpriteRenderer用于显示 / Add SpriteRenderer for display
            var spriteRenderer = ball.AddComponent<SpriteRenderer>();
            spriteRenderer.color = Color.cyan;
            var sprite = CreateCircleSprite();
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("[GameSceneSetup] Failed to create sprite for Ball, using default");
            }
            
            // 加载并分配物理材质 / Load and assign physics material
            var ballMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/PhysicsMaterials/BallPhysicsMaterial.physicsMaterial2D");
            if (ballMaterial != null)
            {
                circleCollider.sharedMaterial = ballMaterial;
            }
            
            Debug.Log("[GameSceneSetup] Ball created");
        }
        
        private static void CreateBoundaries(Data.GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSceneSetup] Config is null in CreateBoundaries!");
                return;
            }
            
            // 删除已存在的Boundaries / Delete existing Boundaries
            var existingBoundaries = GameObject.Find("Boundaries");
            if (existingBoundaries != null)
            {
                DestroyImmediate(existingBoundaries);
            }
            
            // 创建Boundaries父对象 / Create Boundaries parent
            GameObject boundaries = new GameObject("Boundaries");
            
            // 加载边界物理材质 / Load boundary physics material
            var boundaryMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/PhysicsMaterials/BoundaryPhysicsMaterial.physicsMaterial2D");
            
            // 创建LeftWall / Create LeftWall
            CreateWall("LeftWall", boundaries.transform, 
                new Vector3(config.leftBoundary, 0f, 0f), 
                new Vector3(config.boundaryThickness, 12f, 1f),
                boundaryMaterial);
            
            // 创建RightWall / Create RightWall
            CreateWall("RightWall", boundaries.transform, 
                new Vector3(config.rightBoundary, 0f, 0f), 
                new Vector3(config.boundaryThickness, 12f, 1f),
                boundaryMaterial);
            
            // 创建TopWall / Create TopWall
            CreateWall("TopWall", boundaries.transform, 
                new Vector3(0f, config.topBoundary, 0f), 
                new Vector3(20f, config.boundaryThickness, 1f),
                boundaryMaterial);
            
            Debug.Log("[GameSceneSetup] Boundaries created");
        }
        
        private static void CreateWall(string name, Transform parent, Vector3 position, Vector3 scale, PhysicsMaterial2D material)
        {
            GameObject wall = new GameObject(name);
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.layer = LayerMask.NameToLayer("Boundary");
            
            // 添加BoxCollider2D / Add BoxCollider2D
            var boxCollider = wall.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1f);
            if (material != null)
            {
                boxCollider.sharedMaterial = material;
            }
            
            // 添加SpriteRenderer用于显示 / Add SpriteRenderer for display
            var spriteRenderer = wall.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f); // 灰色 / Gray
            var sprite = CreateSimpleSprite();
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }
        
        private static void ConfigureCamera(Data.GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSceneSetup] Config is null in ConfigureCamera!");
                return;
            }
            
            // 查找Main Camera / Find Main Camera
            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogWarning("[GameSceneSetup] Main Camera not found, creating one");
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                camera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
            }
            
            // 配置Camera / Configure Camera
            camera.orthographic = true;
            camera.orthographicSize = config.cameraSize;
            camera.transform.position = config.cameraPosition;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.18f); // 深色背景 / Dark background
            
            Debug.Log("[GameSceneSetup] Camera configured");
        }

        /// <summary>
        /// 创建Brick预制体和BrickGrid / Create Brick prefab and BrickGrid
        /// </summary>
        private static void CreateBrickPrefabAndGrid(Data.GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[GameSceneSetup] Config is null in CreateBrickPrefabAndGrid!");
                return;
            }

            // 确保Prefabs文件夹存在 / Ensure Prefabs folder exists
            string prefabsPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabsPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
                AssetDatabase.Refresh();
                Debug.Log("[GameSceneSetup] Prefabs folder created");
            }

            // 创建或加载Brick预制体 / Create or load Brick prefab
            string prefabPath = "Assets/Prefabs/Brick.prefab";
            GameObject brickPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (brickPrefab == null)
            {
                Debug.Log("[GameSceneSetup] Creating Brick prefab...");

                // 创建临时Brick GameObject / Create temporary Brick GameObject
                GameObject tempBrick = new GameObject("Brick");
                tempBrick.layer = LayerMask.NameToLayer("Brick");
                SetTagSafely(tempBrick, "Brick");

                // 添加BoxCollider2D / Add BoxCollider2D
                var boxCollider = tempBrick.AddComponent<BoxCollider2D>();
                boxCollider.size = new Vector2(1f, 1f);

                // 添加SpriteRenderer / Add SpriteRenderer
                var spriteRenderer = tempBrick.AddComponent<SpriteRenderer>();
                spriteRenderer.color = Color.red;
                var sprite = CreateSimpleSprite();
                if (sprite != null)
                {
                    spriteRenderer.sprite = sprite;
                }

                // 添加Brick脚本 / Add Brick script
                tempBrick.AddComponent<GameObjects.Brick>();

                // 保存为预制体 / Save as prefab
                brickPrefab = PrefabUtility.SaveAsPrefabAsset(tempBrick, prefabPath);

                // 删除临时对象 / Delete temporary object
                DestroyImmediate(tempBrick);

                Debug.Log($"[GameSceneSetup] Brick prefab created at {prefabPath}");
            }
            else
            {
                Debug.Log("[GameSceneSetup] Brick prefab already exists");
            }

            // 删除已存在的BrickGrid / Delete existing BrickGrid
            var existingGrid = GameObject.Find("BrickGrid");
            if (existingGrid != null)
            {
                DestroyImmediate(existingGrid);
            }

            // 创建BrickGrid GameObject / Create BrickGrid GameObject
            GameObject brickGrid = new GameObject("BrickGrid");
            brickGrid.transform.position = Vector3.zero;

            // 添加BrickGrid脚本 / Add BrickGrid script
            var gridScript = brickGrid.AddComponent<GameObjects.BrickGrid>();

            // 使用反射设置私有字段（因为是SerializeField）
            // Use reflection to set private field (because it's SerializeField)
            var brickPrefabField = typeof(GameObjects.BrickGrid).GetField("brickPrefab",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (brickPrefabField != null)
            {
                brickPrefabField.SetValue(gridScript, brickPrefab);
                Debug.Log("[GameSceneSetup] BrickGrid prefab reference assigned");
            }
            else
            {
                Debug.LogWarning("[GameSceneSetup] Could not find brickPrefab field via reflection");
            }

            Debug.Log("[GameSceneSetup] BrickGrid created");
        }

        
        private static void CreateGameCanvas()
        {
            // 删除已存在的Canvas / Delete existing Canvas
            var existingCanvas = GameObject.Find("Canvas");
            if (existingCanvas != null)
            {
                DestroyImmediate(existingCanvas);
            }
            
            // 创建Canvas / Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<UnityEngine.Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f; // 0=宽度优先，适合横屏游戏 / 0=width priority, suitable for landscape games
            
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // 创建ScoreText / Create ScoreText
            GameObject scoreTextObj = new GameObject("ScoreText");
            scoreTextObj.transform.SetParent(canvasObj.transform, false);
            
            var scoreText = scoreTextObj.AddComponent<UnityEngine.UI.Text>();
            scoreText.text = "Score: 0";
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 36;
            scoreText.alignment = TextAnchor.UpperLeft;
            scoreText.color = Color.white;
            scoreText.resizeTextForBestFit = true;
            scoreText.resizeTextMinSize = 20;
            scoreText.resizeTextMaxSize = 36;
            
            var scoreRect = scoreTextObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 1f);
            scoreRect.anchorMax = new Vector2(0.3f, 1f);
            scoreRect.pivot = new Vector2(0f, 1f);
            scoreRect.anchoredPosition = new Vector2(20f, -20f);
            scoreRect.sizeDelta = new Vector2(0f, 50f);
            
            Debug.Log("[GameSceneSetup] Canvas and ScoreText created");
            
            // 添加GameUI脚本 / Add GameUI script
            var gameUI = canvasObj.AddComponent<UI.GameUI>();
            
            // 使用反射设置scoreText引用 / Use reflection to set scoreText reference
            var scoreTextField = typeof(UI.GameUI).GetField("scoreText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (scoreTextField != null)
            {
                scoreTextField.SetValue(gameUI, scoreText);
                Debug.Log("[GameSceneSetup] GameUI script added and scoreText reference assigned");
            }
            else
            {
                Debug.LogWarning("[GameSceneSetup] Could not find scoreText field via reflection");
            }
            
            // 创建EndScreenPanel / Create EndScreenPanel
            CreateEndScreenPanel(canvasObj);
        }
        
        private static void CreateEventSystem()
        {
            // 检查EventSystem是否存在 / Check if EventSystem exists
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("[GameSceneSetup] EventSystem created");
            }
        }

        /// <summary>
        /// 创建结束界面面板 / Create end screen panel
        /// </summary>
        private static void CreateEndScreenPanel(GameObject canvasObj)
        {
            // 创建EndScreenPanel / Create EndScreenPanel
            GameObject panelObj = new GameObject("EndScreenPanel");
            panelObj.transform.SetParent(canvasObj.transform, false);

            // 添加Image组件作为背景 / Add Image component as background
            var panelImage = panelObj.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.8f); // 半透明黑色背景 / Semi-transparent black background

            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            // 创建MessageText / Create MessageText
            GameObject messageTextObj = new GameObject("MessageText");
            messageTextObj.transform.SetParent(panelObj.transform, false);

            var messageText = messageTextObj.AddComponent<UnityEngine.UI.Text>();
            messageText.text = "Game Over";
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 80;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.fontStyle = FontStyle.Bold;
            messageText.resizeTextForBestFit = true;
            messageText.resizeTextMinSize = 40;
            messageText.resizeTextMaxSize = 80;

            var messageRect = messageTextObj.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.2f, 0.7f);
            messageRect.anchorMax = new Vector2(0.8f, 0.85f);
            messageRect.offsetMin = Vector2.zero;
            messageRect.offsetMax = Vector2.zero;

            // 创建FinalScoreText / Create FinalScoreText
            GameObject finalScoreTextObj = new GameObject("FinalScoreText");
            finalScoreTextObj.transform.SetParent(panelObj.transform, false);

            var finalScoreText = finalScoreTextObj.AddComponent<UnityEngine.UI.Text>();
            finalScoreText.text = "Score: 0\nHigh Score: 0";
            finalScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            finalScoreText.fontSize = 40;
            finalScoreText.alignment = TextAnchor.MiddleCenter;
            finalScoreText.color = Color.yellow;
            finalScoreText.resizeTextForBestFit = true;
            finalScoreText.resizeTextMinSize = 24;
            finalScoreText.resizeTextMaxSize = 40;

            var finalScoreRect = finalScoreTextObj.GetComponent<RectTransform>();
            finalScoreRect.anchorMin = new Vector2(0.2f, 0.55f);
            finalScoreRect.anchorMax = new Vector2(0.8f, 0.68f);
            finalScoreRect.offsetMin = Vector2.zero;
            finalScoreRect.offsetMax = Vector2.zero;

            // 创建按钮容器 / Create button container
            GameObject buttonContainerObj = new GameObject("ButtonContainer");
            buttonContainerObj.transform.SetParent(panelObj.transform, false);
            
            var containerRect = buttonContainerObj.GetComponent<RectTransform>();
            if (containerRect == null)
            {
                containerRect = buttonContainerObj.AddComponent<RectTransform>();
            }
            containerRect.anchorMin = new Vector2(0.25f, 0.15f);
            containerRect.anchorMax = new Vector2(0.75f, 0.5f);
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            // 创建NextLevelButton（放在最上面）/ Create NextLevelButton (top position)
            GameObject nextLevelButtonObj = new GameObject("NextLevelButton");
            nextLevelButtonObj.transform.SetParent(buttonContainerObj.transform, false);

            var nextLevelButton = nextLevelButtonObj.AddComponent<UnityEngine.UI.Button>();
            var nextLevelButtonImage = nextLevelButtonObj.AddComponent<UnityEngine.UI.Image>();
            nextLevelButtonImage.color = new Color(0.2f, 0.6f, 1f); // 亮蓝色 / Bright blue

            var nextLevelButtonRect = nextLevelButtonObj.GetComponent<RectTransform>();
            nextLevelButtonRect.anchorMin = new Vector2(0.1f, 0.7f);
            nextLevelButtonRect.anchorMax = new Vector2(0.9f, 0.95f);
            nextLevelButtonRect.offsetMin = Vector2.zero;
            nextLevelButtonRect.offsetMax = Vector2.zero;

            // 创建NextLevelButton文本 / Create NextLevelButton text
            GameObject nextLevelTextObj = new GameObject("Text");
            nextLevelTextObj.transform.SetParent(nextLevelButtonObj.transform, false);

            var nextLevelText = nextLevelTextObj.AddComponent<UnityEngine.UI.Text>();
            nextLevelText.text = "Next Level";
            nextLevelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nextLevelText.fontSize = 36;
            nextLevelText.alignment = TextAnchor.MiddleCenter;
            nextLevelText.color = Color.white;
            nextLevelText.fontStyle = FontStyle.Bold;
            nextLevelText.resizeTextForBestFit = true;
            nextLevelText.resizeTextMinSize = 20;
            nextLevelText.resizeTextMaxSize = 36;

            var nextLevelTextRect = nextLevelTextObj.GetComponent<RectTransform>();
            nextLevelTextRect.anchorMin = Vector2.zero;
            nextLevelTextRect.anchorMax = Vector2.one;
            nextLevelTextRect.offsetMin = Vector2.zero;
            nextLevelTextRect.offsetMax = Vector2.zero;

            // 创建RestartButton（中间）/ Create RestartButton (middle)
            GameObject restartButtonObj = new GameObject("RestartButton");
            restartButtonObj.transform.SetParent(buttonContainerObj.transform, false);

            var restartButton = restartButtonObj.AddComponent<UnityEngine.UI.Button>();
            var restartButtonImage = restartButtonObj.AddComponent<UnityEngine.UI.Image>();
            restartButtonImage.color = new Color(0.3f, 0.8f, 0.3f); // 绿色 / Green

            var restartButtonRect = restartButtonObj.GetComponent<RectTransform>();
            restartButtonRect.anchorMin = new Vector2(0.1f, 0.4f);
            restartButtonRect.anchorMax = new Vector2(0.9f, 0.65f);
            restartButtonRect.offsetMin = Vector2.zero;
            restartButtonRect.offsetMax = Vector2.zero;

            // 创建RestartButton文本 / Create RestartButton text
            GameObject restartTextObj = new GameObject("Text");
            restartTextObj.transform.SetParent(restartButtonObj.transform, false);

            var restartText = restartTextObj.AddComponent<UnityEngine.UI.Text>();
            restartText.text = "Restart Game";
            restartText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            restartText.fontSize = 36;
            restartText.alignment = TextAnchor.MiddleCenter;
            restartText.color = Color.white;
            restartText.fontStyle = FontStyle.Bold;
            restartText.resizeTextForBestFit = true;
            restartText.resizeTextMinSize = 20;
            restartText.resizeTextMaxSize = 36;

            var restartTextRect = restartTextObj.GetComponent<RectTransform>();
            restartTextRect.anchorMin = Vector2.zero;
            restartTextRect.anchorMax = Vector2.one;
            restartTextRect.offsetMin = Vector2.zero;
            restartTextRect.offsetMax = Vector2.zero;

            // 创建MainMenuButton / Create MainMenuButton (bottom)
            GameObject mainMenuButtonObj = new GameObject("MainMenuButton");
            mainMenuButtonObj.transform.SetParent(buttonContainerObj.transform, false);

            var mainMenuButton = mainMenuButtonObj.AddComponent<UnityEngine.UI.Button>();
            var mainMenuButtonImage = mainMenuButtonObj.AddComponent<UnityEngine.UI.Image>();
            mainMenuButtonImage.color = new Color(0.8f, 0.3f, 0.3f); // 红色 / Red

            var mainMenuButtonRect = mainMenuButtonObj.GetComponent<RectTransform>();
            mainMenuButtonRect.anchorMin = new Vector2(0.1f, 0.05f);
            mainMenuButtonRect.anchorMax = new Vector2(0.9f, 0.35f);
            mainMenuButtonRect.offsetMin = Vector2.zero;
            mainMenuButtonRect.offsetMax = Vector2.zero;

            // 创建MainMenuButton文本 / Create MainMenuButton text
            GameObject mainMenuTextObj = new GameObject("Text");
            mainMenuTextObj.transform.SetParent(mainMenuButtonObj.transform, false);

            var mainMenuText = mainMenuTextObj.AddComponent<UnityEngine.UI.Text>();
            mainMenuText.text = "Main Menu";
            mainMenuText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            mainMenuText.fontSize = 36;
            mainMenuText.alignment = TextAnchor.MiddleCenter;
            mainMenuText.color = Color.white;
            mainMenuText.fontStyle = FontStyle.Bold;
            mainMenuText.resizeTextForBestFit = true;
            mainMenuText.resizeTextMinSize = 20;
            mainMenuText.resizeTextMaxSize = 36;

            var mainMenuTextRect = mainMenuTextObj.GetComponent<RectTransform>();
            mainMenuTextRect.anchorMin = Vector2.zero;
            mainMenuTextRect.anchorMax = Vector2.one;
            mainMenuTextRect.offsetMin = Vector2.zero;
            mainMenuTextRect.offsetMax = Vector2.zero;

            // 添加EndScreenUI脚本到Canvas / Add EndScreenUI script to Canvas
            var endScreenUI = canvasObj.AddComponent<UI.EndScreenUI>();

            // 使用反射设置UI引用 / Use reflection to set UI references
            SetEndScreenUIReferences(endScreenUI, panelObj, messageText, finalScoreText, restartButton, nextLevelButton, mainMenuButton);

            Debug.Log("[GameSceneSetup] EndScreenPanel created");
        }

        /// <summary>
        /// 设置EndScreenUI的引用 / Set EndScreenUI references
        /// </summary>
        private static void SetEndScreenUIReferences(UI.EndScreenUI endScreenUI, GameObject panel,
            UnityEngine.UI.Text messageText, UnityEngine.UI.Text finalScoreText,
            UnityEngine.UI.Button restartButton, UnityEngine.UI.Button nextLevelButton, UnityEngine.UI.Button mainMenuButton)
        {
            var type = typeof(UI.EndScreenUI);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            type.GetField("endScreenPanel", flags)?.SetValue(endScreenUI, panel);
            type.GetField("messageText", flags)?.SetValue(endScreenUI, messageText);
            type.GetField("finalScoreText", flags)?.SetValue(endScreenUI, finalScoreText);
            type.GetField("restartButton", flags)?.SetValue(endScreenUI, restartButton);
            type.GetField("nextLevelButton", flags)?.SetValue(endScreenUI, nextLevelButton);
            type.GetField("mainMenuButton", flags)?.SetValue(endScreenUI, mainMenuButton);

            Debug.Log("[GameSceneSetup] EndScreenUI references assigned");
        }

        
        /// <summary>
        /// 创建简单的方形Sprite / Create simple square sprite
        /// </summary>
        private static Sprite CreateSimpleSprite()
        {
            // 尝试使用Unity内置的默认Sprite / Try to use Unity's built-in default sprite
            var builtinSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (builtinSprite != null)
            {
                return builtinSprite;
            }
            
            // 如果内置Sprite不可用，创建简单的白色方块 / If built-in sprite not available, create simple white square
            try
            {
                Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[32 * 32];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.white;
                }
                texture.SetPixels(pixels);
                texture.Apply();
                
                return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameSceneSetup] Failed to create sprite: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 创建圆形Sprite / Create circle sprite
        /// </summary>
        private static Sprite CreateCircleSprite()
        {
            // 尝试使用Unity内置的Knob Sprite（圆形）/ Try to use Unity's built-in Knob sprite (circular)
            var builtinSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            if (builtinSprite != null)
            {
                return builtinSprite;
            }
            
            // 如果内置Sprite不可用，创建圆形 / If built-in sprite not available, create circle
            try
            {
                int size = 64;
                Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                Vector2 center = new Vector2(size / 2f, size / 2f);
                float radius = size / 2f - 1f;
                
                Color[] pixels = new Color[size * size];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), center);
                        pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
                    }
                }
                
                texture.SetPixels(pixels);
                texture.Apply();
                
                return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameSceneSetup] Failed to create circle sprite: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 安全地设置GameObject的Tag / Safely set GameObject tag
        /// </summary>
        private static void SetTagSafely(GameObject obj, string tagName)
        {
            try
            {
                obj.tag = tagName;
            }
            catch (UnityException)
            {
                Debug.LogWarning($"[GameSceneSetup] Tag '{tagName}' is not defined. Please add it in Edit → Project Settings → Tags and Layers");
                Debug.LogWarning($"[GameSceneSetup] GameObject '{obj.name}' will use 'Untagged' tag");
            }
        }
        
        /// <summary>
        /// 创建默认的GameConfig / Create default GameConfig
        /// </summary>
        private static Data.GameConfig CreateDefaultGameConfig()
        {
            try
            {
                Debug.Log("[GameSceneSetup] Creating default GameConfig...");
                
                // 确保Resources文件夹存在 / Ensure Resources folder exists
                string resourcesPath = "Assets/Resources";
                if (!AssetDatabase.IsValidFolder(resourcesPath))
                {
                    Debug.Log("[GameSceneSetup] Resources folder not found, creating...");
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.Refresh();
                    Debug.Log("[GameSceneSetup] Resources folder created");
                }
                
                // 创建GameConfig实例 / Create GameConfig instance
                var config = ScriptableObject.CreateInstance<Data.GameConfig>();
                
                if (config == null)
                {
                    Debug.LogError("[GameSceneSetup] Failed to create ScriptableObject instance!");
                    return null;
                }
                
                Debug.Log("[GameSceneSetup] GameConfig instance created");
                
                // 设置默认值（这些值已经在GameConfig.cs中定义）
                // Default values are already defined in GameConfig.cs
                
                // 保存为asset文件 / Save as asset file
                string assetPath = "Assets/Resources/GameConfig.asset";
                AssetDatabase.CreateAsset(config, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log($"[GameSceneSetup] GameConfig saved to {assetPath}");
                
                // 验证文件是否创建成功 / Verify file was created
                var loadedConfig = AssetDatabase.LoadAssetAtPath<Data.GameConfig>(assetPath);
                if (loadedConfig == null)
                {
                    Debug.LogError("[GameSceneSetup] Failed to load created GameConfig!");
                    return null;
                }
                
                Debug.Log("[GameSceneSetup] GameConfig created and verified successfully");
                return loadedConfig;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameSceneSetup] Exception while creating GameConfig: {e.Message}");
                Debug.LogError($"[GameSceneSetup] Stack trace: {e.StackTrace}");
                return null;
            }
        }
    }
}
