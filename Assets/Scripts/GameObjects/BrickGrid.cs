using UnityEngine;

namespace BreakoutGame.GameObjects
{
    /// <summary>
    /// 砖块网格生成器，负责生成和管理砖块布局
    /// Brick grid generator that creates and manages brick layout
    /// </summary>
    public class BrickGrid : MonoBehaviour
    {
        [Header("Grid Settings / 网格设置")]
        [SerializeField] private GameObject brickPrefab;
        [SerializeField] private int rows = 5;
        [SerializeField] private int columns = 8;
        [SerializeField] private float brickWidth = 1.5f;
        [SerializeField] private float brickHeight = 0.5f;
        [SerializeField] private float spacing = 0.1f;
        [SerializeField] private Vector2 gridStartPosition = new Vector2(-6f, 3f);
        
        [Header("Brick Colors / 砖块颜色")]
        [SerializeField] private Color[] rowColors = new Color[]
        {
            new Color(1f, 0f, 0f),      // 红色 / Red
            new Color(1f, 0.5f, 0f),    // 橙色 / Orange
            new Color(1f, 1f, 0f),      // 黄色 / Yellow
            new Color(0f, 1f, 0f),      // 绿色 / Green
            new Color(0f, 0.5f, 1f)     // 蓝色 / Blue
        };
        
        private void Start()
        {
            // 从GameConfig加载配置（如果存在）/ Load config from GameConfig if exists
            var config = Resources.Load<Data.GameConfig>("GameConfig");
            if (config != null)
            {
                // 获取当前关卡 / Get current level
                int currentLevel = 1;
                if (Core.GameManager.Instance != null)
                {
                    currentLevel = Core.GameManager.Instance.GetCurrentLevel();
                }
                
                // 加载关卡配置 / Load level config
                int levelIndex = currentLevel - 1;
                if (levelIndex >= 0 && levelIndex < config.levelConfigs.Length)
                {
                    var levelConfig = config.levelConfigs[levelIndex];
                    rows = levelConfig.rows;
                    columns = levelConfig.columns;
                    
                    Debug.Log($"[BrickGrid] Loaded level {currentLevel} config - Grid: {rows}x{columns}");
                }
                else
                {
                    // 使用默认配置 / Use default config
                    rows = config.brickRows;
                    columns = config.brickColumns;
                    
                    Debug.Log($"[BrickGrid] Using default config - Grid: {rows}x{columns}");
                }
                
                brickWidth = config.brickWidth;
                brickHeight = config.brickHeight;
                spacing = config.brickSpacing;
                gridStartPosition = config.brickStartPosition;
            }
            
            // 生成砖块网格 / Generate brick grid
            GenerateGrid();
        }
        
        /// <summary>
        /// 生成砖块网格 / Generate brick grid
        /// </summary>
        public void GenerateGrid()
        {
            // 验证预制体 / Validate prefab
            if (brickPrefab == null)
            {
                Debug.LogError("[BrickGrid] Brick prefab is not assigned! Cannot generate grid.");
                return;
            }
            
            // 验证网格尺寸 / Validate grid dimensions
            if (rows <= 0 || columns <= 0)
            {
                Debug.LogWarning($"[BrickGrid] Invalid grid dimensions: {rows}x{columns}. Using default 5x8.");
                rows = 5;
                columns = 8;
            }
            
            Debug.Log($"[BrickGrid] Generating {rows}x{columns} brick grid...");
            
            // 清除已存在的砖块 / Clear existing bricks
            ClearGrid();
            
            // 生成砖块 / Generate bricks
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    SpawnBrick(row, col);
                }
            }
            
            Debug.Log($"[BrickGrid] Generated {rows * columns} bricks");
        }
        
        /// <summary>
        /// 生成单个砖块 / Spawn a single brick
        /// </summary>
        private void SpawnBrick(int row, int col)
        {
            // 计算位置 / Calculate position
            float xPos = gridStartPosition.x + col * (brickWidth + spacing);
            float yPos = gridStartPosition.y - row * (brickHeight + spacing);
            Vector3 position = new Vector3(xPos, yPos, 0f);
            
            // 实例化砖块 / Instantiate brick
            GameObject brickObj = Instantiate(brickPrefab, position, Quaternion.identity, transform);
            brickObj.name = $"Brick_{row}_{col}";
            
            // 设置砖块尺寸 / Set brick size
            brickObj.transform.localScale = new Vector3(brickWidth, brickHeight, 1f);
            
            // 设置砖块颜色 / Set brick color
            var spriteRenderer = brickObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && rowColors.Length > 0)
            {
                Color color = rowColors[row % rowColors.Length];
                spriteRenderer.color = color;
            }
            
            Debug.Log($"[BrickGrid] Spawned brick at ({row}, {col}): {position}");
        }
        
        /// <summary>
        /// 清除所有砖块 / Clear all bricks
        /// </summary>
        private void ClearGrid()
        {
            // 销毁所有子对象 / Destroy all children
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public GameObject BrickPrefab => brickPrefab;
        public int Rows => rows;
        public int Columns => columns;
        
        #endregion
    }
}
