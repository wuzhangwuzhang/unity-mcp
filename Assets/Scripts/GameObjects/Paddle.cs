using UnityEngine;

namespace BreakoutGame.GameObjects
{
    /// <summary>
    /// 挡板控制器，处理玩家输入和移动
    /// Paddle controller that handles player input and movement
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Paddle : MonoBehaviour
    {
        [Header("Movement Settings / 移动设置")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;
        
        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;
        
        private void Awake()
        {
            // 获取组件引用 / Get component references
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            
            // 配置Rigidbody2D / Configure Rigidbody2D
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            
            Debug.Log("[Paddle] Initialized");
        }
        
        private void Start()
        {
            // 从GameConfig加载配置（如果存在）/ Load config from GameConfig if exists
            var config = Resources.Load<Data.GameConfig>("GameConfig");
            if (config != null)
            {
                moveSpeed = config.paddleMoveSpeed;
                minX = config.paddleMinX;
                maxX = config.paddleMaxX;
                
                // 获取当前关卡配置 / Get current level config
                int currentLevel = 1;
                if (Core.GameManager.Instance != null)
                {
                    currentLevel = Core.GameManager.Instance.GetCurrentLevel();
                }
                
                // 加载关卡特定的paddle宽度 / Load level-specific paddle width
                int levelIndex = currentLevel - 1;
                float paddleWidth = config.paddleWidth; // 默认宽度 / Default width
                
                if (levelIndex >= 0 && levelIndex < config.levelConfigs.Length)
                {
                    paddleWidth = config.levelConfigs[levelIndex].paddleWidth;
                    Debug.Log($"[Paddle] Level {currentLevel} - Width: {paddleWidth}");
                }
                
                // 应用paddle宽度 / Apply paddle width
                transform.localScale = new Vector3(paddleWidth, config.paddleHeight, 1f);
                
                Debug.Log($"[Paddle] Loaded config - Speed: {moveSpeed}, Range: [{minX}, {maxX}], Width: {paddleWidth}");
            }
            
            // 确保初始位置在边界内 / Ensure initial position is within bounds
            ClampPosition();
        }
        
        private void Update()
        {
            // 只在游戏进行中处理输入 / Only handle input when game is playing
            if (Core.GameManager.Instance != null && 
                Core.GameManager.Instance.CurrentState == Core.GameState.Playing)
            {
                HandleInput();
            }
        }
        
        /// <summary>
        /// 处理玩家输入 / Handle player input
        /// </summary>
        private void HandleInput()
        {
            float horizontalInput = 0f;
            
            // 检测键盘输入 / Detect keyboard input
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontalInput = 1f;
            }
            
            // 检测触摸/鼠标输入（移动设备支持）/ Detect touch/mouse input (mobile support)
            if (horizontalInput == 0f)
            {
                // 检测触摸输入 / Detect touch input
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        // 点击屏幕左半侧向左移动，右半侧向右移动
                        // Tap left half to move left, right half to move right
                        if (touch.position.x < Screen.width / 2f)
                        {
                            horizontalInput = -1f;
                        }
                        else
                        {
                            horizontalInput = 1f;
                        }
                    }
                }
                // 检测鼠标输入（PC上测试移动设备逻辑）/ Detect mouse input (for testing mobile logic on PC)
                else if (Input.GetMouseButton(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    // 点击屏幕左半侧向左移动，右半侧向右移动
                    // Click left half to move left, right half to move right
                    if (mousePosition.x < Screen.width / 2f)
                    {
                        horizontalInput = -1f;
                    }
                    else
                    {
                        horizontalInput = 1f;
                    }
                }
            }
            
            // 如果有输入，移动挡板 / If there's input, move paddle
            if (horizontalInput != 0f)
            {
                Move(horizontalInput);
            }
        }
        
        /// <summary>
        /// 移动挡板 / Move paddle
        /// </summary>
        /// <param name="direction">移动方向（-1左，1右）/ Movement direction (-1 left, 1 right)</param>
        public void Move(float direction)
        {
            // 计算新位置 / Calculate new position
            Vector3 newPosition = transform.position;
            newPosition.x += direction * moveSpeed * Time.deltaTime;
            
            // 应用新位置 / Apply new position
            transform.position = newPosition;
            
            // 限制在边界内 / Clamp within bounds
            ClampPosition();
        }
        
        /// <summary>
        /// 限制挡板位置在边界内 / Clamp paddle position within bounds
        /// </summary>
        private void ClampPosition()
        {
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, minX, maxX);
            transform.position = position;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 检测与球的碰撞 / Detect collision with ball
            if (collision.gameObject.CompareTag("Ball"))
            {
                // 获取碰撞点 / Get collision point
                Vector2 collisionPoint = collision.contacts[0].point;
                
                // 触发球-挡板碰撞事件 / Trigger ball-paddle collision event
                Core.GameEvents.TriggerBallPaddleCollision(collisionPoint);
                
                Debug.Log($"[Paddle] Ball collision at position: {collisionPoint}");
            }
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public float MoveSpeed => moveSpeed;
        public float MinX => minX;
        public float MaxX => maxX;
        
        #endregion
    }
}
